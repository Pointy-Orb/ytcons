using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Concurrent;
using System.Xml;
using System.Runtime.InteropServices;
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
    public DateOnly uploadDate = new();
    public VideoData video = null!;
    public bool gotVideo = false;
    public bool windowOpen = false;
    public bool playing { get; private set; }
    public Process mediaPlayer = new();
    public List<string> subtitles = new();
    public string chaptersPath = "";

    internal static bool gotSites = false;

    public static async Task<ExtractedVideoInfo> CreateAsync(string id)
    {
        var instance = new ExtractedVideoInfo(id);
        await instance.Init();
        if (!instance.success)
        {
            return instance;
        }
        instance.uploadDate = DateOnly.ParseExact(instance.video.UploadDate, "yyyyMMdd");
        instance.gotVideo = true;
        await instance.GetSubtitles();
        await instance.GetChapters();
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
                string? error;
                while ((error = ytdlp.StandardError.ReadLine()) != null)
                {
                    if (error.Contains("ERROR"))
                    {
                        LoadBar.WriteLog(error);
                        break;
                    }
                }
                return;
            }
            string? line;
            while ((line = ytdlp.StandardError.ReadLine()) != null)
            {
                if (line.Contains("This live event"))
                {
                    success = false;
                    using var errorClient = new HttpClient();
                    string html = await errorClient.GetStringAsync("https://youtube.com/watch?v=" + id);
                    var match = Regex.Match(html, "startTimestamp\":\"([^\"]+)");
                    if (match.Success)
                    {
                        string start = match.Groups[1].Value;
                        var startTime = DateTimeOffset.Parse(start).LocalDateTime;
                        var dayOf = " today";
                        if (startTime.Date != DateTime.Now.Date)
                        {
                            dayOf = $" at {startTime.Date.ToShortDateString()}";
                        }
                        LoadBar.WriteLog("Live broadcast starts at " + startTime.ToShortTimeString() + dayOf);
                    }
                    return;
                }
            }
        }
        try
        {
            var jsonVidData = await File.ReadAllTextAsync(Dirs.VideoInfoJson(id));
            video = JsonConvert.DeserializeObject<VideoData>(jsonVidData);
        }
        catch (Exception ex)
        {
            LoadBar.WriteLog("Exception: " + ex.Message);
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

    private async Task GetChapters()
    {
        if (video.Chapters.Count <= 0)
        {
            return;
        }
        if (File.Exists(Path.Combine(Dirs.VideoIdFolder(id), "chapters.txt")))
        {
            chaptersPath = "--chapters-file=\"" + Path.Combine(Dirs.VideoIdFolder(id), "chapters.txt") + "\"";
        }
        List<string> lines = new();
        lines.Add(";FFMETADATA1");
        foreach (Chapter chapter in video.Chapters)
        {
            if (chapter.StartTime == null || chapter.EndTime == null)
            {
                continue;
            }
            lines.Add("[CHAPTER]");
            lines.Add("TIMEBASE=1/1");
            lines.Add("START=" + (int)chapter.StartTime);
            lines.Add("END=" + (int)chapter.EndTime);
            lines.Add("title=" + chapter.Title);
        }
        await File.WriteAllLinesAsync(Path.Combine(Dirs.VideoIdFolder(id), "chapters.txt"), lines);
        chaptersPath = "--chapters-file=\"" + Path.Combine(Dirs.VideoIdFolder(id), "chapters.txt") + "\"";
    }

    public string ParsedDuration(double? seconds)
    {
        if (seconds == null)
        {
            return "";
        }
        int intSeconds = (int)seconds;
        double milliseconds = (double)seconds - (double)intSeconds;
        int minutes = intSeconds / 60;

        var secondsString = (intSeconds % 60).ToString("D2");
        var minutesString = (minutes % 60).ToString("D2");
        var hoursString = (minutes / 60).ToString("D2");
        var millisecondsString = Regex.Replace(milliseconds.ToString(), @".*\.(.*)", "$1");
        while (millisecondsString.Length < 3)
        {
            millisecondsString += '0';
        }
        if (millisecondsString.Length > 3)
        {
            millisecondsString.Remove(3);
        }
        return $"{hoursString}:{minutesString}:{secondsString}.{millisecondsString}";
    }

    private async Task GetManualSubs()
    {
        ConcurrentBag<string> pathBag = new();
        List<Task> subTasks = new();
        foreach (string lang in video.Subtitles.Keys)
        {
            if (video.Subtitles[lang][0].Name == null) continue;
            var subFolder = Globals.settings.permasaveSubtitles ? Path.Combine(Dirs.localDir, id) : Dirs.VideoIdFolder(id);
            var subPath = Path.Combine(subFolder, Dirs.MakeFileSafe(video.Subtitles[lang][0].Name, true, true) + $".{lang}.srt");
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
        var subFolder = Globals.settings.permasaveSubtitles ? Path.Combine(Dirs.configDir, id) : Dirs.VideoIdFolder(id);
        Directory.CreateDirectory(subFolder);
        var subPath = Path.Combine(subFolder, language + $"-auto.{language}.srt");
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

    public async Task Play(string? format = null, string? resolution = null)
    {
        var rFormat = format;
        var rResolution = resolution;
        if (rFormat == null)
        {
            rFormat = Globals.settings.chosenFormat;
        }
        if (rResolution == null)
        {
            rResolution = Globals.settings.chosenResolution;
        }
        FormatData? videoStream = video.Formats.Where(i => i.FormatNote == rResolution).Where(i => i.Acodec == "none").ToList().Find(i => i.Ext == format);
        if (rResolution == "Highest")
        {
            videoStream = video.Formats.Where(i => i.Protocol == "https").Where(i => i.AudioChannels == null).Reverse().ToList().Find(i => i.Vcodec != "none");
        }
        bool useManifest = false;
        if (videoStream == null)
        {
            //Reverse so it defaults to highest resolution instead of lowest
            videoStream = video.Formats.Where(i => i.Protocol == "https").Where(i => i.AudioChannels == null).Reverse().ToList().Find(i => i.Vcodec != "none");
            if (videoStream == null)
            {
                videoStream = video.Formats.Find(i => i.Resolution.Contains(Regex.Replace(rResolution, @"(\d+).*", "$1")));
                useManifest = true;
            }
        }
        List<FormatData> audioStreams = video.Formats.Where(i => i.AudioChannels != null).Where(i => i.Vcodec == "none").ToList();
        await PlayInner(videoStream, audioStreams, useManifest);
    }

    private async Task PlayInner(FormatData? videoStream, List<FormatData> audioStreams, bool useManifest)
    {
        mediaPlayer = new();
        mediaPlayer.StartInfo.FileName = Dirs.GetPathApp("mpv");
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
        List<string> langs = new();
        foreach (FormatData audioStream in audioStreams)
        {
            if (audioStream.Language != null && !langs.Contains(audioStream.Language))
            {
                langs.Add(audioStream.Language);
            }
        }
        var audioPath = "";
        if (audioStreams != null && audioStreams.Count > 0 && !useManifest)
        {
            if (langs.Count > 1)
            {
                await MakeAudioScript(audioStreams, langs);
                audioPath = $"--script=\"{Path.Combine(Dirs.VideoIdFolder(id), "mpvAudio.lua")}\"";
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
        mediaPlayer.StartInfo.Arguments = $"{(subsAsArg == "" ? "" : "--sub-files=")}\"{subsAsArg}\" \"{(useManifest ? videoStream.ManifestUrl : videoStream.Url)}\" --title=\"{video.Title} | {video.Channel}\" {audioPath} {chaptersPath}";
        playing = true;
        try
        {
            Console.Clear();
            mediaPlayer.Start();
            await mediaPlayer.WaitForExitAsync();
            Console.Clear();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to start media player: " + ex.Message);
        }
    }

    private async Task Escape(CancellationToken token)
    {
        bool leave = false;
        while (!leave && !token.IsCancellationRequested)
        {
            if (!Console.KeyAvailable) continue;
            if (Console.ReadKey(true).Key == ConsoleKey.Escape)
            {
                leave = true;
                Console.WriteLine("leaving");
            }
            await Task.Delay(100);
        }
    }

    //The audio is loaded with a special script in case of videos with mulitple audio tracks.
    //This ensures that all of the tracks load with the right language tag, and in a good order.
    private async Task MakeAudioScript(List<FormatData> audioStreams, List<string> langs)
    {
        bool makeTheScript = !File.Exists(Path.Combine(Dirs.VideoIdFolder(id), "mpvAudio.lua"));
        if (File.Exists(Path.Combine(Dirs.VideoIdFolder(id), "mpvAudio.lua")))
        {
            //Rewrite the file if the settings have been changed since last use
            using var fileStream = File.Open(Path.Combine(Dirs.VideoIdFolder(id), "mpvAudio.lua"), FileMode.Open);
            using var reader = new StreamReader(fileStream);
            var firstLine = reader.ReadLine();
            if (firstLine != null && !firstLine.Contains("--defaultFirst: " + Globals.settings.systemLanguageAudio))
            {
                makeTheScript = true;
            }
        }
        if (makeTheScript)
        {
            List<string> audioTracks = new();
            List<string> usedLangs = new();
            bool selected = false;
            string origOrDefault = Globals.settings.systemLanguageAudio ? "default" : "original";
            for (int i = 0; i < audioStreams.Count; i++)
            {
                if (usedLangs.Contains(audioStreams[i].Language)) continue;

                //Only one audio file can be selected by default
                bool justSelected = audioStreams[i].FormatNote.Contains(origOrDefault);
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
                bool leftOrig = left.Contains(origOrDefault);
                bool rightOrig = right.Contains(origOrDefault);

                if (leftOrig && !rightOrig) return -1;
                if (!leftOrig && rightOrig) return 1;

                return string.Compare(left, right);
            });
            //Leave a tag of what the default config was when this script was made so that it can be overwritten if it changes
            StringBuilder audioFile = new($"--defaultFirst: " + Globals.settings.systemLanguageAudio);
            audioFile.AppendLine();
            audioFile.AppendLine("mp.register_event(\"file-loaded\", function()");
            foreach (string track in audioTracks)
            {
                audioFile.AppendLine($"	{track}");
            }
            audioFile.AppendLine("end)");
            await File.WriteAllTextAsync(Path.Combine(Dirs.VideoIdFolder(id), "mpvAudio.lua"), audioFile.ToString());
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
