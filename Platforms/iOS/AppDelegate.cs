using Foundation;

namespace Quartets
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        #region Overrides

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        #endregion
    }
}
