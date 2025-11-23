
using Plugin.CloudFirestore;
using Plugin.CloudFirestore.Attributes;
using Quartets.ModelLogic;

namespace Quartets.Models
{
    public abstract class GameModel
    {
        protected IListenerRegistration? ilr;
        private readonly Games Games = new();
        protected FBData fbd = new();
        [Ignored]
        public Player[]? PlayersArr { get; set; }
        protected Board GameBoard = new();
        protected GameStatus _status = new();
        [Ignored]
        public EventHandler? OnGameChanged;
        [Ignored]
        public EventHandler? OnGameDeleted;
        public string HostName { get; set; } = string.Empty;
        public string[]? PlayersNames { get; set; }
        public DateTime Created { get; set; }
        public int MaxNumOfPlayers { get; set; }
        public bool IsFull { get; set; }
        public int CurrentNumOfPlayers { get; set; } = 1;
        [Ignored]
        public string Id { get; set; } = string.Empty;
        [Ignored]
        public string MyName { get; set; } = new User().UserName;
        [Ignored]
        public bool IsHostUser { get; set; }
        [Ignored]
        public string NumOfPlayersName => $"{MaxNumOfPlayers}";
        [Ignored]
        public NumberOfPlayers? NumberOfPlayers { get; set; }
        [Ignored]
        public string TimeName => $"{Time} min";
        public int Time { get; set; }
        protected abstract GameStatus Status { get; }
        [Ignored]
        public string StatusMessage => Status.StatusMessage;
        public bool IsHostTurn { get; set; } = false;
        protected abstract void UpdateStatus();
        public abstract void SetDocument(Action<System.Threading.Tasks.Task> OnComplete);
        public abstract void AddSnapShotListener();
        public abstract void RemoveSnapShotListener();
        public abstract void DeleteDocument(Action<System.Threading.Tasks.Task> OnComplete);
    }
}
