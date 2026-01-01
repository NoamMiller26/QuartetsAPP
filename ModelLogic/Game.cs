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

                bool wasFull = IsFull;
                IsFull = updatedGame.IsFull;
                CurrentNumOfPlayers = updatedGame.CurrentNumOfPlayers;

                // If game just became full and we're the host, initialize the starting player to the last one
                // Also handle case where game is already full but CurrentPlayerIndex hasn't been initialized
                if (IsFull && IsHostUser && Players.Count == MaxNumOfPlayers)
                {
                    // Check if we need to initialize the starting player
                    // If CurrentPlayerIndex is 0 and no player has their turn set, initialize it
                    bool anyPlayerHasTurn = Players.Any(p => p.IsCurrentTurn);
                    if ((!wasFull && IsFull) || (CurrentPlayerIndex == 0 && !anyPlayerHasTurn))
                    {
                        // Set starting player to the last player (index = CurrentNumOfPlayers - 1)
                        int lastPlayerIndex = CurrentNumOfPlayers - 1;
                        if (lastPlayerIndex >= 0 && lastPlayerIndex < Players.Count)
                        {
                            CurrentPlayerIndex = lastPlayerIndex;
                            Players[CurrentPlayerIndex].IsCurrentTurn = true;

                            // Update Firebase with the starting player
                            Dictionary<string, object> dict = new Dictionary<string, object>
                            {
                                { nameof(CurrentPlayerIndex), CurrentPlayerIndex }
                            };
                            fbd.UpdateFields(Keys.GamesCollection, Id, dict, task => { });
                        }
                    }
                }
                else if (Players.Count == MaxNumOfPlayers && CurrentPlayerIndex != updatedGame.CurrentPlayerIndex)
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

        private DateTime ExtractTimestamp(object ts)
        {
            if (ts == null) return DateTime.MinValue;

            // Plugin.CloudFirestore.Timestamp supports ToDateTime
            if (ts.GetType().Name == "Timestamp")
            {
                try
                {
                    dynamic dyn = ts;
                    return (DateTime)dyn.ToDateTime();
                }
                catch
                {
                    return DateTime.MinValue;
                }
            }

            if (ts is DateTime dt)
            {
                return dt;
            }

            if (DateTime.TryParse(ts.ToString(), out DateTime parsed))
            {
                return parsed;
            }

            return DateTime.MinValue;
        }

        private void RemoveCardsFromPlayer(Player player, IEnumerable<Card> cards)
        {
            List<Card> list = cards.ToList();
            if (!list.Any())
            {
                return;
            }

            MainThread.BeginInvokeOnMainThread(() =>
            {
                foreach (Card card in list)
                {
                    player.Hand.Remove(card);
                    player.HandObservable.Remove(card);
                }

                OnGameChanged?.Invoke(this, EventArgs.Empty);
            });
        }

        private void AddCardsToPlayer(Player player, IEnumerable<Card> cards)
        {
            List<Card> list = cards.ToList();
            if (!list.Any())
            {
                return;
            }

            MainThread.BeginInvokeOnMainThread(() =>
            {
                foreach (Card card in list)
                {
                    player.Hand.Add(card);
                    player.HandObservable.Add(card);
                }

                OnGameChanged?.Invoke(this, EventArgs.Empty);
            });
        }

        private void CheckAndHandleCompletedSets(Player player, bool notify = true)
        {
            var quartets = player.HandObservable
                .GroupBy(c => c.Value)
                .Where(g => g.Count() == 4)
                .ToList();

            if (!quartets.Any())
            {
                return;
            }

            MainThread.BeginInvokeOnMainThread(() =>
            {
                foreach (var quartet in quartets)
                {
                    foreach (Card card in quartet.ToList())
                    {
                        player.Hand.Remove(card);
                        player.HandObservable.Remove(card);
                    }
                }

                player.AddCompletedSets(quartets.Count);
                OnGameChanged?.Invoke(this, EventArgs.Empty);

                if (notify)
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                        await Toast.Make($"השלמת {quartets.Count} רביעייה!", ToastDuration.Short, 14).Show());
                }
            });
        }

        private async Task HandleIncorrectAsk(string playerIdWhoFailed)
        {
            Card newCard = GetCardFromDeck();

            if (newCard != null)
            {
                await SendCardsToPlayer(new List<Card> { newCard }, playerIdWhoFailed, "AskFail");
            }

            NextTurn();
            MainThread.BeginInvokeOnMainThread(() => OnGameChanged?.Invoke(this, EventArgs.Empty));
        }

        public async Task<bool> AskForCard(Player asking, string targetId, int value)
        {
            if (asking.Id != fbd.UserId || !asking.IsCurrentTurn)
            {
                return false;
            }

            bool hasRankInHand = asking.HandObservable.Any(c => c.Value == value);
            if (!hasRankInHand)
            {
                return false;
            }

            Dictionary<string, object> request = new Dictionary<string, object>
            {
                { "Type", "AskForValue" },
                { "From", asking.Id },
                { "To", targetId },
                { "Value", value },
                { "TimeStamp", DateTime.UtcNow }
            };

            await AddSubDocumentAsync(Keys.GamesCollection, Id, "Requests", request);
            return true;
        }

        private async Task SendCardsToPlayer(IEnumerable<Card> cards, string playerId, string reason = null)
        {
            List<Dictionary<string, object>> cardsPayload = cards
                .Select(c => new Dictionary<string, object>
                {
                    { nameof(Card.Value), c.Value },
                    { nameof(Card.Shape), c.Shape.ToString() }
                })
                .ToList();

            if (!cardsPayload.Any())
            {
                return;
            }

            Dictionary<string, object> payload = new Dictionary<string, object>
            {
                { "Type", "CardTransfer" },
                { "Cards", cardsPayload },
                { "To", playerId },
                { "TimeStamp", DateTime.UtcNow }
            };
            if (!string.IsNullOrWhiteSpace(reason))
            {
                payload["Reason"] = reason;
            }

            await AddSubDocumentAsync(Keys.GamesCollection, Id, "Requests", payload);
            await Task.Delay(100);
        }

        private async void OnRequest(IQuerySnapshot snapshot, Exception error)
        {
            if (error != null || !snapshot.Documents.Any())
            {
                return;
            }

            if (CurrentPlayer == null)
            {
                return;
            }

            // Process only newly added documents to avoid re-processing and duplicates
            var changes = snapshot.DocumentChanges?
                .Where(c => c.Type == DocumentChangeType.Added)
                .Select(c => c.Document)
                .ToList() ?? new List<IDocumentSnapshot>();

            if (!changes.Any())
            {
                return;
            }

            // Materialize to plain objects so we never touch disposed snapshot objects after awaits
            var documentsToHandle = changes
                .Select(d =>
                {
                    var data = d.ToObject<Dictionary<string, object>>();
                    if (data == null) return null;

                    data.TryGetValue("To", out object toObj);
                    data.TryGetValue("From", out object fromObj);
                    data.TryGetValue("Type", out object typeObj);
                    data.TryGetValue("Value", out object valueObj);
                    data.TryGetValue("Card", out object cardObj);
                    data.TryGetValue("Cards", out object cardsObj);
                    data.TryGetValue("TimeStamp", out object tsObj);
                    data.TryGetValue("Reason", out object reasonObj);

                    return new
                    {
                        DocId = d.Id,
                        To = toObj?.ToString(),
                        From = fromObj?.ToString(),
                        Type = typeObj?.ToString(),
                        ValueObj = valueObj,
                        CardObj = cardObj,
                        CardsObj = cardsObj,
                        SortKey = ExtractTimestamp(tsObj),
                        Reason = reasonObj?.ToString()
                    };
                })
                .Where(x => x != null && x.To == fbd.UserId && !string.IsNullOrWhiteSpace(x.Type))
                .OrderBy(x => x.SortKey)
                .ToList();

            foreach (var entry in documentsToHandle)
            {
                IDocumentReference docRef = CrossCloudFirestore.Current.Instance
                    .Collection(Keys.GamesCollection)
                    .Document(Id)
                    .Collection("Requests")
                    .Document(entry.DocId);

                try
                {
                    if (entry.Type == "AskForValue" && entry.ValueObj != null && !string.IsNullOrWhiteSpace(entry.From))
                    {
                        int value = int.Parse(entry.ValueObj.ToString());
                        var matchingCards = CurrentPlayer.HandObservable.Where(c => c.Value == value).ToList();

                        if (matchingCards.Any())
                        {
                            RemoveCardsFromPlayer(CurrentPlayer, matchingCards);
                            await SendCardsToPlayer(matchingCards, entry.From, "AskSuccess");
                            CheckAndHandleCompletedSets(CurrentPlayer, notify: false);
                        }
                        else
                        {
                            await HandleIncorrectAsk(entry.From);
                        }
                    }
                    else if (entry.Type == "CardTransfer" && entry.CardObj is Dictionary<string, object> singleCard)
                    {
                        if (singleCard.TryGetValue("Shape", out object shapeObj) &&
                            singleCard.TryGetValue("Value", out object valObj))
                        {
                            CardModel.Shapes shape;
                            if (Enum.TryParse<CardModel.Shapes>(shapeObj.ToString(), out shape))
                            {
                                int cardValue = int.Parse(valObj.ToString());
                                AddCardsToPlayer(CurrentPlayer, new List<Card> { new Card(shape, cardValue) });
                                CheckAndHandleCompletedSets(CurrentPlayer);
                            }
                        }
                    }
                    else if (entry.Type == "CardTransfer" && entry.CardsObj is IEnumerable<object> cardList)
                    {
                        var cardsToAdd = new List<Card>();
                        foreach (object obj in cardList)
                        {
                            if (obj is Dictionary<string, object> dict &&
                                dict.TryGetValue("Shape", out object shapeObj) &&
                                dict.TryGetValue("Value", out object valObj))
                            {
                                CardModel.Shapes shape;
                                if (Enum.TryParse<CardModel.Shapes>(shapeObj.ToString(), out shape))
                                {
                                    int cardValue = int.Parse(valObj.ToString());
                                    cardsToAdd.Add(new Card(shape, cardValue));
                                }
                            }
                        }

                        AddCardsToPlayer(CurrentPlayer, cardsToAdd);
                        CheckAndHandleCompletedSets(CurrentPlayer);

                        // Notify receiver about result if present
                        string reason = entry.Reason;
                        if (!string.IsNullOrWhiteSpace(reason))
                        {
                            if (reason == "AskSuccess")
                            {
                                MainThread.BeginInvokeOnMainThread(async () =>
                                    await Toast.Make("קיבלת את כל הקלפים שביקשת", ToastDuration.Short, 14).Show());
                            }
                            else if (reason == "AskFail")
                            {
                                MainThread.BeginInvokeOnMainThread(async () =>
                                    await Toast.Make("לא היו קלפים, שלפת קלף מהקופה", ToastDuration.Short, 14).Show());
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    // Ignore individual failures to keep processing other requests
                }
                finally
                {
                    try
                    {
                        await docRef.DeleteAsync();
                    }
                    catch
                    {
                        // best effort
                    }
                }
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