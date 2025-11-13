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

        private readonly Games games = new();
        private readonly User user = new();
        private readonly MainPageML mainPageML = new();
        public ObservableCollection<NumberOfPlayers>? NumberOfPlayersList { get => games.NumberOfPlayersList; set => games.NumberOfPlayersList = value; }
        public NumberOfPlayers SelectedNumberOfPlayers { get => games.SelectedNumberOfPlayers; set => games.SelectedNumberOfPlayers = value; }
        public ICommand InstructionsCommand { get; private set; }
        public ICommand AddGameCommand => new Command(AddGame);
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
                    games.CurrentGame = value;
                    MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        Shell.Current.Navigation.PushAsync(new GamePage(value), true);
                    });
                }
            }
        }
        private void AddGame()
        {
            games.AddGame();
            OnPropertyChanged(nameof(IsBusy));
        }
        public MainPageVM()
        {
            InstructionsCommand = new Command(ShowInstructionsPrompt);
            games.OnGameAdded += OnGameAdded;
            games.OnGamesChanged += OnGamesChanged;
        }
        public void ShowInstructionsPrompt(object obj)
        {
            mainPageML.ShowInstructionsPrompt(obj);
        }

        private void OnGameAdded(object? sender, Game game)
        {
            OnPropertyChanged(nameof(IsBusy));
            MainThread.InvokeOnMainThreadAsync(() =>
            {
                Shell.Current.Navigation.PushAsync(new GamePage(game), true);
            });
        }
        public void AddSnapshotListener()
        {
            games.AddSnapshotListener();
        }

        public void RemoveSnapshotListener()
        {
            games.RemoveSnapshotListener();
        }
        private void OnGamesChanged(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(GamesList));
        }

    }
}

