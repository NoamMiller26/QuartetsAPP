using System;
using Firebase.Auth;
using Firebase.Auth.Providers;
using Microsoft.Maui.Storage;
using Plugin.CloudFirestore;
using Quartets.Models;

namespace Quartets.ModelLogic
{
    public class FBData : FBDataModel
    {
        public override async void CreateUserWithEmailAndPasswordAsync(string email, string password, string name, Action<System.Threading.Tasks.Task> OnComplete)
        {
            await facl.CreateUserWithEmailAndPasswordAsync(email, password, name).ContinueWith(OnComplete);
        }
        public override async void SignInWithEmailAndPasswordAsync(string email, string password, Action<System.Threading.Tasks.Task> OnComplete)
        {
            await facl.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(OnComplete);
        }
        public override string SetDocument(object obj, string collectonName, string id, Action<System.Threading.Tasks.Task> OnComplete)
        {
            IDocumentReference dr = string.IsNullOrEmpty(id) ? fs.Collection(collectonName).Document() : fs.Collection(collectonName).Document(id);
            dr.SetAsync(obj).ContinueWith(OnComplete);
            return dr.Id;
        }
        public override IListenerRegistration AddSnapshotListener(string collectonName, Plugin.CloudFirestore.QuerySnapshotHandler OnChange)
        {
            ICollectionReference cr = fs.Collection(collectonName);
            return cr.AddSnapshotListener(OnChange);
        }
        public override IListenerRegistration AddSnapshotListener(string collectonName, string id, Plugin.CloudFirestore.DocumentSnapshotHandler OnChange)
        {
            IDocumentReference cr = fs.Collection(collectonName).Document(id);
            return cr.AddSnapshotListener(OnChange);
        }
        public async void GetDocumentsWhereEqualTo(string collectonName, string fName, object fValue, Action<IQuerySnapshot> OnComplete)
        {
            ICollectionReference cr = fs.Collection(collectonName);
            IQuerySnapshot qs = await cr.WhereEqualsTo(fName, fValue).GetAsync();
            OnComplete(qs);
        }
        public override async void UpdateFields(string collectonName, string id, Dictionary<string, object> dict, Action<Task> OnComplete)
        {
            IDocumentReference dr = fs.Collection(collectonName).Document(id);
            await dr.UpdateAsync(dict).ContinueWith(OnComplete);
        }
        public override async void DeleteDocument(string collectonName, string id, Action<Task> OnComplete)
        {
            try
            {
                if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(collectonName))
                {
                    OnComplete?.Invoke(Task.CompletedTask);
                    return;
                }
                
                IDocumentReference dr = fs.Collection(collectonName).Document(id);
                await dr.DeleteAsync().ContinueWith(OnComplete);
            }
            catch (Exception ex)
            {
                // If deletion fails, still call OnComplete to avoid hanging
                System.Diagnostics.Debug.WriteLine($"Error deleting document: {ex.Message}");
                OnComplete?.Invoke(Task.FromException(ex));
            }
        }
        public override string DisplayName
        {
            get
            {
                string dn = string.Empty;
                if (facl.User != null)
                    dn = facl.User.Info.DisplayName;
                return dn;
            }
        }
        public override string UserId
        {
            get
            {
                // Prefer Firebase user ID when signed in with email/password
                if (facl?.User != null && !string.IsNullOrWhiteSpace(facl.User.Uid))
                {
                    return facl.User.Uid;
                }

                // Fallback: GitHub login flow stores a stable ID in preferences.
                // It is stored as a long (Int64), so read it as long and convert to string.
                long gitHubIdLong = Preferences.Get(Keys.GitHubUserIdKey, 0L);
                if (gitHubIdLong != 0L)
                {
                    return gitHubIdLong.ToString();
                }

                // As a last resort, return empty string instead of throwing
                return string.Empty;
            }
        }
    }
}


