using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Quartets.ModelLogic;
using Quartets.Models;
using Quartets.Views;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Quartets.ViewModels
{
    public partial class MainPageVM : ObservableObject
    {
        #region Fields

        private readonly Games games = new();
        private readonly User user = new();
        private readonly MainPageML mainPageML = new();

        #endregion

        #region Commands

        public ICommand InstructionsCommand { get; private set; }
        public ICommand AddGameCommand => new Command(AddGame);

        #endregion

        #region Properties

        public ObservableCollection<NumberOfPlayers>? NumberOfPlayersList
        {
            get => games.NumberOfPlayersList;
            set => games.NumberOfPlayersList = value;
        }

        public NumberOfPlayers SelectedNumberOfPlayers
        {
            get => games.SelectedNumberOfPlayers;
            set => games.SelectedNumberOfPlayers = value;
        }

        public ObservableCollection<Game>? GamesList => games.GamesList;
        public string UserName => user.UserName;
        public bool IsBusy => games.IsBusy;

        public Game? SelectedItem
        {
            get => games.CurrentGame;

            set
            {
                if (value != null)
                {
                    Console.WriteLine("opening game");
                    games.CurrentGame = value;
                    MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        Shell.Current.Navigation.PushAsync(new GamePage(value), true);
                    });
                }
            }
        }

        #endregion

        #region Constructor

        public MainPageVM()
        {
            InstructionsCommand = new Command(ShowInstructionsPrompt);
            games.OnGameAdded += OnGameAdded;
            games.OnGamesChanged += OnGamesChanged;
        }

        #endregion

        #region Public Methods

        public void ShowInstructionsPrompt(object obj)
        {
            mainPageML.ShowInstructionsPrompt(obj);
        }

        public void AddSnapshotListener()
        {
            games.AddSnapshotListener();
        }

        public void RemoveSnapshotListener()
        {
            games.RemoveSnapshotListener();
        }

        #endregion

        #region Private Methods

        private void AddGame()
        {
            games.AddGame();
            OnPropertyChanged(nameof(IsBusy));
        }

        private void OnGameAdded(object? sender, Game game)
        {
            OnPropertyChanged(nameof(IsBusy));
            MainThread.InvokeOnMainThreadAsync(() =>
            {
                Shell.Current.Navigation.PushAsync(new GamePage(game), true);
            });
        }

        private void OnGamesChanged(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(GamesList));
        }

        #endregion
    }
}

