

using Quartets.Models;
using Microsoft.Maui.Controls;
using Plugin.CloudFirestore;
using CommunityToolkit.Maui.Alerts;


namespace Quartets.ModelLogic
{
    public class Game : GameModel
    {
        public override string OpponentsNames => IsHostUser ? GuestName : HostName;
        protected override GameStatus Status => _status;

        public Game(GameTime selectedGameTime)
        {
            HostName = new User().UserName;
            IsHostUser = true;
            Time = selectedGameTime.Time;
            Created = DateTime.Now;
            UpdateStatus();
        }
        public Game(NumberOfPlayers selectedNumberOfPlayers)
        {
            HostName = new User().UserName;
            Created = DateTime.Now;
            NumberOfPlayers = selectedNumberOfPlayers;
            IsFull = false;
            CurrentNumOfPlayers = 1;
            MaxNumOfPlayers = selectedNumberOfPlayers.NumPlayers;
            Players = new string[MaxNumOfPlayers];
            UpdateStatus();
        }
        public Game()
        {
            UpdateStatus();
        }
        protected override void UpdateStatus()
        {
            _status.CurrentStatus = IsHostUser && IsHostTurn || !IsHostUser && !IsHostTurn ?
                GameStatus.Status.Play : GameStatus.Status.Wait;
        }
       









        public override void SetDocument(Action<Task> OnComplete)
        {
            Id = fbd.SetDocument(this, Keys.GamesCollection, Id, OnComplete);
        }


        public override void AddSnapShotListener()
        {
            ilr = fbd.AddSnapshotListener(Keys.GamesCollection, Id, OnChange);

        }

        public override void RemoveSnapShotListener()
        {
            ilr?.Remove();
            DeleteDocument(OnComplete);
        }


        private void OnComplete(Task task)
        {
            if (task.IsCompletedSuccessfully)
                OnGameDeleted?.Invoke(this, EventArgs.Empty);
        }

        private void OnChange(IDocumentSnapshot? snapshot, Exception? error)
        {
            Game? updatedGame = snapshot?.ToObject<Game>();
            if (updatedGame != null)
            {
                IsFull = updatedGame.IsFull;
                GuestName = updatedGame.GuestName;
                OnGameChanged?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                MainThread.InvokeOnMainThreadAsync(() =>
                {
                    Shell.Current.Navigation.PopAsync();
                    Toast.Make(Strings.GameCanceld, CommunityToolkit.Maui.Core.ToastDuration.Long, 14).Show();
                });

            }
        }

        public void UpdateGuestUser(Action<Task> OnComplete)
        {
            Players?[CurrentNumOfPlayers - 1] = MyName;
            CurrentNumOfPlayers++;
            if (CurrentNumOfPlayers == MaxNumOfPlayers)
                IsFull = true;
            UpdateFireBaseJoinGame(OnComplete);
        }

        private void UpdateFireBaseJoinGame(Action<Task> OnComplete)
        {
            Dictionary<string, object> dict = new()
            {
                { nameof(Players), Players! },
                { nameof(IsFull), IsFull },
                {  nameof(CurrentNumOfPlayers), CurrentNumOfPlayers }
            };
            fbd.UpdateFields(Keys.GamesCollection, Id, dict, OnComplete);
        }

        public override void DeleteDocument(Action<Task> OnComplete)
        {
            fbd.DeleteDocument(Keys.GamesCollection, Id, OnComplete);
        }
    }
}


