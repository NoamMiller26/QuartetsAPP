

using Quartets.Models;
using Microsoft.Maui.Controls;
using Plugin.CloudFirestore;


namespace Quartets.ModelLogic
{
    public class Game : GameModel
    {

        public Game(GameTime selectedGameTime)
        {
            HostName = new User().UserName;
            Time = selectedGameTime.Time;
            Created = DateTime.Now;
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
        }
        public Game()
        {
        }
        public override string OpponentsNames => GetNoneMeOpponentName();

        public int Time { get; }

        private string GetNoneMeOpponentName()
        {
            string players = string.Empty;
            foreach (string player in Players!)
            {
                if (player != MyName)
                    players += player + " ";
            }
            if (MyName != HostName)
                players += HostName;
            return players;
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
                Players = updatedGame.Players;
                OnGameChanged?.Invoke(this, EventArgs.Empty);
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


