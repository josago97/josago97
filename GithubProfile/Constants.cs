namespace GithubProfile;

public static class Constants
{
    public static string OpenRouterApiKey { get; }
    public static string SpotifyClientId { get; }
    public static string SpotifyClientSecret { get; }

    static Constants()
    {
        OpenRouterApiKey = Environment.GetEnvironmentVariable("OPEN_ROUTER_API_KEY");
        SpotifyClientId = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_ID");
        SpotifyClientSecret = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_SECRET");
    }
}
