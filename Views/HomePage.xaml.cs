using Quartets.ViewModels;

namespace Quartets.Views;

public partial class HomePage : ContentPage
{
    #region Constructor

	public HomePage()
	{
		InitializeComponent();
        BindingContext = new HomePageVM();
    }

    #endregion
}