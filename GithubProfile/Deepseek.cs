using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace GithubProfile;

internal class Deepseek
{
    private const string BASE_URL = "https://openrouter.ai/api/v1/";
    private const string MODEL = "deepseek/deepseek-chat-v3.1:free";

    private readonly HttpClient _httpClient;

    public Deepseek()
    {
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri(BASE_URL);
        _httpClient.Timeout = TimeSpan.FromMinutes(5);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Constants.OpenRouterApiKey);
    }

    public async Task<string> SendMessageAsync(string message)
    {
        var requestBody = new
        {
            model = MODEL,
            messages = new[]
            {
                new { role = "user", content = message }
            }
        };

        string json = JsonSerializer.Serialize(requestBody);
        HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
        HttpResponseMessage response = await _httpClient.PostAsync("chat/completions", content);
        string responseText = await response.Content.ReadAsStringAsync();

        using JsonDocument jsonDocument = JsonDocument.Parse(responseText);
        JsonElement jsonRoot = jsonDocument.RootElement;
        string result = jsonRoot.GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        return result;
    }
}
