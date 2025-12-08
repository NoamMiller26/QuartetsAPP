using Microsoft.Maui.Controls;
using Quartets.ViewModels;

namespace Quartets.Views
{
    public partial class GamePage : ContentPage
    {
        private GameVM _vm;
        public GamePage(GameVM vm)
        {
            InitializeComponent();
            _vm = vm;
            BindingContext = _vm;
        }
    }
}
