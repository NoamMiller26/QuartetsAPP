using CommunityToolkit.Maui.Views;
using Quartets.ViewModels;

namespace Quartets.Views;



	public partial class WinningPopup : Popup
	{
        private readonly WinningPopupVM WinningPopupVM;


        public WinningPopup(string name)
        {
            InitializeComponent();
            WinningPopupVM = new WinningPopupVM(name);
            BindingContext = WinningPopupVM;
        }
    }