using Quartets.ModelLogic;

namespace Quartets.Views
{
    public partial class PlayerView : ContentView
    {
        public PlayerView()
        {
            InitializeComponent();
        }

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
            var control = (PlayerView)bindable;
            if (newValue != null)
            {
                control.BindingContext = newValue;  // זה יעדכן את כל ה־View ל־Player החדש
            }
        }
    }
}
