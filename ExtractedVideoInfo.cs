using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using System.Diagnostics;
using YoutubeDLSharp;
using YoutubeDLSharp.Metadata;
//using System.Net;
using Newtonsoft.Json;

namespace YTCons;

public class ExtractedVideoInfo
{
    private static readonly HttpClient client = new HttpClient
    {
        Timeout = TimeSpan.FromSeconds(10)
    };

    public readonly string id;
    public VideoData video = null!;
    public bool gotVideo = false;
    public bool windowOpen = false;
    public bool playing { get; private set; }
    public Process mediaPlayer = new();
    public List<string> subtitles = new();

    internal static bool gotSites = false;

    /*
    private static void SetBestSite(string site)
    {
        bestSite = site;
        File.WriteAllText(Path.Combine(Dirs.localDir, "bestSite"), bestSite);
    }

    public static async Task GetSites()
    {
        if (File.Exists(Path.Combine(Dirs.localDir, "bestSite")))
        {
            bestSite = File.ReadAllText(Path.Combine(Dirs.localDir, "bestSite"));
        }
        var instancesFile = Path.Combine(Dirs.localDir, "instances.json");
        if (File.Exists(instancesFile))
        {
            if (Globals.debug)
            {
                Console.WriteLine("Reading offline instance list");
            }
            List<string>? offSites = null;
            try
            {
                offSites = JsonConvert.DeserializeObject<List<string>>(await File.ReadAllTextAsync(instancesFile));
            }
            catch
            {
                if (Globals.debug)
                {
                    Console.WriteLine("Error readon offline site list.");
                }
            }
            if (offSites == null)
            {
                if (Globals.debug)
                {
                    Console.WriteLine("Offline instance list was null, referring to online list.");
                }
                await GetSitesOnline();
                await File.WriteAllTextAsync(instancesFile, JsonConvert.SerializeObject(sites));
            }
            else
            {
                sites = (List<string>)offSites;
                if (!sites.Contains(bestSite))
                {
                    bestSite = sites[0];
                }
                gotSites = true;
                gotSitesOffline = true;
            }
        }
        else
        {
            if (Globals.debug)
            {
                Console.WriteLine("Offline instance list not found, referring to online list.");
            }
            await GetSitesOnline();
            await File.WriteAllTextAsync(instancesFile, JsonConvert.SerializeObject(sites));
        }
    }

    public static async Task GetSitesOnline()
    {
        var url = new Uri(instances);
        string instanceList = "";
        using (var staticClient = new HttpClient())
        {
            using HttpResponseMessage response = await staticClient.GetAsync(url);
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
            Console.WriteLine($"Fatal error: site list was null. Did the online instance list change location?");
            Globals.Exit(1);
        }
    }
    */

    public static async Task<ExtractedVideoInfo> CreateAsync(string id)
    {
        var instance = new ExtractedVideoInfo(id);
        await instance.Init();
        instance.gotVideo = true;
        return instance;
    }

    public static ExtractedVideoInfo Empty
    {
        get { return new ExtractedVideoInfo(""); }
    }

    private ExtractedVideoInfo(string id)
    {
        this.id = id;
    }

    async Task Init()
    {
        if (!File.Exists(Dirs.VideoInfoJson(id)))
        {
            var ytdlp = YtdlpFactory(id);
            ytdlp.StartInfo.Arguments += " --write-info-json";
            ytdlp.StartInfo.Arguments += " --skip-download";
            ytdlp.Start();
            await ytdlp.WaitForExitAsync();
        }
        try
        {
            var jsonVidData = await File.ReadAllTextAsync(Dirs.VideoInfoJson(id));
            video = JsonConvert.DeserializeObject<VideoData>(jsonVidData);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception: " + ex.Message);
            throw;
        }
        //await ConvertSubs();
    }

    private Process YtdlpFactory(string id)
    {
        var process = new Process();
        process.StartInfo.FileName = Dirs.GetPathApp("yt-dlp");
        process.StartInfo.Arguments = $"{id} -o {Path.Combine(Dirs.VideoIdFolder(id), id + ".webm")}";
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.RedirectStandardInput = true;
        process.StartInfo.RedirectStandardOutput = true;
        return process;
    }

    /*
    async Task ConvertSubs()
    {
        Directory.CreateDirectory(Path.Combine(Settings.permasaveSubtitles ? Path.Combine(Dirs.localDir, "subtitles") : Path.GetTempPath(), id));
        List<Task> subtitleOps = new();
        var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        foreach (Subtitle subtitle in video.subtitles)
        {
            subtitleOps.Add(ConvertSubsInner(subtitle, timeout.Token));
        }
        await Task.WhenAll(subtitleOps);
    }

    async Task ConvertSubsInner(Subtitle subtitle, CancellationToken cancel)
    {
        if (Settings.permasaveSubtitles)
        {
            Directory.CreateDirectory(Path.Combine(Dirs.localDir, "subtitles"));
        }
        var subPath = Path.Combine(Settings.permasaveSubtitles ? Path.Combine(Dirs.localDir, "subtitles") : Dirs.VideoIdFolder(id), $"{Dirs.MakeFileSafe(subtitle.name, replaceSpace: true)}{(subtitle.autoGenerated ? "-auto" : "")}.{subtitle.code}.srt");
        if (File.Exists(subPath))
        {
            subtitles.Add(subPath);
            return;
        }
        var subUrl = new Uri(subtitle.url);
        using var response = await client.GetAsync(subUrl, cancel);
        if (!response.IsSuccessStatusCode)
        {
            return;
        }
        var xmlSubsRaw = await response.Content.ReadAsStringAsync(cancel);
        if (cancel.IsCancellationRequested)
        {
            return;
        }
        xmlSubsRaw = xmlSubsRaw.Replace("<br />", "¤").Replace("<br/>", "¤");
        var xmlSubs = new XmlDocument();
        xmlSubs.LoadXml(xmlSubsRaw);
        List<(string begin, string end, string value)> subs = new();
        foreach (XmlNode node in xmlSubs.DocumentElement!.ChildNodes[1]!.ChildNodes[0]!.ChildNodes)
        {
            if (node.Name == "p")
            {
                subs.Add((node.Attributes!["begin"]!.Value, node.Attributes!["end"]!.Value, node.InnerText));
            }
            if (cancel.IsCancellationRequested)
            {
                return;
            }
        }
        var srtBuilder = new StringBuilder();
        for (int i = 0; i < subs.Count; i++)
        {
            srtBuilder.AppendLine((i + 1).ToString());
            srtBuilder.AppendLine($"{subs[i].begin} --> {subs[i].end}");
            srtBuilder.AppendLine(subs[i].value.Replace("¤", Environment.NewLine));
            srtBuilder.AppendLine();
            if (cancel.IsCancellationRequested)
            {
                return;
            }
        }
        await File.WriteAllTextAsync(subPath, srtBuilder.ToString());
        subtitles.Add(subPath);
    }

    async Task<string?> GetJsonInner(Uri url, CancellationToken cancellationToken, string site)
    {
        LoadBar.loadMessageDebug = "Finding valid API";
        var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        var combinedCancel = CancellationTokenSource.CreateLinkedTokenSource(timeout.Token, cancellationToken);
        using var response = await client.GetAsync(url, combinedCancel.Token);
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
        LoadBar.loadMessageDebug = "Getting raw data";
        var str = await response.Content.ReadAsStringAsync(combinedCancel.Token);
        return str;
    }
    bool success = false;

    List<HttpStatusCode> codes = new();

    private async Task<(bool success, Video value)> TryWithBestSite(string id, CancellationToken cancel)
    {
        var value = new Video();
        if (!sites.Contains(bestSite))
        {
            bestSite = sites[0];
        }
        var url = new Uri($"{bestSite}{searchFormat}{id}");
        var getJson = GetJsonInner(url, cancel, bestSite);
        string? json = await getJson;
        if (json == null)
        {
            return (false, value);
        }
        try
        {
            LoadBar.loadMessageDebug = "Parsing data";
            value = JsonConvert.DeserializeObject<Video>(json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception: " + ex.Message);
            File.WriteAllText(Path.GetTempPath() + "errorlog.txt", JsonConvert.SerializeObject(ex.Data));
            throw;
        }
        if (value == null)
        {
            return (false, new Video());
        }
        LoadBar.loadMessageDebug = "Checking video streams";
        foreach (VideoStream stream in value.videoStreams)
        {
            if (ValidateStream(stream))
            {
                validStreams.Add(stream);
            }
        }
        if (validStreams.Count > 0)
        {
            await File.WriteAllTextAsync(Path.Combine(Dirs.VideoIdFolder(id), "data.json"), json);
            var validStreamsJson = JsonConvert.SerializeObject(validStreams);
            await File.WriteAllTextAsync(Path.Combine(Dirs.VideoIdFolder(id), "validStreams.json"), validStreamsJson);
            return (true, value);
        }
        else
        {
            return (false, value);
        }
    }

    async Task<Video> Main(string id)
    {
        if (File.Exists(Path.Combine(Dirs.VideoIdFolder(id), "data.json")))
        {
            var fileJson = await File.ReadAllTextAsync(Path.Combine(Dirs.VideoIdFolder(id), "data.json"));
            var fileTempVideo = JsonConvert.DeserializeObject<Video>(fileJson);
            if (File.Exists(Path.Combine(Dirs.VideoIdFolder(id), "validStreams.json")))
            {
                var validStreamsRaw = await File.ReadAllTextAsync(Path.Combine(Dirs.VideoIdFolder(id), "validStreams.json"));
                validStreams = JsonConvert.DeserializeObject<List<VideoStream>>(validStreamsRaw)!;
            }
            else
            {
                foreach (VideoStream stream in fileTempVideo.videoStreams)
                {
                    if (await ValidateStreamAsync(stream))
                    {
                        validStreams.Add(stream);
                    }
                }
                if (validStreams.Count() > 0)
                {
                    var validStreamsJson = JsonConvert.SerializeObject(validStreams);
                    await File.WriteAllTextAsync(Path.Combine(Dirs.VideoIdFolder(id), "validStreams.json"), validStreamsJson);
                }
            }
            if (validStreams.Count() > 0)
            {
                return fileTempVideo;
            }
        }
        var youtube = new YoutubeClient();
        var video = await youtube.Videos.GetAsync(id);
        Dictionary<Task, string> taskWithSite = new();
        Video tempVideo;
        string? json = null;
        var cancel = new CancellationTokenSource();
        var bestSiteAttempt = await TryWithBestSite(id, cancel.Token);
        if (bestSiteAttempt.success)
        {
            return bestSiteAttempt.value;
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
                LoadBar.loadMessageDebug = "Converting to usable format";
                json = maybe;
            }
            else
            {
                continue;
            }
            try
            {
                LoadBar.loadMessageDebug = "Parsing data";
                tempVideo = JsonConvert.DeserializeObject<Video>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                throw;
            }
            LoadBar.loadMessageDebug = "Checking video streams";
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
                await File.WriteAllTextAsync(Path.Combine(Dirs.VideoIdFolder(id), "data.json"), json);
                var validStreamsJson = JsonConvert.SerializeObject(validStreams);
                await File.WriteAllTextAsync(Path.Combine(Dirs.VideoIdFolder(id), "validStreams.json"), validStreamsJson);
                return tempVideo;
            }
            else if (Globals.debug)
            {
                Console.WriteLine("Found API but video streams were invalid");
            }
        }
        if (!success)
        {
            cancel.Cancel();
            //The offline instance list might be out of date, so if all of the offline instances fail, then the list is updated and the method is run again.
            if (gotSitesOffline)
            {
                gotSitesOffline = false;
                await GetSites();
                tempVideo = await Main(id);
                return tempVideo;
            }
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.WriteLine($"Usable mirror could not be found. Try again later.");
            foreach (HttpStatusCode code in codes)
            {
                Console.WriteLine($"{codes.IndexOf(code)}: {code} ({(int)code})");
            }
            Globals.Exit(1);
        }
        //This code is unreachable and exists purely to make the compiler happy.
        //If it did manage to run, there would be a lot of unset parameter errors.
        return new Video();
    }

    private async Task<bool> ValidateStreamAsync(VideoStream stream)
    {
        if (Globals.noCheckStream) return true;
        using var request = new HttpRequestMessage(HttpMethod.Head, stream.url);
        using var testResponse = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        return testResponse.IsSuccessStatusCode;
    }
    */

    public async Task Play(string format = "mp4", string resolution = "720p")
    {
        mediaPlayer = new();
        mediaPlayer.StartInfo.FileName = Dirs.GetPathApp("mpv");
        FormatData? videoStream = video.Formats.Where(i => i.FormatNote == resolution).Where(i => i.Acodec == "none").ToList().Find(i => i.Ext == format);
        FormatData? audioStream = video.Formats.Where(i => i.AudioChannels != null).ToList().Find(i => i.Vcodec == "none");
        if (videoStream == null)
        {
            videoStream = video.Formats.Where(i => i.Protocol == "https").Where(i => i.AudioChannels == null).ToList().Find(i => i.Vcodec != "none");
        }
        string subsAsArg = "";
        string delimiter = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ";" : ":";
        for (int i = 0; i < subtitles.Count(); i++)
        {
            subsAsArg += subtitles[i];
            if (i < subtitles.Count() - 1)
            {
                subsAsArg += delimiter;
            }
        }
        mediaPlayer.StartInfo.Arguments = $"{(subsAsArg == "" ? "" : "--sub-files=")}\"{subsAsArg}\" \"{videoStream.Url}\" --audio-file=\"{audioStream.Url}\" --title=\"{video.Title} | {video.Channel}\"";
        playing = true;
        try
        {
            mediaPlayer.Start();
            await mediaPlayer.WaitForExitAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to start media player: " + ex.Message);
        }
    }

    public string ParsedDuration()
    {
        if (video.Duration == null) return "";
        int minutes = (int)video.Duration / 60;
        if (minutes < 60)
        {
            return $"{minutes}:{(int)video.Duration % 60}";
        }
        else
        {
            return $"{minutes / 60}:{minutes % 60}:{(int)video.Duration % 60}";
        }
    }
}
