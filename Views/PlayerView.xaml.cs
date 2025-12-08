using System;
using Microsoft.Maui.Controls;
using Quartets.ViewModels;

namespace Quartets.Views
{
    public partial class PlayerView : ContentView
    {
        public PlayerView()
        {
            InitializeComponent();
        }

        // מאפשר לקבוע את BindingContext מחוץ ל-View
        public PlayerVM VM
        {
            get => BindingContext as PlayerVM;
            set => BindingContext = value;
        }
    }
}
