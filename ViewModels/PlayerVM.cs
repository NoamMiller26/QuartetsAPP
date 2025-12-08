using System.Collections.ObjectModel;
using System.Windows.Input;
using Quartets.Models;
using Quartets.ModelLogic;
using System.Linq;


namespace Quartets.ViewModels
{
    public class PlayerVM : ObservableObject
    {
        public Player player;

        public string Name => player.Name;
        public ObservableCollection<Card> HandObservable => player.HandObservable;

        // האם זה השחקן המקומי (למשל ה-user שנכנס)
        public bool IsLocalPlayer { get; set; } = false;

        // עבור הצגת גב קלפים (רשימה של פריטים לשכפול התמונות)
        public ObservableCollection<int> PlaceHolderBacks { get; } = new ObservableCollection<int>();

     

        public PlayerVM(Player p,  bool isLocal = false)
        {
            player = p;
            IsLocalPlayer = isLocal;

            // מלא רצף של גבים לפי מספר הקלפים (עדפתי עד 6 לשונה)
            UpdatePlaceholders();

            // דוגמה לפקודה של לשאול
            AskCommand = new Command(async () => await ExecuteAsk());
            // נסמן מאזין לשינויים ביד
            HandObservable.CollectionChanged += (s, e) => UpdatePlaceholders();
        }

        private void UpdatePlaceholders()
        {
            PlaceHolderBacks.Clear();
            int n = HandObservable?.Count ?? 0;
            int show = Math.Min(n, 8);
            for (int i = 0; i < show; i++) PlaceHolderBacks.Add(i);
        }

        public ICommand AskCommand { get; }

        private async System.Threading.Tasks.Task ExecuteAsk()
        {
            // בקשה מהשחקן המקומי לבחור איזה קלף לשאול וממי.
            // כאן נשלח קריאה ל-GameVM לטפל בלוגיקה.
          

            // דוגמה: מוציאים חיבור UI פשוט לבחירה. כאן אנו מניחים שה-GamePage יטפל בבחירה
           
        }
    }
}
