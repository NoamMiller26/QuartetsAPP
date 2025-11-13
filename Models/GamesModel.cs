
using Plugin.CloudFirestore;
using Quartets.ModelLogic;

using System.Collections.ObjectModel;

namespace Quartets.Models
{
    public abstract class GamesModel
    {
        protected IListenerRegistration? ilr;
        protected FBData fbd = new();
        protected Game? currentGame;
        public bool IsBusy { get; set; }
        public EventHandler<Game>? OnGameAdded;
        public ObservableCollection<Game>? GamesList { get; set; } = [];
        public ObservableCollection<NumberOfPlayers>? NumberOfPlayersList { get; set; } = [new NumberOfPlayers(4), new NumberOfPlayers(5), new NumberOfPlayers(6)];
        public NumberOfPlayers SelectedNumberOfPlayers { get; set; } = new NumberOfPlayers();
        public Game? CurrentGame { get => currentGame; set => currentGame = value; }
        public EventHandler? OnGamesChanged;
        public abstract void AddSnapshotListener();
        public abstract void RemoveSnapshotListener();

    }
}
