using Quartets.ModelLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quartets.Models
{
    class CardModel : ImageButton
    {
        public class Card
        {
            public string Category { get; set; }=string.Empty;
            public string Group { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
        }
        List<Card> deck = new List<Card>
{
    // ===== Animals =====
    // Savanna
    new Card { Category="Animals", Group="Savanna", Name="Lion" },
    new Card { Category="Animals", Group="Savanna", Name="Elephant" },
    new Card { Category="Animals", Group="Savanna", Name="Giraffe" },
    new Card { Category="Animals", Group="Savanna", Name="Zebra" },

    // Forest
    new Card { Category="Animals", Group="Forest", Name="Bear" },
    new Card { Category="Animals", Group="Forest", Name="Wolf" },
    new Card { Category="Animals", Group="Forest", Name="Fox" },
    new Card { Category="Animals", Group="Forest", Name="Deer" },

    // Ocean
    new Card { Category="Animals", Group="Ocean", Name="Dolphin" },
    new Card { Category="Animals", Group="Ocean", Name="Shark" },
    new Card { Category="Animals", Group="Ocean", Name="Octopus" },
    new Card { Category="Animals", Group="Ocean", Name="Turtle" },

    // Birds
    new Card { Category="Animals", Group="Birds", Name="Eagle" },
    new Card { Category="Animals", Group="Birds", Name="Owl" },
    new Card { Category="Animals", Group="Birds", Name="Parrot" },
    new Card { Category="Animals", Group="Birds", Name="Penguin" },

    // ===== Vehicles =====
    // Cars
    new Card { Category="Vehicles", Group="Cars", Name="Sedan" },
    new Card { Category="Vehicles", Group="Cars", Name="SUV" },
    new Card { Category="Vehicles", Group="Cars", Name="Sports Car" },
    new Card { Category="Vehicles", Group="Cars", Name="Electric Car" },

    // Air Vehicles
    new Card { Category="Vehicles", Group="Air", Name="Airplane" },
    new Card { Category="Vehicles", Group="Air", Name="Helicopter" },
    new Card { Category="Vehicles", Group="Air", Name="Jet" },
    new Card { Category="Vehicles", Group="Air", Name="Glider" },

    // Water Vehicles
    new Card { Category="Vehicles", Group="Water", Name="Boat" },
    new Card { Category="Vehicles", Group="Water", Name="Ship" },
    new Card { Category="Vehicles", Group="Water", Name="Submarine" },
    new Card { Category="Vehicles", Group="Water", Name="Jet Ski" },

    // Work Vehicles
    new Card { Category="Vehicles", Group="Work", Name="Fire Truck" },
    new Card { Category="Vehicles", Group="Work", Name="Ambulance" },
    new Card { Category="Vehicles", Group="Work", Name="Tractor" },
    new Card { Category="Vehicles", Group="Work", Name="Bulldozer" },

    // ===== Sports =====
    // Ball Sports
    new Card { Category="Sports", Group="Ball Sports", Name="Football" },
    new Card { Category="Sports", Group="Ball Sports", Name="Basketball" },
    new Card { Category="Sports", Group="Ball Sports", Name="Tennis" },
    new Card { Category="Sports", Group="Ball Sports", Name="Volleyball" },

    // Water Sports
    new Card { Category="Sports", Group="Water Sports", Name="Swimming" },
    new Card { Category="Sports", Group="Water Sports", Name="Surfing" },
    new Card { Category="Sports", Group="Water Sports", Name="Diving" },
    new Card { Category="Sports", Group="Water Sports", Name="Water Polo" },

    // Combat Sports
    new Card { Category="Sports", Group="Combat Sports", Name="Boxing" },
    new Card { Category="Sports", Group="Combat Sports", Name="Judo" },
    new Card { Category="Sports", Group="Combat Sports", Name="Karate" },
    new Card { Category="Sports", Group="Combat Sports", Name="Taekwondo" },

    // Athletics
    new Card { Category="Sports", Group="Athletics", Name="Running" },
    new Card { Category="Sports", Group="Athletics", Name="High Jump" },
    new Card { Category="Sports", Group="Athletics", Name="Long Jump" },
    new Card { Category="Sports", Group="Athletics", Name="Javelin" }
};
    }
}
