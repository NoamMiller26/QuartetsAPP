using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Quartets.Models;
using Quartets.ModelLogic;

namespace Quartets.ViewModels
{
    public class GameVM : ObservableObject
    {
        public ObservableCollection<PlayerVM> Players { get; } = new ObservableCollection<PlayerVM>();
        public ObservableCollection<Card> Deck { get; } = new ObservableCollection<Card>();

        private int currentPlayerIndex = 0;
        public int CurrentPlayerIndex
        {
            get => currentPlayerIndex;
            set { currentPlayerIndex = value; OnPropertyChanged(); }
        }

        public ICommand NextTurnCommand { get; }

        public GameVM()
        {
            NextTurnCommand = new Command(() => NextTurn());
        }

        public void NextTurn()
        {
            CurrentPlayerIndex = (CurrentPlayerIndex + 1) % Players.Count;
            OnPropertyChanged(nameof(CurrentPlayerIndex));
        }

        // ------------ חוקי רביעיות ------------
        public async Task HandleAskRequest(PlayerVM askingPlayerVM)
        {
            var askingPlayer = askingPlayerVM.player;

            // בחירת מי לשאול
            PlayerVM target = await AskUserToChooseTargetPlayer(askingPlayerVM);
            if (target == null) return;

            // בחירת Value (1-13)
            int requestedValue = await AskUserForValue();
            if (requestedValue == -1) return;

            // מוצא כל הקלפים אצל היעד עם אותו value
            var matchingValueCards = target.player.Hand
                .Where(c => c.Value == requestedValue)
                .ToList();

            if (matchingValueCards.Any())
            {
                // אם צדק → עכשיו שואל על ה-Shape
                var requestedShape = await AskUserForShape();

                var exactCard = matchingValueCards
                    .FirstOrDefault(c => c.Shape == requestedShape);

                if (exactCard != null)
                {
                    // צדק גם ב-Shape → מקבל את הקלף
                    target.player.Hand.Remove(exactCard);
                    askingPlayer.Hand.Add(exactCard);

                    // נשאר בתור
                    return;
                }
                else
                {
                    // טעה בשאלה על shape → מושך מהקופה
                    DrawFromDeck(askingPlayer);
                    NextTurn();
                    return;
                }
            }
            else
            {
                // טעה ב-Value → מושך מהקופה
                DrawFromDeck(askingPlayer);
                NextTurn();
            }
        }

        private void DrawFromDeck(PlayerModel player)
        {
            if (Deck.Any())
            {
                var card = Deck[0];
                Deck.RemoveAt(0);
                player.Hand.Add(card);
            }
        }

        // ---------- פונקציות UI (Stub – תמלא לפי האפליקציה) ----------

        private Task<PlayerVM> AskUserToChooseTargetPlayer(PlayerVM asking)
        {
            // בגרסה אמיתית: חלון בחירה של שחקן.
            var target = Players.FirstOrDefault(p => p != asking);
            return Task.FromResult(target);
        }

        private Task<int> AskUserForValue()
        {
            // בגרסה אמיתית: בחירת מספר 1–13.
            return Task.FromResult(1);
        }

        private Task<CardModel.Shapes> AskUserForShape()
        {
            // בגרסה אמיתית: Popup לבחירת צורה.
            return Task.FromResult(CardModel.Shapes.Heart);
        }
    }
}
