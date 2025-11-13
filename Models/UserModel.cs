using Quartets.ModelLogic;

namespace Quartets.Models
{
    internal abstract class UserModels
    {
        protected FBData fbd = new();
        public EventHandler? OnAuthCompleted;
        
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Age { get; set; } = string.Empty;
        public abstract void Register();
        public abstract void Login();
        public abstract bool CanLogin();
        public abstract bool CanRegister();
        public abstract string GetFirebaseErrorMessage(string msg);
       
        public bool IsRegistered => (!string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(Password) && !string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Age));
    }
}
