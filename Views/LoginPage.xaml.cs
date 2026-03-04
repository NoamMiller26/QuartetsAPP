using Quartets.ViewModels;

namespace Quartets.Views;

public partial class LoginPage : ContentPage
{
    #region Constructor

	public LoginPage()
	{
		InitializeComponent();
        BindingContext = new LoginPageVM();
    }

    #endregion
}