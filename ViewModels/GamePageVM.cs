using CommunityToolkit.Maui.Alerts;
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
        public ObservableCollection<Player> Players => game.Players;

        public ObservableCollection<Card> PlayerHand { get; } = new();

        public GamePageVM(Game game)
        {
            this.game = game;
            game.OnGameChanged += OnGameChanged;

            if (!game.IsHostUser)
                game.UpdateGuestUser(OnComplete);

            foreach (Card card in PlayerBoard.Hand)
                PlayerHand.Add(card);
        }

        private void OnGameChanged(object? sender, System.EventArgs e)
        {
            // כאן ניתן לעדכן את ה־UI או לטעון נתונים מחדש
        }

        private void OnComplete(Task task)
        {
            if (!task.IsCompletedSuccessfully)
            {
                Toast.Make(Strings.JoinGameErr, CommunityToolkit.Maui.Core.ToastDuration.Long, 14);
            }
        }

        /// <summary>
        /// בקשת קלף משחקן אחר ועדכון היד
        /// </summary>
        public void RequestCard(Card requestedCard, Board opponentBoard)
        {
            bool success = PlayerBoard.RequestCardFromOpponent(requestedCard, opponentBoard);
            if (success)
            {
                PlayerHand.Clear();
                foreach (Card card in PlayerBoard.Hand)
                    PlayerHand.Add(card);
            }

            List<List<Card>> completedQuartets = PlayerBoard.CheckCompletedSets();
            foreach (List<Card> quartet in completedQuartets)
            {
                foreach (Card card in quartet)
                    PlayerHand.Remove(card);
            }
        }

        public void AddSnapshotListener() => game.AddSnapShotListener();
        public void RemoveSnapshotListener() => game.RemoveSnapShotListener();
    }
}
