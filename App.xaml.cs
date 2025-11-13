
using Quartets.ModelLogic;
using Quartets.Views;


namespace Quartets
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            User user = new();
            MainPage = user.IsRegistered ? new LoginPage() : new RegisterPage();
        }
    }
}
