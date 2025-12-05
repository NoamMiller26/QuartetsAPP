
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Plugin.CloudFirestore;
using Quartets.Models;
using System.Threading.Tasks;

namespace Quartets.ModelLogic
{
    public class Games : GamesModel
    {
        public override void AddGame()
        {
            int size = (new Game()).MaxNumOfPlayers;
            IsBusy = true;
            CurrentGame = new(SelectedNumberOfPlayers)
            {
                IsHostUser = true
            };

            CurrentGame.PlayersNames?[0] = (new User()).UserName;
            CurrentGame.PlayersIds?[0] = fbd.UserId;

            currentGame?.OnGameDeleted += OnGameDeleted;
            CurrentGame.SetDocument(OnComplete);
        }

        private void OnGameDeleted(object? sender, EventArgs e)
        {
            MainThread.InvokeOnMainThreadAsync(() =>
            {
                Toast.Make(Strings.GameDeleted, ToastDuration.Long).Show();
            });
        }
        public Games()
        {

        }
        protected override void OnComplete(Task task)
        {
            IsBusy = false;
            OnGameAdded?.Invoke(this, CurrentGame!);
        }
        public override void AddSnapshotListener()
        {
            ilr = fbd.AddSnapshotListener(Keys.GamesCollection, OnChange!);
        }
        public override void RemoveSnapshotListener()
        {
            ilr?.Remove();
        }
        private void OnChange(IQuerySnapshot snapshot, Exception error)
        {
            fbd.GetDocumentsWhereEqualTo(Keys.GamesCollection, nameof(GameModel.IsFull), false, OnComplete);
        }
        private void OnComplete(IQuerySnapshot qs)
        {
            GamesList!.Clear();
            foreach (IDocumentSnapshot ds in qs.Documents)
            {
                Game? game = ds.ToObject<Game>();
                if (game != null)
                {
                    game.Id = ds.Id;
                    game.Init();
                    GamesList.Add(game);
                }
            }
            OnGamesChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}

