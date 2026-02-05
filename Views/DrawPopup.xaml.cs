using CommunityToolkit.Maui.Views;
using Quartets.ViewModels;

namespace Quartets.Views;

public partial class DrawPopup : Popup
{
    public DrawPopup()
    {
        InitializeComponent();
        BindingContext = new DrawPopupVM();
    }
}


