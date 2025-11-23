using Quartets.ViewModels;

namespace Quartets.Views;

public partial class HomePage : ContentPage
{
	public HomePage()
	{
		InitializeComponent();
        BindingContext = new HomePageVM();
    }
}