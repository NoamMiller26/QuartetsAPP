using CommunityToolkit.Maui.Views;
using Quartets.ViewModels;

namespace Quartets.Views;

public partial class LosingPopup : Popup
{
    #region Fields

    private readonly LosingPopupVM losingPopupVM;

    #endregion

    #region Constructor

    public LosingPopup(string winnerName)
    {
        InitializeComponent();
        losingPopupVM = new LosingPopupVM(winnerName);
        BindingContext = losingPopupVM;
    }

    #endregion
}