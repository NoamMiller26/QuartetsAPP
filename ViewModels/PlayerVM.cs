using Quartets.Models;
using Quartets.ModelLogic;
using System.Collections.ObjectModel;

namespace Quartets.ViewModels
{
    public class PlayerVM
    {
        public Player player;

        public string Name => player.Name;

        // נקשר ל-UI ישירות לאוסף של השחקן
        public ObservableCollection<Card> HandObservable => player.HandObservable;

        public PlayerVM(Player player)
        {
            this.player = player;
        }
    }
}
