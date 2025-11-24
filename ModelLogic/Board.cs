using Quartets.ModelLogic;
using Quartets.Models;
using System.Collections.Generic;
using System.Linq;

namespace Quartets.ModelsLogic
{
    public class Board : BoardModel
    {
        public Board(int initialHandSize = 4) : base(initialHandSize)
        {
        }

        public bool RequestCardFromOpponent(Card requestedCard, Board opponentBoard)
        {
            Card? matchingCard = opponentBoard.Hand.FirstOrDefault(card => card.Value == requestedCard.Value);
            if (matchingCard != null)
            {
                opponentBoard.RemoveCardFromHand(matchingCard);
                AddCardToHand(matchingCard);
                return true;
            }
            return false;
        }

        public List<List<Card>> CheckCompletedSets()
        {
            List<List<Card>> completedSets = GetCompleteSets();
            return completedSets;
        }
    }
}
