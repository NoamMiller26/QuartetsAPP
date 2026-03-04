using Quartets.Models;
using Quartets.ModelLogic;
using System.Collections.Generic;
using System.Linq;

namespace Quartets.ModelsLogic
{
    public abstract class BoardModel
    {
        #region Fields

        protected readonly SetOfCards setOfCards = new();

        #endregion

        #region Properties

        public List<Card> Hand { get; protected set; } = new();

        #endregion

        #region Constructor

        public BoardModel(int initialHandSize = 4)
        {
            for (int i = 0; i < initialHandSize; i++)
                Hand.Add(setOfCards.GetRandomCard());
        }

        #endregion

        #region Public Methods

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

        #endregion
    }
}
