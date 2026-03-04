using Quartets.ModelLogic;
using Quartets.ViewModels;

namespace Quartets;

public partial class MainPage : ContentPage
{
    #region Fields

    private readonly MainPageVM mpVM = new();

    #endregion

    #region Constructor

    public MainPage()
    {
        InitializeComponent();
        BindingContext = mpVM;
    }

    #endregion

    #region Overrides

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

    #endregion
}       
    


