using Quartets.Models;
using System.Collections.ObjectModel;

namespace Quartets.ModelLogic
{
    public class Player : PlayerModel
    {
      
        public ObservableCollection<Card> HandObservable { get; private set; } = new();
        public Player(string playerName, string id) : base(playerName, id)
        {
            foreach (Card card in Hand)
                HandObservable.Add(card);
        }
        

     
        public void AddCard(Card card)
        {
            Hand.Add(card);
            HandObservable.Add(card);
        }

       
        public void RemoveCard(Card card)
        {
            Hand.Remove(card);
            HandObservable.Remove(card);
        }


        public List<List<Card>> CheckQuartets()
        {
            List<List<Card>> quartets = Hand
                .GroupBy(card => card.Value)
                .Where(group => group.Count() == 4)
                .Select(group => group.ToList())
                .ToList();

            return quartets;
        }
    }
}
