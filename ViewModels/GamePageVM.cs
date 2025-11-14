using CommunityToolkit.Maui.Core;
using Quartets.Models;
using Quartets.ModelLogic;
using CommunityToolkit.Maui.Alerts;
using Microsoft.Maui.Dispatching;

namespace Quartets.ViewModels
{
    public partial class GamePageVM : ObservableObject
    {
        private readonly Game game;

        
        public string OpponentsNames =>
            game.Players == null || game.Players.Length == 0
            ? "Waiting for players..."
            : game.OpponentsNames;

        public string MyName => game.MyName;

        public GamePageVM(Game game)
        {
            this.game = game;

            
            MainThread.BeginInvokeOnMainThread(() =>
            {
                OnPropertyChanged(nameof(OpponentsNames));
            });

           
            game.OnGameChanged += OnGameChanged;

            
            if (!game.IsHostUser)
                game.UpdateGuestUser(OnComplete);
        }

        private void OnGameChanged(object? sender, EventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                OnPropertyChanged(nameof(OpponentsNames));
            });
        }

        private void OnComplete(Task task)
        {
            if (!task.IsCompletedSuccessfully)
                Toast.Make(Strings.JoinGameErr, ToastDuration.Long).Show();
        }

        public void AddSnapshotListener()
        {
            game.AddSnapShotListener();
        }

        public void RemoveSnapshotListener()
        {
            game.RemoveSnapShotListener();
        }
    }
}
