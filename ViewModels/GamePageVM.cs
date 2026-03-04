using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Quartets.ModelLogic;
using Quartets.Models;
using Quartets.ModelsLogic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Controls;
using Keyboard = Microsoft.Maui.Keyboard;

namespace Quartets.ViewModels
{
    public partial class GamePageVM : ObservableObject
    {
        #region Fields

        private readonly Game game;
        private readonly ObservableCollection<PlayerVM> opponents = new();
        private IDispatcherTimer? turnTimer;
        private (long startMs, int index)? lastHostTimeoutHandled;
        private long localTurnSeenTicks;
        private long lastSeenTurnStartUnixMs;
        private int lastSeenTurnIndex = -1;

        #endregion

        #region Properties

        public string MyName => game.MyName;

        // היד המקומית
        public ObservableCollection<Card> PlayerHand => game.CurrentPlayer?.HandObservable ?? new ObservableCollection<Card>();

        // יריבים בלבד (בשביל התצוגה)
        public ObservableCollection<PlayerVM> Opponents => opponents;

        public string CurrentStatus => game.CurrentStatus;
        public bool IsMyTurn => CurrentPlayer.IsCurrentTurn;
        public Player CurrentPlayer => game.CurrentPlayer;
        public int DeckCount => game.Deck.Count;
        public int CompletedSets => game.CurrentPlayer?.CompletedSets ?? 0;

        public int RemainingSeconds { get; private set; } = 60;
        public bool IsTurnTimerVisible => game.IsFull;

        #endregion

        #region Events

        public event Action<string>? OnGameEndedEvent;
        public event Action? OnGameDrawnEvent;
        public event Action<string, string>? OnQuartetCompletedEvent; // Parameters: playerName, playerId

        #endregion

        #region Constructor

        public GamePageVM(Game game)
        {
            this.game = game;

            if (!game.IsHostUser)
            {
                game.UpdateGuestUser(OnComplete);
            }

            BuildPlayerVMs();

            game.OnGameChanged += OnGameChanged;
            game.OnGameEnded += OnGameEnded;
            game.OnGameDrawn += OnGameDrawn;
            game.OnQuartetCompleted += OnQuartetCompleted;
        }

        #endregion

        #region Public Methods

        public void AddSnapshotListener()
        {
            game.AddSnapShotListener();
            StartTurnTimer();
        }

        public void RemoveSnapshotListener()
        {
            StopTurnTimer();
            game.RemoveSnapShotListener();
        }

        #endregion

        #region Private Methods

        private void BuildPlayerVMs()
        {
            opponents.Clear();

            foreach (Player player in game.Players)
            {
                bool isLocal = game.CurrentPlayer != null && player.Id == game.CurrentPlayer.Id;
                var vm = new PlayerVM(player, isLocal, AskOpponentAsync);

                if (!vm.IsLocalPlayer)
                {
                    opponents.Add(vm);
                }
            }
        }

        private async Task AskOpponentAsync(PlayerVM opponent)
        {
            if (!IsMyTurn)
            {
                await Toast.Make(Strings.NotYourTurn, ToastDuration.Short, 14).Show();
                return;
            }

            string valueInput = await Shell.Current.DisplayPromptAsync("שאלה", $"איזה ערך לבקש מ-{opponent.Name}?", "OK", "ביטול", "1-13", 2, keyboard: Keyboard.Numeric);
            if (!int.TryParse(valueInput, out int value) || value < 1 || value > 13)
            {
                await Toast.Make("ערך לא חוקי", ToastDuration.Short, 14).Show();
                return;
            }

            bool hasRankInHand = CurrentPlayer?.HandObservable.Any(c => c.Value == value) ?? false;
            if (!hasRankInHand)
            {
                await Toast.Make("אפשר לבקש רק ערך שקיים ביד שלך", ToastDuration.Short, 14).Show();
                return;
            }

            bool accepted = await game.AskForCard(CurrentPlayer, opponent.Id, value);
            if (!accepted)
            {
                await Toast.Make("לא ניתן לבצע את הבקשה כעת", ToastDuration.Short, 14).Show();
            }
        }

        private void OnGameChanged(object sender, EventArgs e)
        {
            OnPropertyChanged(nameof(PlayerHand));
            OnPropertyChanged(nameof(CurrentStatus));
            OnPropertyChanged(nameof(IsMyTurn));
            OnPropertyChanged(nameof(DeckCount));
            OnPropertyChanged(nameof(CompletedSets));
            OnPropertyChanged(nameof(IsTurnTimerVisible));

            BuildPlayerVMs();
            UpdateTurnTracking();
            UpdateRemainingSeconds();
        }

        private void OnComplete(Task task)
        {
            if (!task.IsCompletedSuccessfully)
            {
                Toast.Make(Strings.JoinGameErr,
                    ToastDuration.Long, 14).Show();
            }
        }

        private void OnGameEnded(object sender, string winnerName)
        {
            // Notify the page to show the appropriate popup
            OnGameEndedEvent?.Invoke(winnerName);
        }

        private void OnGameDrawn(object sender, EventArgs e)
        {
            OnGameDrawnEvent?.Invoke();
        }

        private void OnQuartetCompleted(object sender, (string playerName, string playerId) e)
        {
            // Notify the page to show the popup
            OnQuartetCompletedEvent?.Invoke(e.playerName, e.playerId);
        }

        private void StartTurnTimer()
        {
            if (turnTimer != null)
            {
                return;
            }

            turnTimer = Application.Current?.Dispatcher.CreateTimer();
            if (turnTimer == null)
            {
                return;
            }

            turnTimer.Interval = TimeSpan.FromSeconds(1);
            turnTimer.Tick += (_, __) =>
            {
                UpdateRemainingSeconds();
                TryHostAutoAdvanceOnTimeout();
            };
            turnTimer.Start();

            UpdateTurnTracking();
            UpdateRemainingSeconds();
        }

        private void StopTurnTimer()
        {
            if (turnTimer != null)
            {
                turnTimer.Stop();
                turnTimer = null;
            }
        }

        private void UpdateRemainingSeconds()
        {
            int seconds = CalculateRemainingSeconds();
            if (seconds != RemainingSeconds)
            {
                RemainingSeconds = seconds;
                OnPropertyChanged(nameof(RemainingSeconds));
            }
        }

        private int CalculateRemainingSeconds()
        {
            if (!game.IsFull || game.TurnDurationSeconds <= 0)
            {
                return game.TurnDurationSeconds > 0 ? game.TurnDurationSeconds : 60;
            }

            // If we haven't seen a turn start yet, show full duration
            if (localTurnSeenTicks == 0)
            {
                return game.TurnDurationSeconds;
            }

            long nowTicks = Environment.TickCount64;
            long elapsedMs = Math.Max(0, nowTicks - localTurnSeenTicks);
            long remainingMs = (game.TurnDurationSeconds * 1000L) - elapsedMs;
            int remainingSeconds = (int)Math.Ceiling(remainingMs / 1000.0);
            return Math.Clamp(remainingSeconds, 0, game.TurnDurationSeconds);
        }

        private void UpdateTurnTracking()
        {
            // Reset local time anchor when we see a new turn start from Firebase
            if (game.TurnStartUnixMs != lastSeenTurnStartUnixMs ||
                game.CurrentPlayerIndex != lastSeenTurnIndex)
            {
                lastSeenTurnStartUnixMs = game.TurnStartUnixMs;
                lastSeenTurnIndex = game.CurrentPlayerIndex;
                localTurnSeenTicks = Environment.TickCount64;
            }
        }

        private void TryHostAutoAdvanceOnTimeout()
        {
            if (!game.IsHostUser || !game.IsFull)
            {
                return;
            }

            // Only host advances, and only once per (turn start, index)
            var key = (game.TurnStartUnixMs, game.CurrentPlayerIndex);
            if (lastHostTimeoutHandled.HasValue && lastHostTimeoutHandled.Value == key)
            {
                return;
            }

            if (CalculateRemainingSeconds() <= 0)
            {
                lastHostTimeoutHandled = key;
                game.NextTurn();
            }
        }

        // Android native timer integration removed to ensure all platforms use the same
        // Firestore-based clock, avoiding drift between devices.

        #endregion
    }
}