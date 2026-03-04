using Quartets.Models;

namespace Quartets.ModelLogic
{
    public class SetOfCards : SetOfCardsModel
    {
        #region Constructor

        public SetOfCards()
        {
            cards = [];
            usedCards = [];
            FillPakage();
        }

        #endregion

        #region Protected Methods

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

        #endregion

        #region Public Methods

        public override Card GetRandomCard()
        {
            if (cards == null || !cards.Any())
            {
                return null!;
            }

            int idx = rnd.Next(cards.Count);
            Card card = cards[idx];
            cards.RemoveAt(idx);
            usedCards!.Add(card);
            return card;

        }

        public override Card Add(Card card)
        {
            cards!.Add(card);
            return card;
        }

        #endregion
    }
}