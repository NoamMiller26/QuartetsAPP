using Quartets.ModelLogic;
using System.Windows.Input;

namespace Quartets.ViewModels
{
    public class HomePageVM
    {
        private readonly User user = new();
        private readonly MainPageML mainPageML = new();
        public ICommand PlayCommand { get; }
        public ICommand InstructionsCommand { get; private set; }
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
            InstructionsCommand = new Command(ShowInstructionsPrompt);
        }
        public void ShowInstructionsPrompt(object obj)
        {
            mainPageML.ShowInstructionsPrompt(obj);
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
