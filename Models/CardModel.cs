using Quartets.ModelLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quartets.Models
{
    public class CardModel : ImageButton
    {
        public CardModel(Shapes shape, int value)
        {
            Shape = shape;
            Value = value;
            if (value > 0)
                Source = CardsImage[(int)shape, value - 1];
            Aspect = Aspect.AspectFit;
            HorizontalOptions = new LayoutOptions(LayoutAlignment.Start, false);
            WidthRequest = 100;
        }
        private static readonly string[,] CardsImage = {
        {"ace_club.png","two_club.png","three_club.png","four_club.png","five_club.png","six_club.png","seven_club.png","eight_club.png","nine_club.png","ten_club.png","jack_club.png","queen_club.png","king_club.png"  },
        {"ace_diamond.png","two_diamond.png","three_diamond.png","four_diamond.png","five_diamond.png","six_diamond.png","seven_diamond.png","eight_diamond.png","nine_diamond.png","ten_diamond.png","jack_diamond.png","queen_diamond.png","king_diamond.png"  },
        {"ace_heart.png","two_heart.png","three_heart.png","four_heart.png","five_heart.png","six_heart.png","seven_heart.png","eight_heart.png","nine_heart.png","ten_heart.png","jack_heart.png","queen_heart.png","king_heart.png" },
        {"ace_spade.png","two_spade.png","three_spade.png","four_spade.png" ,"five_spade.png","six_spade.png","seven_spade.png","eight_spade.png","nine_spade.png" ,"ten_spade.png","jack_spade.png","queen_spade.png","king_spade.png"}};
        public enum Shapes { Club, Diamond, Heart, Spade };
        public static int CardsInShape
        {
            get
            {
                return CardsImage.GetLength(1);
            }
        }
        public Shapes Shape { get; set; }
        public int Value { get; set; }
    }
}
