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
            // Subscribe to quartet completion event to show popup
            gpVM.OnQuartetCompletedEvent += ShowQuartetCompletedPopup;
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
            
            // Navigate back to homepage after showing popup
            await Task.Delay(2000); // Wait 2 seconds for user to see the popup
            
            try
            {
                // Clean up before navigating
                gpVM.OnGameEndedEvent -= ShowGameEndPopup;
                gpVM.OnQuartetCompletedEvent -= ShowQuartetCompletedPopup;
                gpVM.RemoveSnapshotListener();
                
                // Navigate back to homepage
                if (Shell.Current != null && Shell.Current.Navigation != null)
                {
                    await Shell.Current.Navigation.PopToRootAsync();
                }
            }
            catch (Exception ex)
            {
                // Log error but don't crash - navigation might have already happened
                System.Diagnostics.Debug.WriteLine($"Error during game cleanup: {ex.Message}");
            }
        }

        private async void ShowQuartetCompletedPopup(string playerName, string playerId)
        {
            // Check if the current player completed the quartet
            bool isCurrentPlayer = game.CurrentPlayer != null && game.CurrentPlayer.Id == playerId;
            
            var completedSetPopup = new CompletedSetPopUp(playerName, isCurrentPlayer);
            await this.ShowPopupAsync(completedSetPopup);
        }
        
        protected override void OnAppearing()
        {
            base.OnAppearing();
            gpVM.AddSnapshotListener();

        }

        protected override void OnDisappearing()
        {
            gpVM.OnGameEndedEvent -= ShowGameEndPopup;
            gpVM.OnQuartetCompletedEvent -= ShowQuartetCompletedPopup;
            gpVM.RemoveSnapshotListener();
            base.OnDisappearing();
        }
    }
}