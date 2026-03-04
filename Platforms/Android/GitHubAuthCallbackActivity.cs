using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Microsoft.Maui.Authentication;

namespace Quartets.Platforms.Android
{
    [Activity(
        NoHistory = true,
        LaunchMode = LaunchMode.SingleTask,
        Exported = true)]
    [IntentFilter(
        new[] { Intent.ActionView },
        Categories = new[]
        {
            Intent.CategoryDefault,
            Intent.CategoryBrowsable
        },
        DataScheme = "com.school.quartets",
        DataHost = "github-auth-callback")]
    public class GitHubAuthCallbackActivity : WebAuthenticatorCallbackActivity
    {
        #region Lifecycle

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        #endregion
    }
}
