using System.Collections.ObjectModel;
using System.Windows.Input;
using Quartets.Models;
using Quartets.ModelLogic;
using System.Linq;
using System;
using Microsoft.Maui.Controls;

namespace Quartets.ViewModels
{
    public partial class PlayerVM : ObservableObject
    {
        private readonly Player player;
        private readonly Func<PlayerVM, Task> onAsk;
        private bool isLocalPlayer;

        public string Name => player.Name;
        public string Id => player.Id;

        // === תיקון 1: שימוש ב-HandObservable ===
        public ObservableCollection<Card> HandObservable => player.HandObservable;

        public bool IsLocalPlayer
        {
            get => isLocalPlayer;
            private set
            {
                if (isLocalPlayer != value)
                {
                    isLocalPlayer = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<int> PlaceHolderBacks { get; } = new ObservableCollection<int>();

        public ICommand AskCommand { get; }

        public PlayerVM(Player p, bool isLocal, Func<PlayerVM, Task> onAskCallback)
        {
            player = p;
            isLocalPlayer = isLocal;
            onAsk = onAskCallback;

            UpdatePlaceholders();

            AskCommand = new Command(async () => await ExecuteAsk(), () => !IsLocalPlayer);

            // האזנה לשינויים ביד השחקן
            player.HandObservable.CollectionChanged += PlayerHand_CollectionChanged;
        }

        private void PlayerHand_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdatePlaceholders();
            OnPropertyChanged(nameof(HandObservable));
        }

        private void UpdatePlaceholders()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                PlaceHolderBacks.Clear();
                int n = HandObservable?.Count ?? 0;
                int show = Math.Min(n, 8);
                for (int i = 0; i < show; i++)
                {
                    PlaceHolderBacks.Add(i);
                }
            });
        }

        private async Task ExecuteAsk()
        {
            if (onAsk != null)
            {
                await onAsk(this);
            }
        }
    }
}