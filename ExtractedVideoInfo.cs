using System.Text.RegularExpressions;
using System.Text;
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
        if (!instance.success)
        {
            return instance;
        }
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

    public bool success = true;

    async Task Init()
    {
        if (!File.Exists(Dirs.VideoInfoJson(id)) || DateTime.Now - File.GetCreationTime(Dirs.VideoInfoJson(id)) > TimeSpan.FromHours(2))
        {
            var ytdlp = YtdlpFactory(id);
            ytdlp.StartInfo.Arguments += " --write-info-json";
            ytdlp.StartInfo.Arguments += " --skip-download";
            ytdlp.Start();
            await ytdlp.WaitForExitAsync();
            if (ytdlp.ExitCode > 1)
            {
                LoadBar.WriteLog("yt-dlp failed to grab metadata");
                success = false;
                return;
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
        ConcurrentBag<string> pathBag = new();
        List<Task> subTasks = new();
        foreach (string lang in video.Subtitles.Keys)
        {
            if (video.Subtitles[lang][0].Name == null) continue;
            var subPath = Path.Combine(Dirs.VideoIdFolder(id), Dirs.MakeFileSafe(video.Subtitles[lang][0].Name, true, true) + $".{lang}.srt");
            if (File.Exists(subPath))
            {
                subtitles.Add(subPath);
                continue;
            }
            subTasks.Add(GetSubsInner(video.Subtitles[lang], pathBag, subPath));
        }
        if (subTasks.Count() <= 0) return;

        await Task.WhenAll(subTasks);
        foreach (string path in pathBag)
        {
            subtitles.Add(path);
        }
    }

    private async Task GetAutoSubs()
    {
        var language = video.Language;
        if (language == null)
        {
            language = video.AutomaticCaptions.Keys.ToList().Find(i => i.Contains("orig"));
            if (language != null)
            {
                language = Regex.Replace(language, "(.*)-.*", "$1");
            }
            else
            {
                //TODO: Dynamically get failsafe language from system config
                language = "en";
            }
        }
        var subPath = Path.Combine(Dirs.VideoIdFolder(id), language + $"-auto.{language}.srt");
        if (File.Exists(subPath))
        {
            subtitles.Add(subPath);
            return;
        }
        string? autoSub = video.AutomaticCaptions.Keys.ToList().Find(i => i.Contains(language));
        if (autoSub == null)
        {
            var deHyphenLanguage = Regex.Replace(language, "(.*)-.*", "$1");
            autoSub = video.AutomaticCaptions.Keys.ToList().Find(i => i.Contains(deHyphenLanguage));
        }
        if (autoSub == null)
        {
            return;
        }
        await GetSubsInner(video.AutomaticCaptions[autoSub], subtitles, subPath);
    }

    private async Task GetSubsInner(SubLang[] lang, IEnumerable<string> collection, string path)
    {
        SubLang? srt = lang.ToList().Find(i => i.Ext == "srt");
        if (srt == null) return;
        using var client = new HttpClient();
        using var subStream = await client.GetStreamAsync(srt.Url);
        using var fileStream = File.Create(path);
        await subStream.CopyToAsync(fileStream);
        if (collection is ConcurrentBag<string> bag)
        {
            bag.Add(path);
        }
        else if (collection is List<string> list)
        {
            list.Add(path);
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
        process.StartInfo.Arguments = $"https://youtu.be/{id} -o {Path.Combine(Dirs.VideoIdFolder(id), id + ".webm")}";
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
        List<FormatData>? audioStreams = video.Formats.Where(i => i.AudioChannels != null).Where(i => i.Vcodec == "none").ToList();
        List<string> langs = new();
        foreach (FormatData audioStream in audioStreams)
        {
            if (audioStream.Language != null && !langs.Contains(audioStream.Language))
            {
                langs.Add(audioStream.Language);
            }
        }
        var audioPath = "";
        if (audioStreams != null)
        {
            if (langs.Count > 1)
            {
                await MakeAudioScript(audioStreams, langs);
                audioPath = $"--script=\"{Path.Combine(Dirs.VideoIdFolder(id), "audio.lua")}\"";
            }
            else
            {
                var mediumStreams = audioStreams.Where(i => i.FormatNote.Contains("medium")).ToArray();
                if (mediumStreams != null && mediumStreams.Length > 0)
                {
                    audioPath = $"--audio-file=\"{mediumStreams[0].Url}\"";
                }
                else
                {
                    audioPath = $"--audio-file=\"{audioStreams[0].Url}\"";
                }
            }
        }
        mediaPlayer.StartInfo.Arguments = $"{(subsAsArg == "" ? "" : "--sub-files=")}\"{subsAsArg}\" \"{videoStream.Url}\" --title=\"{video.Title} | {video.Channel}\" {audioPath}";
        playing = true;
        try
        {
            Console.Clear();
            mediaPlayer.Start();
            await mediaPlayer.WaitForExitAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to start media player: " + ex.Message);
        }
    }

    //The audio is loaded with a special script in case of videos with mulitple audio tracks.
    //This ensures that all of the tracks load with the right language tag, and in a good order.
    private async Task MakeAudioScript(List<FormatData> audioStreams, List<string> langs)
    {
        if (!File.Exists(Path.Combine(Dirs.VideoIdFolder(id), "audio.lua")))
        {
            List<string> audioTracks = new();
            List<string> usedLangs = new();
            bool selected = false;
            for (int i = 0; i < audioStreams.Count; i++)
            {
                if (usedLangs.Contains(audioStreams[i].Language)) continue;

                //Only one audio file can be selected by default
                bool justSelected = audioStreams[i].FormatNote.Contains("default");
                justSelected = selected ? false : justSelected;
                selected = justSelected ? true : selected;

                var langDiff = new List<string>();
                foreach (string lang in langs)
                {
                    if (!usedLangs.Contains(lang))
                    {
                        langDiff.Add(lang);
                    }
                }
                if (langDiff.Count <= 1 && !selected)
                {
                    justSelected = true;
                }
                //Select vs auto flags ensure that the video's native language is prioritized over any auto-generated dubs
                var track = $"audio-add {audioStreams[i].Url} {(justSelected ? "select" : "auto")} \\\"{audioStreams[i].FormatNote}\\\"";
                if (audioStreams[i].Language != null)
                {
                    var twoLetterCode = Regex.Replace(audioStreams[i].Language, "(.*)-.*", "$1");
                    var lang = Language.FromPart1(twoLetterCode);
                    if (lang != null)
                    {
                        track += $" {lang.Part2}";
                    }
                    usedLangs.Add(audioStreams[i].Language);
                }
                audioTracks.Add($"mp.command(\"{track}\")");
            }
            audioTracks.Sort((left, right) =>
            {
                bool leftOrig = left.Contains("default");
                bool rightOrig = right.Contains("default");

                if (leftOrig && !rightOrig) return -1;
                if (!leftOrig && rightOrig) return 1;

                return string.Compare(left, right);
            });
            StringBuilder audioFile = new("mp.register_event(\"file-loaded\", function()");
            audioFile.AppendLine();
            foreach (string track in audioTracks)
            {
                audioFile.AppendLine($"	{track}");
            }
            audioFile.AppendLine("end)");
            await File.WriteAllTextAsync(Path.Combine(Dirs.VideoIdFolder(id), "audio.lua"), audioFile.ToString());
        }
    }

    public async Task Download(string format, string resolution)
    {
        if (Globals.debug)
        {
            Console.Write(subtitles);
        }
        var targetPath = $"{Path.Combine(Dirs.downloadsDir, Dirs.MakeFileSafe(video.Title))}.{format}";
        if (format == "mp3")
        {
            var targetFolder = Path.Combine(Dirs.downloadsDir, "music");
            Directory.CreateDirectory(targetFolder);
            targetPath = Path.Combine(targetFolder, $"{Dirs.MakeFileSafe(video.Title)}.{format}");
        }
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
            var resolutionStuff = $"-S \"res:{formattedResolution}{(fps == null ? "" : ",fps:" + fps)}\"";
            if (format == "mp3") resolutionStuff = "";
            ytdlp.StartInfo.Arguments = $"-P {(format == "mp3" ? Path.Combine(Dirs.downloadsDir, "music") : Dirs.downloadsDir)} {(format == "webm" ? "" : $"-t {format}")} -o \"{Dirs.MakeFileSafe(video.Title)}{(Dirs.TryGetPathApp("ffmpeg") != null && subtitles.Count() > 0 && format != "mp3" ? "-noSub" : "")}.{format}\" {resolutionStuff} {id}";
            if (Globals.debug)
            {
                Console.WriteLine(ytdlp.StartInfo.Arguments);
            }
            Console.SetCursorPosition(0, Console.WindowHeight);
            ytdlp.Start();
            await ytdlp.WaitForExitAsync();
            if (!Globals.debug)
            {
                Console.Clear();
            }
        }
        if (Dirs.TryGetPathApp("ffmpeg") != null && subtitles.Count() > 0 && format != "mp3")
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
                var twoLetterCode = Regex.Match(subtitles[i], @".*?\.([^\.-]+).*?\.srt").Groups[1].Value;
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
        if (File.Exists(targetPath))
        {
            var prelude = format == "mp3" ? "Audio track" : "Video";
            LoadBar.WriteLog($"{prelude} \"{video.Title}\" was downloaded to {(format == "mp3" ? Path.Combine(Dirs.downloadsDir, "music") : Dirs.downloadsDir)}.");
        }
        else
        {
            LoadBar.WriteLog("Something went wrong with the download.");
        }
    }
}
