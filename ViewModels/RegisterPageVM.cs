using Quartets.Models;
using Quartets.ModelLogic;
using Quartets.Views;
using System.Windows.Input;

namespace Quartets.ViewModels
{
    internal partial class RegisterPageVM : ObservableObject
    {
        #region Fields

        private readonly User user = new();

        #endregion

        #region Commands

        public ICommand ToggleIsPasswordCommand { get; }
        public ICommand RegisterCommand { get; }

        #endregion

        #region Properties

        public bool IsPassword { get; set; } = true;

        public string UserName
        {
            get => user.UserName;
            set
            {
                user.UserName = value;
                (RegisterCommand as Command)?.ChangeCanExecute();
            }
        }

        public string Password
        {
            get => user.Password;
            set
            {
                user.Password = value;
                (RegisterCommand as Command)?.ChangeCanExecute();
            }
        }

        public string Email
        {
            get => user.Email;
            set
            {
                user.Email = value;
                (RegisterCommand as Command)?.ChangeCanExecute();
            }
        }

        public string Age
        {
            get => user.Age;
            set
            {
                user.Age = value;
                (RegisterCommand as Command)?.ChangeCanExecute();
            }
        }

        #endregion

        #region Constructor

        public RegisterPageVM()
        {
            RegisterCommand = new Command(Register, CanRegister);
            ToggleIsPasswordCommand = new Command(ToggleIsPassword);
            user.OnAuthCompleted += OnAuthComplete;
        }

        #endregion

        #region Public Methods

        public bool CanRegister()
        {
            return user.CanRegister();
        }

        #endregion

        #region Private Methods

        private void OnAuthComplete(object? sender, EventArgs e)
        {
            MainThread.InvokeOnMainThreadAsync(() =>
            {
                if (Application.Current != null)
                {
                    Application.Current.MainPage = new LoginPage();
                }
            });
        }

        private void ToggleIsPassword()
        {
            IsPassword = !IsPassword;
            OnPropertyChanged(nameof(IsPassword));
        }

        private void Register()
        {
            user.Register();
        }

        #endregion
    }
}
