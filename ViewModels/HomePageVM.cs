using Quartets.ModelLogic;
using System.Windows.Input;

namespace Quartets.ViewModels
{
    public class HomePageVM
    {
        private readonly User user = new();
        public ICommand PlayCommand { get; }
        public string UserName
        {
            get => user.UserName;
            set
            {
                user.UserName = value;
            }
        }
        public HomePageVM()
        {
            PlayCommand = new Command(Play);
        }

        private void Play(object? sender)
        {
            MainThread.InvokeOnMainThreadAsync(() =>
            {
                if (Application.Current != null)
                {
                    Application.Current.MainPage = new AppShell();
                }
            });
        }
    }
}
