
using Plugin.CloudFirestore;
using Plugin.CloudFirestore.Attributes;
using Quartets.ModelLogic;

namespace Quartets.Models
{
    public abstract class GameModel
    {
        protected FBData fbd = new();
        protected IListenerRegistration? ilr;
        [Ignored]
        public EventHandler? OnGameChanged;
        [Ignored]
        public EventHandler? OnGameDeleted;
        public string HostName { get; set; } = string.Empty;
        public string GuestName { get; set; } = string.Empty;
        public DateTime Created { get; set; }
        public int Time { get; set; }
        private readonly Games Games = new();
      
       
       
      
       
        public string[]? Players { get; set; }
  
        public int MaxNumOfPlayers { get; set; }
        public bool IsFull { get; set; }
        [Ignored]
        public int CurrentNumOfPlayers { get; set; } = 1;
        [Ignored]
        public string Id { get; set; } = string.Empty;
        [Ignored]
        public string MyName { get; set; } = new User().UserName;
        [Ignored]
        public abstract string OpponentsNames { get; }
        [Ignored]
        public bool IsHostUser { get; set; }
        [Ignored]
        public string NumOfPlayersName => $"{MaxNumOfPlayers}";
        [Ignored]
        public NumberOfPlayers? NumberOfPlayers { get; set; }
        public abstract void SetDocument(Action<System.Threading.Tasks.Task> OnComplete);
        public abstract void AddSnapShotListener();
        public abstract void RemoveSnapShotListener();
        public abstract void DeleteDocument(Action<System.Threading.Tasks.Task> OnComplete);
    }
}
