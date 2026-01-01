using Quartets.Models;
using Plugin.CloudFirestore;
using Plugin.CloudFirestore.Attributes;
namespace Quartets.ModelLogic
{
    public class Card : CardModel
    {
        public Card(Shapes shape, int value) : base(shape, value)
        {
        }
    }
}
