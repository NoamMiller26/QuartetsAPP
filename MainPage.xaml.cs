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
}       
    


