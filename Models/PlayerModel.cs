using Quartets.ModelLogic;
using System;
using System.Collections.Generic;

namespace Quartets.Models
{
    public abstract class PlayerModel
    {
        private readonly SetOfCards setCards = new();
        private readonly Random random = new Random();

        public List<Card> Hand { get; private set; } = new();
        public string Name { get; private set; }

        public PlayerModel(string name)
        {
            Name = name;

            // מחלקים 4 קלפים אקראיים לשחקן
            for (int i = 0; i < 4; i++)
            {
                Card newCard = setCards.GetRandomCard();
                Hand.Add(newCard);
            }
        }
    }
}
