using Quartets.Models;
using System.Collections.ObjectModel;

namespace Quartets.ModelLogic
{
    public class Player : PlayerModel
    {
        // ObservableCollection כדי שה־UI יתעדכן אוטומטית כשקלפים משתנים
        public ObservableCollection<Card> HandObservable { get; private set; } = new();

        public Player(string name) : base(name)
        {
            // העברת הקלפים מרשימת ה־Hand ל־ObservableCollection
            foreach (Card card in Hand)
                HandObservable.Add(card);
        }

        /// <summary>
        /// מוסיף קלף ליד השחקן
        /// </summary>
        public void AddCard(Card card)
        {
            Hand.Add(card);
            HandObservable.Add(card);
        }

        /// <summary>
        /// מסיר קלף מיד השחקן
        /// </summary>
        public void RemoveCard(Card card)
        {
            Hand.Remove(card);
            HandObservable.Remove(card);
        }

        /// <summary>
        /// בודק אם יש רביעיות ומחזיר רשימה של רביעיות
        /// </summary>
        public List<List<Card>> CheckQuartets()
        {
            List<List<Card>> quartets = Hand
                .GroupBy(card => card.Value)
                .Where(group => group.Count() == 4)
                .Select(group => group.ToList())
                .ToList();

            return quartets;
        }
    }
}
