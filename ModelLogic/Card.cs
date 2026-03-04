using Quartets.Models;
using Plugin.CloudFirestore;
using Plugin.CloudFirestore.Attributes;
namespace Quartets.ModelLogic
{
    public class Card : CardModel
    {
        #region Constructor

        public Card(Shapes shape, int value) : base(shape, value)
        {
        }

        #endregion
    }
}
