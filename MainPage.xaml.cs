using Quartets.ModelLogic;
using Quartets.ViewModels;

namespace Quartets;

public partial class MainPage : ContentPage
{
    private readonly MainPageVM mpVM = new();
    public MainPage()
    {
        InitializeComponent();
        BindingContext = mpVM;
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        mpVM.AddSnapshotListener();
    }

    protected override void OnDisappearing()
    {
        mpVM.RemoveSnapshotListener();
        base.OnDisappearing();
    }

    private async void OnGameItemTapped(object sender, ItemTappedEventArgs e)
    {
        if (e.Item is Game game)
        {
            // Set the current game in the ViewModel
            mpVM.SelectedItem = game;
            
            try
            {
                if (Shell.Current != null)
                {
                    await Shell.Current.Navigation.PushAsync(new Views.GamePage(game), true);
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                System.Diagnostics.Debug.WriteLine($"Navigation error: {ex.Message}");
            }
        }
        
        // Clear the selection
        if (sender is ListView listView)
        {
            listView.SelectedItem = null;
        }
    }
}       
    
