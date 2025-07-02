using System.Text.RegularExpressions;
using System.Collections.Concurrent;
using System.Collections;
using System.Runtime.InteropServices;
using System.Reflection;
using Iso639;
using System.Diagnostics;
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

    public static async Task<ExtractedVideoInfo> CreateAsync(string id)
    {
        var instance = new ExtractedVideoInfo(id);
        await instance.Init();
        instance.gotVideo = true;
        await instance.GetSubtitles();
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
            if (ytdlp.ExitCode > 1)
            {
                throw new Exception("yt-dlp failed to grab the file");
            }
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
    }

    private async Task GetSubtitles()
    {
        var autoSubs = GetAutoSubs();
        var manualSubs = GetManualSubs();
        await autoSubs;
        await manualSubs;
    }

    private async Task GetManualSubs()
    {
        PropertyInfo[] langs = video.Subtitles.GetType().GetProperties();
        ConcurrentBag<string> pathBag = new();
        List<Task> subTasks = new();
        foreach(PropertyInfo lang in langs)
        {
            var language = lang.Name;
            if (language == "as") language = "ase";
            if (language == "is") language = "isl";
            if (language == "new") language = "nwa";
            var nullLang = video.Subtitles.GetType().GetProperty(language);
            if(nullLang == null) continue;
            var langList = (List<SubLang>?)nullLang.GetValue(video.Subtitles);
            if(langList == null || langList.Count() < 1) continue;
            var subPath = Path.Combine(Dirs.VideoIdFolder(id), Dirs.MakeFileSafe(langList[0].Name,true,true)+ $".{language}.srt");
            if (File.Exists(subPath))
            {
                subtitles.Add(subPath);
                continue;
            }
            subTasks.Add(GetSubsInner(langList,pathBag,subPath));
        }
        if(subTasks.Count() <= 0) return;

        await Task.WhenAll(subTasks);
        foreach(string path in pathBag)
        {
            subtitles.Add(path);
        }
    }

    private async Task GetAutoSubs()
    {
        var language = video.Language;
        var subPath = Path.Combine(Dirs.VideoIdFolder(id), language + $"-auto.{language}.srt");
        //Some languages are problematic keywords. These were changed in the json but have to be accounted for in the live conversion
        if (language == "as") language = "ase";
        if (language == "is") language = "isl";
        if (language == "new") language = "nwa";
        if (File.Exists(subPath))
        {
            subtitles.Add(subPath);
            return;
        }
        List<SubLang>? autoSub = (List<SubLang>?)video.AutomaticCaptions.GetType().GetProperty(video.Language)!.GetValue(video.AutomaticCaptions);
        if (autoSub == null) return;
        await GetSubsInner(autoSub,subtitles,subPath);
    }

    private async Task GetSubsInner(List<SubLang> lang,IEnumerable<string> collection, string path)
    {
        SubLang? srt = lang.Find(i => i.Ext == "srt");
        if (srt == null) return;
        using var client = new HttpClient();
        using var subStream = await client.GetStreamAsync(srt.Url);
        using var fileStream = File.Create(path);
        await subStream.CopyToAsync(fileStream);
        if(collection is ConcurrentBag<string> bag)
        {
            bag.Add(path);
        }
        else
        {
            collection.Append(path);
        }
    }

    private Process YtdlpFactory(string id)
    {
        var process = new Process();
        process.StartInfo.FileName = Dirs.GetPathApp("yt-dlp");
        process.StartInfo.Arguments = $"{id} -o {Path.Combine(Dirs.VideoIdFolder(id), id + ".webm")}";
        if (!Globals.debug)
        {
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
        }
        return process;
    }

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

    public async Task Download(string format, string resolution)
    {
        if (Globals.debug)
        {
            Console.Write(subtitles);
        }
        var targetPath = $"{Path.Combine(Dirs.downloadsDir, Dirs.MakeFileSafe(video.Title))}.{format}";
        var targetPathNoSub = $"{Path.Combine(Dirs.downloadsDir, Dirs.MakeFileSafe(video.Title))}-noSub.{(format == "MPEG_4" ? "mp4" : format.ToLower())}";
        if (File.Exists(targetPath))
        {
            File.Delete(targetPath);
        }
        if (Dirs.TryGetPathApp("yt-dlp") != null)
        {
            var ytdlp = new Process();
            ytdlp.StartInfo.FileName = Dirs.TryGetPathApp("yt-dlp");
            //Get rid of the p's and the fps's
            var formattedResolution = Regex.Replace(resolution, @"(\d*?)p.*", "$1");
            string? fps = Regex.Replace(resolution, @"\d*p(\d+)", "$1");
            if (fps == resolution)
            {
                fps = null;
            }
            ytdlp.StartInfo.Arguments = $"-P {Dirs.downloadsDir} {(format == "mp4" ? "-f mp4" : "")} -o \"{video.Title}{(Dirs.TryGetPathApp("ffmpeg") != null && subtitles.Count() > 0 ? "-noSub" : "")}.{format}\" -S \"res:{formattedResolution}{(fps == null ? "" : ",fps:" + fps)}\" {id}";
            if (Globals.debug)
            {
                Console.WriteLine(ytdlp.StartInfo.Arguments);
            }
            ytdlp.Start();
            await ytdlp.WaitForExitAsync();
            if (!Globals.debug)
            {
                Console.Clear();
            }
        }
        if (Dirs.TryGetPathApp("ffmpeg") != null && subtitles.Count() > 0)
        {
            var ffmpeg = new Process();
            var subInput = "";
            var subMaps = subtitles.Count() > 1 ? "-map 0 " : "";
            var subMetadata = $"-c:s {(format == "WEBM" ? "webvtt" : "mov_text")} ";
            for (int i = 0; i < subtitles.Count(); i++)
            {
                subInput += $"-i {subtitles[i]} ";
                if (subMaps != "")
                {
                    subMaps += $"-map {i + 1} ";
                }
                var twoLetterCode = Regex.Match(subtitles[i], @"\w+\.(.*?)\.srt").Groups[1].Value;
                var language = Language.FromPart1(twoLetterCode);
                var threeLetterCode = language == null ? twoLetterCode : language.Part2;
                subMetadata += $"-metadata:s:s:{i} language={threeLetterCode} ";
            }
            ffmpeg.StartInfo.Arguments = $"-i \"{targetPathNoSub}\" {subInput} {subMaps} -c copy {subMetadata} \"{targetPath}\"";
            ffmpeg.StartInfo.FileName = Dirs.TryGetPathApp("ffmpeg");
            if (Globals.debug)
            {
                Console.SetCursorPosition(0, 10);
                Console.WriteLine(ffmpeg.StartInfo.Arguments);
                Console.SetCursorPosition(0, 60);
            }
            ffmpeg.Start();
            await ffmpeg.WaitForExitAsync();
            File.Delete(targetPathNoSub);
        }
        if (Globals.debug)
        {
            Console.ReadKey();
        }
        else
        {
            Console.Clear();
        }
        Globals.activeScene.PopMenu();
        Globals.activeScene.PopMenu();
        if (File.Exists(Path.Combine(Dirs.downloadsDir, $"{video.Title}.{format}")))
        {
            LoadBar.WriteLog($"Video \"{video.Title}\" was downloaded to {Dirs.downloadsDir}.");
        }
        else
        {
            LoadBar.WriteLog("Something went wrong with the download.");
        }
    }
}
