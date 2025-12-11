using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Microsoft.Maui.Controls;
using Plugin.CloudFirestore.Attributes;
using Plugin.CloudFirestore;
using Quartets.Models;
using Quartets.ViewModels;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace Quartets.ModelLogic
{
    public class Game : GameModel
    {
        private IListenerRegistration requestsListener;

        public override string CurrentStatus
        {
            get
            {
                if (CurrentPlayer != null && CurrentPlayer.IsCurrentTurn)
                {
                    return "play please";
                }
                return "please wait";
            }
            set { }
        }

        [Ignored]
        public ObservableCollection<Card> Deck { get; } = new ObservableCollection<Card>();

        public Game() { }

        public Game(GameTime selectedGameTime)
        {
            HostName = new User().UserName;
            IsHostUser = true;
            Time = selectedGameTime.Time;
            Created = DateTime.Now;
        }

        public Game(NumberOfPlayers selectedNumberOfPlayers)
        {
            HostName = new User().UserName;
            Created = DateTime.Now;
            NumberOfPlayers = selectedNumberOfPlayers;
            IsFull = false;
            CurrentNumOfPlayers = 1;
            MaxNumOfPlayers = selectedNumberOfPlayers.NumPlayers;
            CurrentPlayerIndex = 0;
            PlayersNames = new string[MaxNumOfPlayers];
            PlayersIds = new string[MaxNumOfPlayers];
            FillDummes();
            Players = new ObservableCollection<Player>();
            OtherPlayers = new ObservableCollection<PlayerVM>();
            createPlayers();
            EnsureDeckInitialized();
        }

        private void FillDummes()
        {
            for (int i = 0; i < MaxNumOfPlayers; i++)
            {
                PlayersNames[i] = "";
                PlayersIds[i] = "";
            }
        }

        protected override void createPlayers()
        {
            int index = 0;
            foreach (string name in PlayersNames)
            {
                if (name != "")
                {
                    Player player = new Player(name, PlayersIds[index]);
                    index++;
                    Players.Add(player);

                    if (player.Id == fbd.UserId)
                    {
                        CurrentPlayer = player;
                    }
                    else
                    {
                        OtherPlayers.Add(new PlayerVM(player, false, null));
                    }
                }
            }

            if (CurrentPlayer == null && MyName != null && fbd.UserId != null)
            {
                CurrentPlayer = new Player(MyName, fbd.UserId);
            }

            EnsureDeckInitialized();
        }

        public override void Init()
        {
            createPlayers();
        }

        public override void SetDocument(Action<Task> OnComplete)
        {
            Id = fbd.SetDocument(this, Keys.GamesCollection, Id, OnComplete);
        }

        private IListenerRegistration AddRequestSnapshotListener()
        {
            return CrossCloudFirestore.Current.Instance
                .Collection(Keys.GamesCollection)
                .Document(Id)
                .Collection("Requests")
                .AddSnapshotListener(OnRequest);
        }

        public override void AddSnapShotListener()
        {
            ilr = fbd.AddSnapshotListener(Keys.GamesCollection, Id, OnChange);
            requestsListener = AddRequestSnapshotListener();
        }

        public override void RemoveSnapShotListener()
        {
            ilr.Remove();
            requestsListener?.Remove();
            DeleteDocument(OnComplete);
        }

        private void OnComplete(Task task)
        {
            if (task.IsCompletedSuccessfully)
            {
                OnGameDeleted.Invoke(this, EventArgs.Empty);
            }
        }

        public void UpdateGuestUser(Action<Task> OnComplete)
        {
            int i;

            for (i = 0; i < PlayersIds.Length; i++)
            {
                if (PlayersIds[i] == fbd.UserId)
                {
                    return;
                }
            }

            PlayersNames[CurrentNumOfPlayers] = MyName;
            PlayersIds[CurrentNumOfPlayers] = fbd.UserId;

            CurrentNumOfPlayers++;

            if (CurrentNumOfPlayers == MaxNumOfPlayers)
            {
                IsFull = true;
            }

            UpdateFireBaseJoinGame(OnComplete);
        }

        private void UpdateFireBaseJoinGame(Action<Task> OnComplete)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add(nameof(PlayersNames), PlayersNames);
            dict.Add(nameof(PlayersIds), PlayersIds);
            dict.Add(nameof(IsFull), IsFull);
            dict.Add(nameof(CurrentNumOfPlayers), CurrentNumOfPlayers);

            fbd.UpdateFields(Keys.GamesCollection, Id, dict, OnComplete);
        }

        public override void DeleteDocument(Action<Task> OnComplete)
        {
            fbd.DeleteDocument(Keys.GamesCollection, Id, OnComplete);
        }

        private void OnChange(IDocumentSnapshot snapshot, Exception error)
        {
            Game updatedGame = snapshot.ToObject<Game>();

            if (updatedGame != null)
            {
                // Sync players metadata (names/ids) and add missing players when someone joins
                PlayersNames = updatedGame.PlayersNames;
                PlayersIds = updatedGame.PlayersIds;
                SyncPlayersFromMetadata();

                if (Players.Count == MaxNumOfPlayers && CurrentPlayerIndex != updatedGame.CurrentPlayerIndex)
                {
                    int previous = CurrentPlayerIndex;

                    CurrentPlayerIndex = updatedGame.CurrentPlayerIndex;

                    if (previous >= 0 && previous < Players.Count)
                    {
                        Players[previous].IsCurrentTurn = false;
                    }
                    if (CurrentPlayerIndex >= 0 && CurrentPlayerIndex < Players.Count)
                    {
                        Players[CurrentPlayerIndex].IsCurrentTurn = true;
                    }
                }

                IsFull = updatedGame.IsFull;

                MainThread.BeginInvokeOnMainThread(() => OnGameChanged?.Invoke(this, EventArgs.Empty));
            }
            else
            {
                MainThread.InvokeOnMainThreadAsync(() =>
                {
                    Shell.Current.Navigation.PopAsync();
                    Toast.Make(Strings.GameDeleted, ToastDuration.Long).Show();
                });
            }
        }

        private Card GetCardFromDeck()
        {
            EnsureDeckInitialized();
            if (Deck.Any())
            {
                Card card = Deck.First();
                Deck.RemoveAt(0);
                return card;
            }
            return null;
        }

        private async Task AddSubDocumentAsync(string parentCollection, string parentId, string subCollection, Dictionary<string, object> data)
        {
            await CrossCloudFirestore.Current.Instance
                .Collection(parentCollection)
                .Document(parentId)
                .Collection(subCollection)
                .AddAsync(data);
        }

        private async Task HandleIncorrectAsk(string playerIdWhoFailed)
        {
            Card newCard = GetCardFromDeck();

            if (newCard != null)
            {
                await SendCardToPlayer(newCard, playerIdWhoFailed);
            }

            NextTurn();
            MainThread.BeginInvokeOnMainThread(() => OnGameChanged?.Invoke(this, EventArgs.Empty));
        }

        public async Task AskForCard(Player asking, string targetId, int value)
        {
            if (asking.Id != fbd.UserId || !asking.IsCurrentTurn) return;

            Dictionary<string, object> request = new Dictionary<string, object>
            {
                { "Type", "AskForValue" },
                { "From", asking.Id },
                { "To", targetId },
                { "Value", value },
                { "TimeStamp", DateTime.UtcNow }
            };

            await AddSubDocumentAsync(Keys.GamesCollection, Id, "Requests", request);
        }

        public async Task AskForShape(Player asking, string targetId, int value, CardModel.Shapes shape)
        {
            if (asking.Id != fbd.UserId || !asking.IsCurrentTurn) return;

            Dictionary<string, object> request = new Dictionary<string, object>
            {
                { "Type", "AskForShape" },
                { "From", asking.Id },
                { "To", targetId },
                { "Value", value },
                { "Shape", shape.ToString() },
                { "TimeStamp", DateTime.UtcNow }
            };

            await AddSubDocumentAsync(Keys.GamesCollection, Id, "Requests", request);
        }

        private async Task SendCardToPlayer(Card card, string playerId)
        {
            Dictionary<string, object> payload = new Dictionary<string, object>
            {
                { "Type", "CardTransfer" },
                { "Card", new Dictionary<string, object> { { nameof(Card.Value), card.Value }, { nameof(Card.Shape), card.Shape.ToString() } } },
                { "To", playerId },
                { "TimeStamp", DateTime.UtcNow }
            };

            await AddSubDocumentAsync(Keys.GamesCollection, Id, "Requests", payload);
            await Task.Delay(100);
        }

        private async void OnRequest(IQuerySnapshot snapshot, Exception error)
        {
            if (error != null || !snapshot.Documents.Any())
            {
                return;
            }

            List<IDocumentSnapshot> documentsToHandle = snapshot.Documents
                .Where((IDocumentSnapshot d) =>
                {
                    if (d.ToObject<Dictionary<string, object>>().TryGetValue("To", out object toValue))
                    {
                        return toValue.ToString() == fbd.UserId;
                    }
                    return false;
                })
                .OrderBy((IDocumentSnapshot d) => d.ToObject<Dictionary<string, object>>().TryGetValue("TimeStamp", out object ts) ? ts : DateTime.MinValue)
                .ToList();

            foreach (IDocumentSnapshot document in documentsToHandle)
            {
                Dictionary<string, object> request = document.ToObject<Dictionary<string, object>>();
                string type = request["Type"].ToString();
                string fromId = request["From"].ToString();

                if (type == "AskForValue" && request.ContainsKey("Value"))
                {
                    int value = int.Parse(request["Value"].ToString());
                    // === תיקון 1: שימוש ב-HandObservable ===
                    bool hasCard = CurrentPlayer.HandObservable.Any((Card c) => c.Value == value);

                    if (!hasCard)
                    {
                        await HandleIncorrectAsk(fromId);
                    }
                }

                else if (type == "AskForShape" && request.ContainsKey("Shape") && request.ContainsKey("Value"))
                {
                    int value = int.Parse(request["Value"].ToString());
                    CardModel.Shapes shape;
                    Enum.TryParse<CardModel.Shapes>(request["Shape"].ToString(), out shape);

                    // === תיקון 2: שימוש ב-HandObservable ===
                    Card found = CurrentPlayer.HandObservable.FirstOrDefault((Card c) => c.Value == value && c.Shape == shape);

                    if (found != null)
                    {
                        // === תיקון 3: שימוש ב-HandObservable ===
                        MainThread.BeginInvokeOnMainThread(() => CurrentPlayer.HandObservable.Remove(found));
                        await SendCardToPlayer(found, fromId);
                        MainThread.BeginInvokeOnMainThread(() => OnGameChanged?.Invoke(this, EventArgs.Empty));
                    }
                    else
                    {
                        await HandleIncorrectAsk(fromId);
                    }
                }

                else if (type == "CardTransfer" && request.ContainsKey("Card"))
                {
                    Dictionary<string, object> cardDict = (Dictionary<string, object>)request["Card"];
                    CardModel.Shapes shape;
                    Enum.TryParse<CardModel.Shapes>(cardDict["Shape"].ToString(), out shape);
                    int cardValue = int.Parse(cardDict["Value"].ToString());

                    Card receivedCard = new Card(shape, cardValue);

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        // === תיקון 4: שימוש ב-HandObservable ===
                        CurrentPlayer.HandObservable.Add(receivedCard);
                        OnGameChanged?.Invoke(this, EventArgs.Empty);
                    });
                }

                await document.Reference.DeleteAsync();
            }
        }

        private void EnsureDeckInitialized()
        {
            if (Deck.Any() || Players == null)
            {
                return;
            }

            List<Card> full = new List<Card>();
            foreach (CardModel.Shapes shape in Enum.GetValues(typeof(CardModel.Shapes)))
            {
                for (int value = 1; value <= Card.CardsInShape; value++)
                {
                    full.Add(new Card(shape, value));
                }
            }

            foreach (Player player in Players)
            {
                foreach (Card card in player.HandObservable.ToList())
                {
                    Card? match = full.FirstOrDefault(c => c.Shape == card.Shape && c.Value == card.Value);
                    if (match != null)
                    {
                        full.Remove(match);
                    }
                }
            }

            foreach (Card card in full)
            {
                Deck.Add(card);
            }
        }

        private void SyncPlayersFromMetadata()
        {
            if (PlayersNames == null || PlayersIds == null)
            {
                return;
            }

            // Add any new players that are not yet in the Players collection
            for (int i = 0; i < PlayersNames.Length; i++)
            {
                string name = PlayersNames[i];
                string id = PlayersIds.Length > i ? PlayersIds[i] : string.Empty;

                if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(id))
                {
                    continue;
                }

                bool exists = Players.Any(p => p.Id == id);
                if (!exists)
                {
                    Player newPlayer = new Player(name, id);
                    Players.Add(newPlayer);

                    if (id == fbd.UserId)
                    {
                        CurrentPlayer = newPlayer;
                    }
                    else
                    {
                        // prevent duplicates in OtherPlayers
                        if (!OtherPlayers.Any(op => op.Id == id))
                        {
                            OtherPlayers.Add(new PlayerVM(newPlayer, false, null));
                        }
                    }
                }
            }

            // Update OtherPlayers list if needed (host needs to see new joiners)
            foreach (Player p in Players)
            {
                if (p.Id != CurrentPlayer?.Id && !OtherPlayers.Any(op => op.Id == p.Id))
                {
                    OtherPlayers.Add(new PlayerVM(p, false, null));
                }
            }
        }

        public override void NextTurn()
        {
            if (Players.Count == 0) return;

            int current = CurrentPlayerIndex;
            Players[current].IsCurrentTurn = false;

            CurrentPlayerIndex = (CurrentPlayerIndex + 1) % Players.Count;

            int next = CurrentPlayerIndex;
            Players[next].IsCurrentTurn = true;

            Dictionary<string, object> dict = new Dictionary<string, object>
            {
                { nameof(CurrentPlayerIndex), CurrentPlayerIndex }
            };

            fbd.UpdateFields(Keys.GamesCollection, Id, dict, task =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    MainThread.BeginInvokeOnMainThread(() => OnGameChanged?.Invoke(this, EventArgs.Empty));
                }
            });
        }
    }
}