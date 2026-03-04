using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using CommunityToolkit.Mvvm.Messaging;
using Quartets.Models;
using Quartets.Platforms.Android;

namespace Quartets
{
    [Activity(
        Theme = "@style/Maui.SplashTheme",
        MainLauncher = true,
        LaunchMode = LaunchMode.SingleTask, // 🔥 שינוי קריטי
        ConfigurationChanges = ConfigChanges.ScreenSize |
                               ConfigChanges.Orientation |
                               ConfigChanges.UiMode |
                               ConfigChanges.ScreenLayout |
                               ConfigChanges.SmallestScreenSize |
                               ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        #region Fields

        MyTimer? mTimer;

        #endregion

        #region Lifecycle

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RegisterTimerMessages();
            StartDeleteFBDocsService();
        }

        #endregion

        #region Private Methods

        private void StartDeleteFBDocsService()
        {
            Intent = new Intent(this, typeof(DeleteFBDocsService));
            StartService(Intent);
        }

        private void RegisterTimerMessages()
        {
            WeakReferenceMessenger.Default.Register<AppMessage<TimerSettings>>(this, (r, m) =>
            {
                OnMessageReceived(m.Value);
            });

            WeakReferenceMessenger.Default.Register<AppMessage<bool>>(this, (r, m) =>
            {
                OnMessageReceived(m.Value);
            });
        }

        private void OnMessageReceived(bool value)
        {
            if (value)
            {
                mTimer?.Cancel();
                mTimer = null;
            }
        }

        private void OnMessageReceived(TimerSettings value)
        {
            mTimer = new MyTimer(value.TotalTimeInMillSeconds, value.IntervalTimeInMillSeconds);
            mTimer.Start();
        }

        #endregion
    }
}
