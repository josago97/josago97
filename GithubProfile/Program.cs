using System.Globalization;
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
        string today = DateOnly.FromDateTime(DateTime.Now)
            .ToString("d 'de' MMMM", new CultureInfo("es-ES"));

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
        string request = $"""
            En dos líneas dime solamente un hecho histórico sobre la programación y el software que haya sucedido en un día como {today}.
            Dame solamente el hecho, sin saludos ni despedidas.
        """;
        string historicalEvent = await _deepseek.SendMessageAsync(request);

        return historicalEvent;
    }

    private static async Task<Song> GetSongAsync(string today)
    {
        string request = $"""
            Elige un single hit clásico de los 80s, 90s o 00s que se haya publicado en un {today}. 
            Me vas a dar como respuesta un json que incluye varios campos.
            Un campo llamado author que tenga el nombre del artista. 
            Otro campo llamado title que tenga el nombre de la canción.
            El último campo llamado description en el cual hablas en castellano sobre la canción en dos líneas.
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
        string authorAndTitle = $"{song.Author} - {song.Title}";
        string youtubeLink = await youtube.GetVideoUrlAsync(authorAndTitle);
        stringBuilder.AppendLine($"#### [{authorAndTitle}]({youtubeLink})");
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

    private record Song(string Title, string Author, string Description);
}
