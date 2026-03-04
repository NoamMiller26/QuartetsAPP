using CommunityToolkit.Maui.Views;
using Quartets.ViewModels;

namespace Quartets.Views;

public partial class WinningPopup : Popup
{
    #region Fields

    private readonly WinningPopupVM WinningPopupVM;

    #endregion

    #region Constructor

    public WinningPopup(string name)
    {
        InitializeComponent();
        WinningPopupVM = new WinningPopupVM(name);
        BindingContext = WinningPopupVM;
    }

    #endregion
}