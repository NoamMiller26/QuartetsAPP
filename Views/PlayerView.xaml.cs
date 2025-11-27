using Quartets.ModelLogic;
using Quartets.ViewModels;

namespace Quartets.Views;

public partial class PlayerView : ContentView
{
    public static readonly BindableProperty PlayerProperty =
        BindableProperty.Create(
            nameof(Player),
            typeof(Player),
            typeof(PlayerView),
            propertyChanged: OnPlayerChanged);

    public Player Player
    {
        get => (Player)GetValue(PlayerProperty);
        set => SetValue(PlayerProperty, value);
    }

    private static void OnPlayerChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (PlayerView)bindable;
        if (newValue is Player player)
            view.BindingContext = new PlayerVM(player);
    }

    public PlayerView()
    {
        InitializeComponent();
    }
}
