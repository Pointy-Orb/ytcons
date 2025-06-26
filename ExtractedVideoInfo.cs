using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Net;
using Newtonsoft.Json;

namespace YTCons;

public class ExtractedVideoInfo
{
    private static string bestSite = "pipedapi.reallyaweso.me";
    private const string instances = "https://raw.githubusercontent.com/TeamPiped/documentation/refs/heads/main/content/docs/public-instances/index.md";
    public static readonly List<string> sites = new();
    private List<VideoStream> validStreams = new();
    private const string searchFormat = "/streams/";
    private HttpClient client = new HttpClient();

    public Video video = new Video();
    public bool gotVideo = false;
    public bool windowOpen = false;
    public bool playing { get; private set; }
    public Process mediaPlayer = new();

    internal static bool gotSites = false;

    private static void SetBestSite(string site)
    {
        bestSite = site;
        File.WriteAllText(Path.Combine(Globals.localDir, "bestSite"), bestSite);
    }

    public static async Task GetSites()
    {
        if (File.Exists(Path.Combine(Globals.localDir, "bestSite")))
        {
            bestSite = File.ReadAllText(Path.Combine(Globals.localDir, "bestSite"));
        }
        var url = new Uri(instances);
        string instanceList = "";
        using (var staticClient = new HttpClient())
        {
            HttpResponseMessage response = await staticClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error getting instances: {response.StatusCode} ({(int)response.StatusCode})");
                Globals.Exit((int)response.StatusCode);
            }
            instanceList = await response.Content.ReadAsStringAsync();
        }
        var rawSites = Regex.Matches(instanceList, @"\| https://[^ ]+");
        foreach (Match rawSite in rawSites)
        {
            sites.Add(rawSite.ToString().Replace("| ", ""));
            if (Globals.debug)
            {
                Console.WriteLine(rawSite);
            }
        }
        if (sites.Count > 0)
        {
            if (!sites.Contains(bestSite))
            {
                bestSite = sites[0];
            }
            gotSites = true;
        }
        else
        {
            Console.WriteLine($"Fatal error: site list was null.");
            Globals.Exit(1);
        }
    }

    public ExtractedVideoInfo(string id)
    {
        Task.Run(() => Init(id)).ConfigureAwait(false);
    }

    async Task Init(string id)
    {
        LoadBar.loadMessage = "Starting process";
        video = await Main(id);
        gotVideo = true;
    }

    async Task<string?> GetJsonInner(Uri url, CancellationToken cancellationToken, string site)
    {
        LoadBar.loadMessage = "Finding valid API";
        var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        var combinedCancel = CancellationTokenSource.CreateLinkedTokenSource(timeout.Token, cancellationToken);
        HttpResponseMessage response = await client.GetAsync(url, combinedCancel.Token);
        if (Globals.debug)
        {
            Console.WriteLine($"{site} got a response");
        }
        if (!response.IsSuccessStatusCode)
        {
            if (Globals.debug)
            {
                Console.SetCursorPosition(0, sites.IndexOf(site));
                Console.Write($"{site} failed with error code {(int)response.StatusCode} ({response.StatusCode})");
            }
            codes.Add(response.StatusCode);
            return null;
        }
        if (Globals.debug)
        {
            Console.SetCursorPosition(0, sites.IndexOf(site));
            Console.Write($"{site}: successfully got data");
        }
        LoadBar.loadMessage = "Getting raw data";
        var str = await response.Content.ReadAsStringAsync();
        return str;
    }
    bool success = false;

    List<HttpStatusCode> codes = new();

    private bool TryWithBestSite(string id, CancellationToken cancel, out Video value)
    {
        value = new Video();
        if (!sites.Contains(bestSite))
        {
            bestSite = sites[0];
        }
        var url = new Uri($"{bestSite}{searchFormat}{id}");
        var getJson = GetJsonInner(url, cancel, bestSite);
        string? json = Task.Run(() => getJson).Result;
        if (json == null)
        {
            return false;
        }
        try
        {
            LoadBar.loadMessage = "Parsing data";
            value = JsonConvert.DeserializeObject<Video>(json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception: " + ex.Message);
            throw;
        }
        if (value == null)
        {
            return false;
        }
        LoadBar.loadMessage = "Checking video streams";
        foreach (VideoStream stream in value.videoStreams)
        {
            if (ValidateStream(stream))
            {
                validStreams.Add(stream);
            }
        }
        if (validStreams.Count > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    async Task<Video> Main(string id)
    {
        Dictionary<Task, string> taskWithSite = new();
        Video tempVideo;
        string? json = null;
        var cancel = new CancellationTokenSource();
        if (TryWithBestSite(id, cancel.Token, out var value))
        {
            return value;
        }
        List<Task<string?>> tasks = new();
        foreach (string site in sites)
        {
            var url = new Uri($"{site}{searchFormat}{id}");
            tasks.Add(GetJsonInner(url, cancel.Token, site));
            taskWithSite.Add(tasks[tasks.Count - 1], site);
        }
        while (tasks.Count > 0 && !success)
        {
            validStreams.Clear();
            if (Globals.debug)
            {
                Console.Write(tasks.Count);
            }
            var fin = await Task.WhenAny(tasks);
            if (Globals.debug)
            {
                Console.SetCursorPosition(0, 20 + tasks.Count);
            }
            string? maybe = null;
            try
            {
                maybe = await fin;
            }
            catch
            {
                if (Globals.debug)
                {
                    Console.WriteLine("Task failed: " + fin);
                }
            }
            var maybeBestSite = taskWithSite[fin];
            tasks.Remove(fin);
            if (maybe != null)
            {
                LoadBar.loadMessage = "Converting to usable format";
                json = maybe;
            }
            else
            {
                continue;
            }
            try
            {
                LoadBar.loadMessage = "Parsing data";
                tempVideo = JsonConvert.DeserializeObject<Video>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                throw;
            }
            LoadBar.loadMessage = "Checking video streams";
            foreach (VideoStream stream in tempVideo.videoStreams)
            {
                if (await ValidateStreamAsync(stream))
                {
                    validStreams.Add(stream);
                }
            }
            if (validStreams.Count > 0)
            {
                cancel.Cancel();
                success = true;
                SetBestSite(maybeBestSite);
                return tempVideo;
            }
            else if (Globals.debug)
            {
                Console.WriteLine("Found API but video streams were invalid");
            }
        }
        if (!success)
        {
            Console.WriteLine($"Usable mirror could not be found.");
            foreach (HttpStatusCode code in codes)
            {
                Console.WriteLine($"{codes.IndexOf(code)}: {code} ({(int)code})");
            }
            Globals.Exit(1);
        }
        return new Video();
    }

    private async Task<bool> ValidateStreamAsync(VideoStream stream)
    {
        using var request = new HttpRequestMessage(HttpMethod.Head, stream.url);
        using var testResponse = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        return testResponse.IsSuccessStatusCode;
    }

    private bool ValidateStream(VideoStream stream)
    {
        using var request = new HttpRequestMessage(HttpMethod.Head, stream.url);
        using var testResponse = client.Send(request, HttpCompletionOption.ResponseHeadersRead);
        return testResponse.IsSuccessStatusCode;
    }

    public void Play(string format = "MPEG_4", string quality = "720p")
    {
        mediaPlayer = new();
        mediaPlayer.StartInfo.FileName = "/usr/bin/mpv";
        VideoStream bestStream = validStreams[0];
        foreach (VideoStream stream in validStreams)
        {
            if (stream.format == format && stream.quality == quality)
            {
                bestStream = stream;
                if (Globals.debug)
                {
                    File.WriteAllText(Path.GetTempPath() + "debugStream.txt", stream.url);
                }
                break;
            }
        }
        mediaPlayer.StartInfo.Arguments = $"\"{bestStream.url}\" --audio-file=\"{video.audioStreams[0].url}\" --title=\"{video.title} | {video.uploader}\"";
        /*
        mediaPlayer.StartInfo.RedirectStandardInput = true;
        mediaPlayer.StartInfo.RedirectStandardOutput = true;
        mediaPlayer.StartInfo.RedirectStandardError = true;
		*/
        playing = true;
        try
        {
            mediaPlayer.Start();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to start media player: " + ex.Message);
        }
    }

    public string ParsedDuration()
    {
        int minutes = video.duration / 60;
        if (minutes < 60)
        {
            return $"{minutes}:{video.duration % 60}";
        }
        else
        {
            return $"{minutes / 60}:{minutes % 60}:{video.duration % 60}";
        }
    }
}
