using CommunityToolkit.Maui.Views;
using Quartets.ViewModels;

namespace Quartets.Views;

public partial class CompletedSetPopUp : Popup
{
    #region Fields

    private readonly CompletedSetPopUpVM completedSetPopUpVM;

    #endregion

    #region Constructor

    public CompletedSetPopUp(string playerName, bool isCurrentPlayer)
    {
        InitializeComponent();
        completedSetPopUpVM = new CompletedSetPopUpVM(playerName, isCurrentPlayer);
        BindingContext = completedSetPopUpVM;
    }

    #endregion
}