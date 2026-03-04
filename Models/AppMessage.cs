using CommunityToolkit.Mvvm.Messaging.Messages;
namespace Quartets.Models
{
    public class AppMessage<T>(T msg) : ValueChangedMessage<T>(msg)
    {
        #region Constructors

        // Primary constructor provided by C# 12 primary constructor syntax.

        #endregion
    }
}
