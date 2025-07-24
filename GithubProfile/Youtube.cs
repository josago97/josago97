using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Search;

namespace GithubProfile;

internal class Youtube
{
    private YoutubeClient _client;

    public Youtube()
    {
        _client = new YoutubeClient();
    }

    public async Task<string> GetVideoUrlAsync(string query)
    {
        IReadOnlyList<VideoSearchResult> results = await _client.Search.GetVideosAsync(query).CollectAsync();
        VideoSearchResult result = results[0];

        return result.Url;
    }
}
