using System.ClientModel;
using OpenAI;
using OpenAI.Chat;

namespace GithubProfile;

internal class Deepseek
{
    private const string BASE_URL = "https://openrouter.ai/api/v1/";
    private const string MODEL = "deepseek/deepseek-chat-v3.1";

    private ChatClient _chatClient;

    public Deepseek()
    {
        _chatClient = new ChatClient(MODEL,
            new ApiKeyCredential(Constants.OpenRouterApiKey),
            new OpenAIClientOptions()
            {
                Endpoint = new Uri(BASE_URL)
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
