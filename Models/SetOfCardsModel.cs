using Quartets.ModelLogic;

namespace Quartets.Models
{
    public abstract class SetOfCardsModel
    {
        #region Fields

        protected List<Card>? cards;
        protected List<Card>? usedCards;
        protected Random rnd = new();

        #endregion

        #region Public Methods

        public abstract Card GetRandomCard();
        public abstract Card Add(Card card);

        #endregion

        #region Protected Methods

        protected abstract void FillPakage();
        protected abstract bool IsExist(Card currCard);

        #endregion
    }
}
