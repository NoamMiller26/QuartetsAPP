using Quartets.Models;
using Quartets.ModelLogic;
using Quartets.Views;
using System.Windows.Input;

namespace Quartets.ViewModels
{
    internal partial class RegisterPageVM : ObservableObject
    {
        public ICommand ToggleIsPasswordCommand { get; }
        public bool IsPassword { get; set; } = true;
        public ICommand RegisterCommand { get; }
        private readonly User user = new();
        public bool CanRegister()
        {
            return user.CanRegister();
        }
        public RegisterPageVM()
        {
            RegisterCommand = new Command(Register, CanRegister);
            ToggleIsPasswordCommand = new Command(ToggleIsPassword);
            user.OnAuthCompleted += OnAuthComplete;
        }
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

    }
}
