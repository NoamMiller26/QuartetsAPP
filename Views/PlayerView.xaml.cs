using Quartets.ModelLogic;
using Quartets.ViewModels;

namespace Quartets.Views;

public partial class PlayerView : ContentView
{
    public PlayerView()
    {
        InitializeComponent();
    }
    public static readonly BindableProperty PlayerProperty =
        BindableProperty.Create(
            nameof(Player),
            typeof(PlayerVM),
            typeof(PlayerView),
            propertyChanged: OnPlayerChanged);

    public PlayerVM Player
    {
        get => (PlayerVM)GetValue(PlayerProperty);
        set => SetValue(PlayerProperty, value);
    }

    private static void OnPlayerChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (PlayerView)bindable;
        if (newValue == null)
        {
            return;
        }
        control.BindingContext = newValue;  // 👈 This binds the WHOLE view to the Player object
    }
}
