using Quartets.ModelLogic;
using System.Windows.Input;

namespace Quartets.ViewModels
{
    public class HomePageVM
    {
        #region Fields

        private readonly User user = new();
        private readonly MainPageML mainPageML = new();

        #endregion

        #region Commands

        public ICommand PlayCommand { get; }
        public ICommand InstructionsCommand { get; private set; }

        #endregion

        #region Properties

        public string UserName
        {
            get => user.UserName;
            set
            {
                user.UserName = value;
            }
        }

        #endregion

        #region Constructor

        public HomePageVM()
        {
            PlayCommand = new Command(Play);
            InstructionsCommand = new Command(ShowInstructionsPrompt);
        }

        #endregion

        #region Public Methods

        public void ShowInstructionsPrompt(object obj)
        {
            mainPageML.ShowInstructionsPrompt(obj);
        }

        #endregion

        #region Private Methods

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

        #endregion
    }
}
