using System;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;

namespace Quartets
{
    internal class Program : MauiApplication
    {
        #region Overrides

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        #endregion

        #region Entry Point

        static void Main(string[] args)
        {
            var app = new Program();
            app.Run(args);
        }

        #endregion
    }
}
