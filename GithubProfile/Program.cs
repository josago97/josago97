using System.Text;
using System.Text.Json;

namespace GithubProfile;

internal class Program
{
    private const string README_PATH = "../../../../README.md";
    private const string README_TEMPLATE_PATH = "ReadmeTemplate.md";
    private const string HISTORICAL_EVENT_KEY = "<HISTORICAL_EVENT>";
    private const string SONG_KEY = "<SONG>";

    private static Deepseek _deepseek = new Deepseek();

    static async Task Main(string[] args)
    {
        Directory.SetCurrentDirectory(AppContext.BaseDirectory);
        DotNetEnv.Env.Load();

        string readme = File.ReadAllText(README_TEMPLATE_PATH);
        string today = DateOnly.FromDateTime(DateTime.UtcNow).ToString("dd-MM");

        Task<string> historicalEventTask = GetHistoricalEventAsync(today);
        Task<Song> songTask = GetSongAsync(today);
        await Task.WhenAll(historicalEventTask, songTask);

        string historicalEvent = await historicalEventTask;
        Song song = await songTask;

        readme = readme.Replace(HISTORICAL_EVENT_KEY, historicalEvent);
        readme = await ReplaceSongAsync(readme, song);

        File.WriteAllText(README_PATH, readme);
    }

    private static async Task<string> GetHistoricalEventAsync(string today)
    {
        string request = $"En dos líneas dime un hecho histórico sobre la programación y el software que haya sucedido en un día como {today}";
        string historicalEvent = await _deepseek.SendMessageAsync(request);

        return historicalEvent;
    }

    private static async Task<Song> GetSongAsync(string today)
    {
        string request = $"""
            Elige un single hit clásico de los 80s, 90s o 00s que se haya publicado en un día como {today}. 
            Me vas a dar como respuesta un json que incluye los siguientes campos, un campo llamado title que tiene el nombre del artista y la canción unidos con un '-'. 
            Otro campo llamado description en el cual hablas en castellano sobre la canción en dos líneas.";
        """;

        string response = await _deepseek.SendMessageAsync(request);
        string json = ExtractJson(response);
        Song song = JsonSerializer.Deserialize<Song>(json, JsonSerializerOptions.Web);

        return song;
    }

    private static async Task<string> ReplaceSongAsync(string readme, Song song)
    {
        StringBuilder stringBuilder = new StringBuilder();

        Youtube youtube = new Youtube();
        string youtubeLink = await youtube.GetVideoUrlAsync(song.Title);
        stringBuilder.AppendLine($"#### [{song.Title}]({youtubeLink})");
        stringBuilder.AppendLine(song.Description);

        return readme.Replace(SONG_KEY, stringBuilder.ToString());
    }

    private static string ExtractJson(string message)
    {
        int startIndex = message.IndexOf('{');
        int endIndex = message.LastIndexOf('}');
        string json = message.Substring(startIndex, endIndex - startIndex + 1);

        return json;
    }

    private record Song(string Title, string Description);
}
