using System.ClientModel;
using OpenAI;
using OpenAI.Chat;

namespace GithubProfile;

internal class Model
{
    private const string BASE_URL = "https://openrouter.ai/api/v1/";
    private static readonly string[] MODELS = ["nemotron-3-ultra-550b-a55b:free", "gemma-4-31b-it:free", "nvidia/nemotron-3-embed-1b:free"];

    public async Task<string> SendMessageAsync(string message)
    {
        string responseText = null;
        int modelIndex = 0;
        Exception lastException = null;

        do
        {
            ChatClient chatClient = new ChatClient(MODELS[modelIndex],
                new ApiKeyCredential(Constants.OpenRouterApiKey),
                new OpenAIClientOptions()
                {
                    Endpoint = new Uri(BASE_URL),
                    NetworkTimeout = TimeSpan.FromMinutes(10)
                }
            );

            try
            {
                ChatCompletion response = await chatClient.CompleteChatAsync(message);
                responseText = response.Content[0].Text;
            }
            catch (Exception ex)
            {
                lastException = ex;
            }
        } 
        while (string.IsNullOrWhiteSpace(responseText) && modelIndex < MODELS.Length);

        if (lastException is not null)
            throw lastException;

        return responseText;
    }
}
