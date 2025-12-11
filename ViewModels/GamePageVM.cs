using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Quartets.ModelLogic;
using Quartets.Models;
using Quartets.ModelsLogic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Microsoft.Maui.Controls;
using Keyboard = Microsoft.Maui.Keyboard;

namespace Quartets.ViewModels
{
    public partial class GamePageVM : ObservableObject
    {
        private readonly Game game;
        private readonly ObservableCollection<PlayerVM> opponents = new();

        public string MyName => game.MyName;

        // היד המקומית
        public ObservableCollection<Card> PlayerHand => game.CurrentPlayer?.HandObservable ?? new ObservableCollection<Card>();

        // יריבים בלבד (בשביל התצוגה)
        public ObservableCollection<PlayerVM> Opponents => opponents;

        public ICommand NextTurnCommand => new Command(NextTurn);
        public string CurrentStatus => game.CurrentStatus;
        public bool IsMyTurn => CurrentPlayer.IsCurrentTurn;
        public Player CurrentPlayer => game.CurrentPlayer;
        public int DeckCount => game.Deck.Count;

        public GamePageVM(Game game)
        {
            this.game = game;

            if (!game.IsHostUser)
            {
                game.UpdateGuestUser(OnComplete);
            }

            BuildPlayerVMs();

            game.OnGameChanged += OnGameChanged;
        }

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

            string suit = await Shell.Current.DisplayActionSheet("איזה סוג?", "ביטול", null, "Clubs", "Diamonds", "Hearts", "Spades");
            if (string.IsNullOrWhiteSpace(suit) || suit == "ביטול")
            {
                return;
            }

            CardModel.Shapes shape = suit switch
            {
                "Clubs" => CardModel.Shapes.Club,
                "Diamonds" => CardModel.Shapes.Diamond,
                "Hearts" => CardModel.Shapes.Heart,
                "Spades" => CardModel.Shapes.Spade,
                _ => CardModel.Shapes.Club
            };

            await game.AskForShape(CurrentPlayer, opponent.Id, value, shape);
        }

        private void NextTurn(object obj)
        {
            game.NextTurn();
            OnPropertyChanged(nameof(CurrentStatus));
            OnPropertyChanged(nameof(IsMyTurn));
        }

        private void OnGameChanged(object sender, EventArgs e)
        {
            OnPropertyChanged(nameof(PlayerHand));
            OnPropertyChanged(nameof(CurrentStatus));
            OnPropertyChanged(nameof(IsMyTurn));
            OnPropertyChanged(nameof(DeckCount));

            BuildPlayerVMs();
        }

        private void OnComplete(Task task)
        {
            if (!task.IsCompletedSuccessfully)
            {
                Toast.Make(Strings.JoinGameErr,
                    ToastDuration.Long, 14).Show();
            }
        }

        public void AddSnapshotListener()
        {
            game.AddSnapShotListener();
        }

        public void RemoveSnapshotListener()
        {
            game.RemoveSnapShotListener();
        }
    }
}