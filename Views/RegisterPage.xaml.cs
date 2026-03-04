using Quartets.ViewModels;

namespace Quartets.Views;

public partial class RegisterPage : ContentPage
{
    #region Constructor

	public RegisterPage()
	{
		InitializeComponent();
        BindingContext = new RegisterPageVM();
    }

    #endregion
}