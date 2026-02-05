using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Microsoft.Maui.Authentication;
using Quartets.Models;

namespace Quartets.ModelLogic
{
    internal static class GitHubAuthService
    {
        private static readonly HttpClient httpClient = new HttpClient();

        // Redirect URI must also be registered in the GitHub OAuth app and platform-specific callbacks.
        private const string RedirectUri = "com.school.quartets://github-auth-callback";

        public static async Task<GitHubUser?> LoginAsync()
        {
            var clientId = Environment.GetEnvironmentVariable("GITHUB_CLIENT_ID");
            var clientSecret = Environment.GetEnvironmentVariable("GITHUB_CLIENT_SECRET");

            if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret))
            {
                await Toast.Make("GitHub OAuth is not configured (missing CLIENT_ID / CLIENT_SECRET).", ToastDuration.Long, 14).Show();
                return null;
            }

            var state = Guid.NewGuid().ToString("N");

            var authorizeUrl =
                $"https://github.com/login/oauth/authorize?client_id={Uri.EscapeDataString(clientId)}" +
                $"&redirect_uri={Uri.EscapeDataString(RedirectUri)}" +
                $"&scope={Uri.EscapeDataString("read:user")}" +
                $"&state={Uri.EscapeDataString(state)}";

            WebAuthenticatorResult authResult;
            try
            {
                authResult = await WebAuthenticator.AuthenticateAsync(
                    new Uri(authorizeUrl),
                    new Uri(RedirectUri));
            }
            catch (TaskCanceledException)
            {
                // User cancelled the login.
                return null;
            }

            if (!authResult.Properties.TryGetValue("code", out var code) || string.IsNullOrWhiteSpace(code))
            {
                await Toast.Make("GitHub did not return an authorization code.", ToastDuration.Short, 14).Show();
                return null;
            }

            // Exchange authorization code for access token
            var tokenRequest = new HttpRequestMessage(HttpMethod.Post, "https://github.com/login/oauth/access_token");
            var body = new Dictionary<string, string>
            {
                ["client_id"] = clientId,
                ["client_secret"] = clientSecret,
                ["code"] = code,
                ["redirect_uri"] = RedirectUri
            };
            tokenRequest.Content = new FormUrlEncodedContent(body);
            tokenRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var tokenResponse = await httpClient.SendAsync(tokenRequest);
            tokenResponse.EnsureSuccessStatusCode();

            var tokenJson = await tokenResponse.Content.ReadAsStringAsync();
            using var tokenDoc = JsonDocument.Parse(tokenJson);
            if (!tokenDoc.RootElement.TryGetProperty("access_token", out var accessTokenElement))
            {
                await Toast.Make("GitHub did not return an access token.", ToastDuration.Short, 14).Show();
                return null;
            }

            var accessToken = accessTokenElement.GetString();
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                await Toast.Make("GitHub access token is empty.", ToastDuration.Short, 14).Show();
                return null;
            }

            // Fetch basic profile
            var userRequest = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/user");
            userRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            userRequest.Headers.UserAgent.Add(new ProductInfoHeaderValue("QuartetsApp", "1.0"));

            var userResponse = await httpClient.SendAsync(userRequest);
            userResponse.EnsureSuccessStatusCode();

            var userJson = await userResponse.Content.ReadAsStringAsync();
            using var userDoc = JsonDocument.Parse(userJson);

            var root = userDoc.RootElement;
            var id = root.GetProperty("id").GetInt64();
            var login = root.GetProperty("login").GetString() ?? string.Empty;

            return new GitHubUser
            {
                Id = id,
                UserName = login
            };
        }
    }
}


