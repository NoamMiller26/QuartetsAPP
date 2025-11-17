
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Microsoft.Maui.Dispatching;
using Quartets.ModelLogic;
using Quartets.Models;

namespace Quartets.ViewModels
{
    public partial class GamePageVM : ObservableObject
    {
        private readonly Game game;
        public string MyName => game.MyName;
        public string StatusMessage => game.StatusMessage;

        public string OpponentsNames => game.OpponentsNames ?? string.Empty;




        public GamePageVM(Game game)
        {
            this.game = game;
            game.OnGameChanged += OnGameChanged;

            if (!game.IsHostUser)
                game.UpdateGuestUser(OnComplete);
        }


        private void OnGameChanged(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(OpponentsNames));
        }

        private void OnComplete(Task task)
        {
            if (!task.IsCompletedSuccessfully)
                Toast.Make(Strings.JoinGameErr, CommunityToolkit.Maui.Core.ToastDuration.Long, 14);

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
