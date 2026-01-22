using CommunityToolkit.Maui.Views;
using Quartets.ViewModels;

namespace Quartets.Views;

public partial class CompletedSetPopUp : Popup
{
	private readonly CompletedSetPopUpVM completedSetPopUpVM;

	public CompletedSetPopUp(string playerName, bool isCurrentPlayer)
	{
		InitializeComponent();
		completedSetPopUpVM = new CompletedSetPopUpVM(playerName, isCurrentPlayer);
		BindingContext = completedSetPopUpVM;
	}
}