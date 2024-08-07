
using System.Net.Http.Headers;
using System.Text;
using Google.Apis.Auth.OAuth2;
using Newtonsoft.Json;

namespace StoreSp.Stores;

public class FcmService
{
    private readonly HttpClient _httpClient;
    private readonly string _accessToken;
    private readonly string _projectId;

    public FcmService(string projectId , string credentialPath)
    {
        _httpClient = new HttpClient();
        _projectId = projectId; // Replace with your Firebase project ID
        _accessToken = GetAccessTokenAsync(credentialPath).Result; // Get the access token
    }

    private async Task<string> GetAccessTokenAsync(string credentialPath)
    {
        var credential = GoogleCredential.FromFile(credentialPath)
                                        .CreateScoped("https://www.googleapis.com/auth/cloud-platform");
        var accessToken = await credential.UnderlyingCredential.GetAccessTokenForRequestAsync();
        return accessToken;
    }

    public async Task SendNotificationAsync(string fcmToken, string title, string body)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"https://fcm.googleapis.com/v1/projects/{_projectId}/messages:send");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

        var payload = new
        {
            message = new
            {
                token = fcmToken,
                notification = new
                {
                    title = title,
                    body = body
                }
            }
        };

        request.Content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

        await _httpClient.SendAsync(request);
    }
}
