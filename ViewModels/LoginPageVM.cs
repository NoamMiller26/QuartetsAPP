using Quartets.ModelLogic;
using Quartets.Models;
using Quartets.Views;
using System.Windows.Input;

namespace Quartets.ViewModels
{
    internal partial class LoginPageVM : ObservableObject
    {
        #region Fields

        private readonly User user = new();

        #endregion

        #region Commands

        public ICommand ToggleIsPasswordCommand { get; }
        public ICommand LoginCommand { get; }
        public ICommand LoginWithGitHubCommand { get; }

        #endregion

        #region Properties

        public bool IsPassword { get; set; } = true;

        public string UserName
        {
            get => user.UserName;
            set
            {
                user.UserName = value;
                (LoginCommand as Command)?.ChangeCanExecute();
            }
        }

        public string Password
        {
            get => user.Password;
            set
            {
                user.Password = value;
                (LoginCommand as Command)?.ChangeCanExecute();
            }
        }

        public string Email
        {
            get => user.Email;
            set
            {
                user.Email = value;
                (LoginCommand as Command)?.ChangeCanExecute();
            }
        }

        #endregion

        #region Constructor

        public LoginPageVM()
        {
            LoginCommand = new Command(Login, CanLogin);
            LoginWithGitHubCommand = new Command(async () => await LoginWithGitHubAsync());
            ToggleIsPasswordCommand = new Command(ToggleIsPassword);
            user.OnAuthCompleted += OnAuthComplete;
        }

        #endregion

        #region Public Methods

        public bool CanLogin()
        {
            return user.CanLogin();
        }

        #endregion

        #region Private Methods

        private void OnAuthComplete(object? sender, EventArgs e)
        {
            MainThread.InvokeOnMainThreadAsync(() =>
            {
                if (Application.Current != null)
                {
                    Application.Current.MainPage = new HomePage();
                }
            });
        }

        private void Login()
        {
            user.Login();
        }

        private async Task LoginWithGitHubAsync()
        {
            var ghUser = await GitHubAuthService.LoginAsync();
            if (ghUser == null)
            {
                return;
            }

            // Store minimal info locally so the rest of the app can use it (e.g., for display name).
            Preferences.Set(Keys.UserNameKey, ghUser.UserName);
            Preferences.Set(Keys.GitHubUserIdKey, ghUser.Id);

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                if (Application.Current != null)
                {
                    Application.Current.MainPage = new HomePage();
                }
            });
        }

        private void ToggleIsPassword()
        {
            IsPassword = !IsPassword;
            OnPropertyChanged(nameof(IsPassword));
        }

        #endregion
    }
}
