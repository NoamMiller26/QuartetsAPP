


using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Microsoft.Maui.Controls;
using Plugin.CloudFirestore;
using Quartets.Models;
using System.Linq;


namespace Quartets.ModelLogic
{
    public class Game : GameModel
    {
       
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
            PlayersNames = new string[MaxNumOfPlayers];
            Players = [];
            createPlayers();
            UpdateStatus();

        }
        private void createPlayers()
        {
            Players!.Add(new Player(HostName));
            foreach (string playerName in PlayersNames!)
            {
                if (playerName != null)
                {
                    Player player = new(playerName);
                    Players!.Add(player);
                }
            }

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

       
        

        public void UpdateGuestUser(Action<Task> OnComplete)
        {
            PlayersNames?[CurrentNumOfPlayers - 1] = MyName;
            CurrentNumOfPlayers++;
            if (CurrentNumOfPlayers == MaxNumOfPlayers)
                IsFull = true;
            UpdateFireBaseJoinGame(OnComplete);
        }

        private void UpdateFireBaseJoinGame(Action<Task> OnComplete)
        {
            Dictionary<string, object> dict = new()
            {
                { nameof(PlayersNames), PlayersNames! },
                { nameof(IsFull), IsFull },
                {  nameof(CurrentNumOfPlayers), CurrentNumOfPlayers }
            };
            fbd.UpdateFields(Keys.GamesCollection, Id, dict, OnComplete);
        }

        public override void DeleteDocument(Action<Task> OnComplete)
        {
            fbd.DeleteDocument(Keys.GamesCollection, Id, OnComplete);
        }
        private void OnChange(IDocumentSnapshot? snapshot, Exception? error)
        {
            Game? updatedGame = snapshot?.ToObject<Game>();
            if (updatedGame != null)
            {
                IsFull = updatedGame.IsFull;
                PlayersNames = updatedGame.PlayersNames;
                OnGameChanged?.Invoke(this, EventArgs.Empty);
            }
            else
            {

                MainThread.InvokeOnMainThreadAsync(() =>
                {
                    Shell.Current.Navigation.PopAsync();
                    Toast.Make(Strings.GameDeleted, ToastDuration.Long).Show();
                });
            }
        }
    }
}


