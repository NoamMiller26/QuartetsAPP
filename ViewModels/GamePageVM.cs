using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Quartets.ModelLogic;
using Quartets.Models;
using Quartets.ModelsLogic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Quartets.ViewModels
{
    public partial class GamePageVM : ObservableObject
    {
        private readonly Game game;
        private readonly Board playerBoard;
        public string MyName;
        public ObservableCollection<Player> Players { get => game.Players; set => game.Players = value; }
        public ObservableCollection<PlayerVM> OtherPlayers { get => game.OtherPlayers; set => game.OtherPlayers = value; }
        public ICommand NextTurnCommand => new Command(NextTurn);
        public string CurrentStatus => game.CurrentStatus;
        public bool IsMyTurn => CurrentPlayer.IsCurrentTurn;
        public Player CurrentPlayer { get => game.CurrentPlayer; set => game.CurrentPlayer = value; }
        public PlayerVM CurrentPlayerVM { get; set; }
        public ObservableCollection<Card> PlayerHand { get; } = new ObservableCollection<Card>();


        public GamePageVM(Game game)
        {
            this.game = game;
            playerBoard = new Board();
            MyName = game.MyName;        
            CurrentPlayerVM = new PlayerVM(game.CurrentPlayer);           
            if (!game.IsHostUser)
            {
                game.UpdateGuestUser(OnComplete);
            }          
            int handCount = playerBoard.Hand.Count;
            for (int i = 0; i < handCount; i++)
            {
                Card card = playerBoard.Hand[i];
                PlayerHand.Add(card);
            }

            game.OnGameChanged += OnGameChanged;
        }


        private void NextTurn(object obj)
        {
            game.NextTurn();
            OnPropertyChanged(nameof(CurrentStatus));
        }

        private void OnGameChanged(object sender, EventArgs e)
        {
            OnPropertyChanged(nameof(Players));
            OnPropertyChanged(nameof(CurrentStatus));
            OnPropertyChanged(nameof(IsMyTurn));
        }

        private void OnComplete(Task task)
        {
            if (!task.IsCompletedSuccessfully)
            {
                Toast.Make(Strings.JoinGameErr,
                    ToastDuration.Long, 14).Show();
            }
        }


        public void RequestCard(Card requestedCard, Board opponentBoard)
        {
            bool success = playerBoard.RequestCardFromOpponent(requestedCard, opponentBoard);

            if (success)
            {
                PlayerHand.Clear();

                int handCount = playerBoard.Hand.Count;
                for (int i = 0; i < handCount; i++)
                {
                    Card card = playerBoard.Hand[i];
                    PlayerHand.Add(card);
                }
            }

            List<List<Card>> completedSets = playerBoard.CheckCompletedSets();

            int setsCount = completedSets.Count;
            for (int i = 0; i < setsCount; i++)
            {
                List<Card> set = completedSets[i];

                int setSize = set.Count;
                for (int j = 0; j < setSize; j++)
                {
                    Card card = set[j];
                    PlayerHand.Remove(card);
                }
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
