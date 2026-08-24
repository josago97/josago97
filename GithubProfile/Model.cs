using System.ClientModel;
using OpenAI;
using OpenAI.Chat;

namespace GithubProfile;

internal class Model
{
    private const string BASE_URL = "https://openrouter.ai/api/v1/";
    private const string MODEL = "nemotron-3-ultra-550b-a55b:free";

    private ChatClient _chatClient;

    public Model()
    {
        _chatClient = new ChatClient(MODEL,
            new ApiKeyCredential(Constants.OpenRouterApiKey),
            new OpenAIClientOptions()
            {
                Endpoint = new Uri(BASE_URL),
                NetworkTimeout = TimeSpan.FromMinutes(10)
            }
        );
    }

    public async Task<string> SendMessageAsync(string message)
    {
        ChatCompletion response = await _chatClient.CompleteChatAsync(message);
        string responseText = response.Content[0].Text;

        return responseText;
    }
}
