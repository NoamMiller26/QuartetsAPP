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
        public ICommand AddGameCommand => new Command(AddGame);
        private void AddGame()
        {
            if (!IsBusy)
            {
                games.AddGame();
                OnPropertyChanged(nameof(IsBusy));
            }
        }
        public ObservableCollection<GameTime>? GameTimes { get => games.GameTimes; set => games.GameTimes = value; }
        public GameTime SelectedGameTime { get => games.SelectedGameTime; set => games.SelectedGameTime = value; }
        private readonly User user = new();
     
        public ObservableCollection<NumberOfPlayers>? NumberOfPlayersList { get => games.NumberOfPlayersList; set => games.NumberOfPlayersList = value; }
        public NumberOfPlayers SelectedNumberOfPlayers { get => games.SelectedNumberOfPlayers; set => games.SelectedNumberOfPlayers = value; }
      
    
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
        
        public MainPageVM()
        {
           
            games.OnGameAdded += OnGameAdded;
            games.OnGamesChanged += OnGamesChanged;
        }


        private void OnGamesChanged(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(GamesList));
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
       

    }
}

