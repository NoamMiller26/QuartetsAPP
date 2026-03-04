using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Quartets.Models;

namespace Quartets.Platforms.Android
{
    public class DeleteFBDocsService : Service
    {
        #region Fields

        private bool isRunning = true;

        #endregion

        #region Lifecycle

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent? intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            ThreadStart threadStart = new(DeleteFBDocs);
            Thread thread = new(threadStart);
            thread.Start();
            return base.OnStartCommand(intent, flags, startId);
        }

        public override void OnDestroy()
        {
            isRunning = false;
            base.OnDestroy();
        }

        public override IBinder? OnBind(Intent? intent)
        {
            //not used
            return null;
        }

        #endregion

        #region Private Methods

        private void DeleteFBDocs()
        {
            while (isRunning)
            {
                Thread.Sleep(Keys.OneHourInMilliseconds);
            }
            StopSelf();
        }

        #endregion
    }
}
