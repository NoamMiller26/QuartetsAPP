


using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Microsoft.Maui.Controls;
using Plugin.CloudFirestore;
using Quartets.Models;
using Quartets.ViewModels;
using System.Linq;


namespace Quartets.ModelLogic
{
    public class Game : GameModel
    {

        public override string CurrentStatus
        {

            get => CurrentPlayer.IsCurrentTurn ? "play please" : "please wait";
            set;
        }
        public override void NextTurn()
        {

            Players[CurrentPlayerIndex].IsCurrentTurn = false;

            CurrentPlayerIndex = (CurrentPlayerIndex + 1) % Players.Count;

            Players[CurrentPlayerIndex].IsCurrentTurn = true;

            OnGameChanged?.Invoke(this, EventArgs.Empty);
        }

        public Game(GameTime selectedGameTime)
        {
            HostName = new User().UserName;
            IsHostUser = true;
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
            CurrentPlayerIndex = 0;
            PlayersNames = new string[MaxNumOfPlayers];
            PlayersIds = new string[MaxNumOfPlayers];
            FillDummes();
            Players = [];
            OtherPlayers = [];
            createPlayers();
        }

        private void FillDummes()
        {
            for (int i = 0; i < MaxNumOfPlayers; i++)
            {
                PlayersNames[i] = "";
                PlayersIds[i] = "";
            }
        }
        protected override void createPlayers()
        {
            int i = 0;
            foreach (string playerName in PlayersNames!)
            {
                if (playerName != "")
                {
                    Player player = new(playerName, PlayersIds[i++]);
                    Players!.Add(player);
                    if (player.Id == fbd.UserId)
                    {
                        CurrentPlayer = player;
                    }
                    else
                    {
                        OtherPlayers.Add(new PlayerVM(player));
                    }

                }

            }
            if (CurrentPlayer == null)
            {
                CurrentPlayer = new Player(MyName, fbd.UserId);
            }
        }
        public Game()
        {
         
          
        }
        public override void Init()
        {
            createPlayers();
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
            foreach (string id in PlayersIds)
            {
                if (id == fbd.UserId)
                    return;
            }

            Console.WriteLine(CurrentNumOfPlayers + "/" + MaxNumOfPlayers);
            PlayersNames?[CurrentNumOfPlayers] = MyName;
            PlayersIds?[CurrentNumOfPlayers] = fbd.UserId;
            Console.WriteLine("init players");
            CurrentNumOfPlayers++;
            if (CurrentNumOfPlayers == MaxNumOfPlayers)
            {
                IsFull = true;
            }
            Console.WriteLine("joining");
            UpdateFireBaseJoinGame(OnComplete);
        }

        private void UpdateFireBaseJoinGame(Action<Task> OnComplete)
        {
            Dictionary<string, object> dict = new()
            {
                { nameof(PlayersNames), PlayersNames! },
                { nameof(PlayersIds), PlayersIds! },
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
            if (Players.Count() == MaxNumOfPlayers && CurrentPlayerIndex != updatedGame.CurrentPlayerIndex)
            {
                int prevCurrnetPlayerIndex = CurrentPlayerIndex;
                CurrentPlayerIndex = updatedGame.CurrentPlayerIndex;
                Players[CurrentPlayerIndex].IsCurrentTurn = true;
                Players[prevCurrnetPlayerIndex].IsCurrentTurn = false;
            }
            if (updatedGame != null)
            {
                if (IsFull == false && updatedGame.IsFull == true)
                {
                    Players[0].IsCurrentTurn = true;
                    Console.WriteLine(Players[0].IsCurrentTurn);
                }
                IsFull = updatedGame.IsFull;
                PlayersNames = updatedGame.PlayersNames;
                PlayersIds = updatedGame.PlayersIds;

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


