using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Quartets.ModelLogic;
using Quartets.Models;
using Quartets.ModelsLogic;
using System.Collections.ObjectModel;

namespace Quartets.ViewModels
{
    public partial class GamePageVM : ObservableObject
    {
        private readonly Game game;
        private readonly Board PlayerBoard = new();

        public string MyName => game.MyName;
        public string StatusMessage => game.StatusMessage;

        public ObservableCollection<PlayerVM> Players { get; } = new();

        public ObservableCollection<Card> PlayerHand { get; } = new();

        public GamePageVM(Game game, Grid board)
        {
            this.game = game;
            game.OnGameChanged += OnGameChanged;
        

            // טעינת כל השחקנים ללא foreach
            for (int i = 0; i < game.Players.Count; i++)
            {
                Player p = game.Players[i];
                PlayerVM vm = new PlayerVM(p);
                Players.Add(vm);
            }

            if (!game.IsHostUser)
                game.UpdateGuestUser(OnComplete);

            // טעינת יד השחקן
            for (int i = 0; i < PlayerBoard.Hand.Count; i++)
            {
                Card card = PlayerBoard.Hand[i];
                PlayerHand.Add(card);
            }
        }

        private void OnGameChanged(object? sender, EventArgs e)
        {
            // UI update
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
            bool success = PlayerBoard.RequestCardFromOpponent(requestedCard, opponentBoard);

            if (success)
            {
                PlayerHand.Clear();

                for (int i = 0; i < PlayerBoard.Hand.Count; i++)
                {
                    Card card = PlayerBoard.Hand[i];
                    PlayerHand.Add(card);
                }
            }

            List<List<Card>> completedSets = PlayerBoard.CheckCompletedSets();

            for (int i = 0; i < completedSets.Count; i++)
            {
                List<Card> set = completedSets[i];

                for (int j = 0; j < set.Count; j++)
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
