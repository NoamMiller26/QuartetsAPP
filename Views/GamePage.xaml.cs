using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls;
using Quartets.ModelLogic;
using Quartets.ViewModels;

namespace Quartets.Views
{
    public partial class GamePage : ContentPage
    {
        private readonly GamePageVM gpVM;
        private readonly Game game;
        public GamePage(Game game)
        {
            Console.WriteLine("MEIR create game page");
            InitializeComponent();
            this.game = game;
            gpVM = new GamePageVM(game);
            BindingContext = gpVM;
            
            // Subscribe to game end event to show popups
            gpVM.OnGameEndedEvent += ShowGameEndPopup;
        }
        
        private async void ShowGameEndPopup(string winnerName)
        {
            // Check if current player is the winner
            bool isWinner = game.CurrentPlayer != null && game.CurrentPlayer.Name == winnerName;
            
            if (isWinner)
            {
                var winningPopup = new WinningPopup(winnerName);
                await this.ShowPopupAsync(winningPopup);
            }
            else
            {
                var losingPopup = new LosingPopup(winnerName);
                await this.ShowPopupAsync(losingPopup);
            }
        }
        
        protected override void OnAppearing()
        {
            base.OnAppearing();
            gpVM.AddSnapshotListener();

        }

        protected override void OnDisappearing()
        {
            gpVM.OnGameEndedEvent -= ShowGameEndPopup;
            gpVM.RemoveSnapshotListener();
            base.OnDisappearing();
        }
    }
}