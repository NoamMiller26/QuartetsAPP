using Quartets.ModelLogic;
using Quartets.Models;
using System.Collections.ObjectModel;

namespace Quartets.ViewModels
{
    public class PlayerVM
    {
        private readonly Player player;

        public PlayerVM(Player player)
        {
            this.player = player;

            // העתקה של ה־ObservableCollection מה־Model
            HandObservable = player.HandObservable;
        }

        public string Name => player.Name;

        // קלפים של השחקן
        public ObservableCollection<Card> HandObservable { get; private set; }
    }
}
