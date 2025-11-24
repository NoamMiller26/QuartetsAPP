using Quartets.Models;
using Quartets.ModelLogic;
using System.Collections.Generic;
using System.Linq;

namespace Quartets.ModelsLogic
{
    public abstract class BoardModel
    {
        protected readonly SetOfCards setOfCards = new();
        public List<Card> Hand { get; protected set; } = new();

        public BoardModel(int initialHandSize = 4)
        {
            for (int i = 0; i < initialHandSize; i++)
                Hand.Add(setOfCards.GetRandomCard());
        }

        public void AddCardToHand(Card card) => Hand.Add(card);
        public void RemoveCardFromHand(Card card) => Hand.Remove(card);

        public List<List<Card>> GetCompleteSets()
        {
            return Hand
                .GroupBy(card => card.Value)
                .Where(group => group.Count() == 4)
                .Select(group => group.ToList())
                .ToList();
        }
    }
}
