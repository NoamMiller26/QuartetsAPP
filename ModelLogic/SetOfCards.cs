using Quartets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quartets.ModelLogic
{
    public class SetOfCards : SetOfCardsModel
    {
        public SetOfCards()
        {
            cards = [];
            usedCards = [];
            FillPakage();
        }
        protected override bool IsExist(Card currCard)
        {
            bool res = false;
            foreach (Card card in usedCards!)
            {
                if (currCard.Shape == card.Shape && currCard.Value == card.Value)
                {
                    res = true;
                }
            }
            return res;
        }
        protected override void FillPakage()
        {
            foreach (CardModel.Shapes shape in Enum.GetValues(typeof(CardModel.Shapes)))
                for (int value = 1; value <= Card.CardsInShape; value++)
                    cards!.Add(new Card(shape, value));
        }
        public override Card GetRandomCard()
        {
            Card card = null!;
            while (card == null)
                card = cards![rnd.Next(cards.Count)];
            usedCards!.Add(card);
            return card;

        }
        public override Card Add(Card card)
        {
            cards!.Add(card);
            return card;
        }
    }
}