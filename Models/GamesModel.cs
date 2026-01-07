
using Plugin.CloudFirestore;
using Quartets.ModelLogic;

using System.Collections.ObjectModel;

namespace Quartets.Models
{
    public abstract class GamesModel
    {
        protected FBData fbd = new();
        protected IListenerRegistration? ilr;
        protected Game? currentGame;      
        public NumberOfPlayers SelectedNumberOfPlayers { get; set; } = new NumberOfPlayers();
        public bool IsBusy { get; set; }
        public Game? CurrentGame { get => currentGame; set => currentGame = value; }
        public ObservableCollection<Game>? GamesList { get; set; } = [];
        public ObservableCollection<NumberOfPlayers>? NumberOfPlayersList { get; set; } = [new NumberOfPlayers(2), new NumberOfPlayers(3), new NumberOfPlayers(4), new NumberOfPlayers(5)];
        public EventHandler<Game>? OnGameAdded;
        public EventHandler? OnGamesChanged;
        public abstract void AddSnapshotListener();
        public abstract void RemoveSnapshotListener();
        public abstract void AddGame();
        protected abstract void OnComplete(Task task);

    }
}
