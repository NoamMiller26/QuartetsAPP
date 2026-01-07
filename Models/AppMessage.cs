using CommunityToolkit.Mvvm.Messaging.Messages;
namespace Quartets.Models
{
    public class AppMessage<T>(T msg) : ValueChangedMessage<T>(msg)
    {
    }
}
