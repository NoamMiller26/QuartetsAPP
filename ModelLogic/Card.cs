using Quartets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Quartets.Models.CardModel;

namespace Quartets.ModelLogic
{
    public class Card : CardModel
    {
        public Card(Shapes shape, int value) : base(shape, value)
        {
        }
    }
}
