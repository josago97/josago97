namespace GithubProfile;

public static class Constants
{
    public static string OpenRouterApiKey { get; }

    static Constants()
    {
        OpenRouterApiKey = Environment.GetEnvironmentVariable("OPEN_ROUTER_API_KEY");
    }
}
