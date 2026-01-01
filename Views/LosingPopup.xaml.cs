using CommunityToolkit.Maui.Views;
using Quartets.ViewModels;

namespace Quartets.Views;

public partial class LosingPopup : Popup
{
	private readonly LosingPopupVM losingPopupVM;

	public LosingPopup(string winnerName)
	{
		InitializeComponent();
		losingPopupVM = new LosingPopupVM(winnerName);
		BindingContext = losingPopupVM;
	}
}