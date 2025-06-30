using Iso639;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace YTCons.MenuBlocks;

public class ChooseResolution : MenuBlock
{
    public bool inactiveBecausePlaying = false;
    private ExtractedVideoInfo info;
    private string chosenFormat;

    public ChooseResolution(ExtractedVideoInfo info, string chosenFormat, bool download, AnchorType anchorType = AnchorType.Cursor) : base(anchorType)
    {
        this.info = info;
        this.chosenFormat = chosenFormat;
        List<string> resolutions = new List<string>();
        foreach (VideoStream stream in info.video.videoStreams)
        {
            if (!resolutions.Contains(stream.quality) && stream.format == chosenFormat)
            {
                resolutions.Add(stream.quality);
            }
        }
        foreach (string resolution in resolutions)
        {
            options.Add(new MenuOption(resolution, this, download ? () => Download(resolution) : () => Play(resolution)));
        }
        options[cursor].selected = true;
    }

    private async Task Download(string resolution)
    {
        if (Globals.debug)
        {
            Console.Write(info.subtitles);
        }
        var rightStream = info.validStreams.Find(stream => stream.format == chosenFormat && stream.quality == resolution);
        if (rightStream == null)
        {
            Globals.activeScene.PopMenu();
            Globals.activeScene.PopMenu();
            return;
        }
        var targetPath = $"{Path.Combine(Dirs.downloadsDir, Dirs.MakeFileSafe(info.video.title))}.{(chosenFormat == "MPEG_4" ? "mp4" : chosenFormat.ToLower())}";
        var targetPathNoSub = $"{Path.Combine(Dirs.downloadsDir, Dirs.MakeFileSafe(info.video.title))}-noSub.{(chosenFormat == "MPEG_4" ? "mp4" : chosenFormat.ToLower())}";
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
            ytdlp.StartInfo.Arguments = $"-P {Dirs.downloadsDir} {(chosenFormat == "MPEG_4" ? "-f mp4" : "")} -o \"{info.video.title}{(Dirs.TryGetPathApp("ffmpeg") != null && info.subtitles.Count() > 0 ? "-noSub" : "")}.{(chosenFormat == "MPEG_4" ? "mp4" : chosenFormat.ToLower())}\" -S \"res:{formattedResolution}{(fps == null ? "" : ",fps:" + fps)}\" {info.id}";
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
        else if (Dirs.TryGetPathApp("ffmpeg") != null)
        {
            var ffmpeg = new Process();
            var chosenPath = Dirs.TryGetPathApp("ffmpeg") != null && info.subtitles.Count() > 0 ? targetPathNoSub : targetPath;
            ffmpeg.StartInfo.Arguments = $"-i \"{rightStream.url}\" -i {info.video.audioStreams[0].url} -c:v libx264 -c:a aac \"{chosenPath}\"";
            ffmpeg.StartInfo.FileName = Dirs.TryGetPathApp("ffmpeg");
            ffmpeg.Start();
            await ffmpeg.WaitForExitAsync();
        }
        if (Dirs.TryGetPathApp("ffmpeg") != null && info.subtitles.Count() > 0)
        {
            var ffmpeg = new Process();
            var subInput = "";
            var subMaps = info.subtitles.Count() > 1 ? "-map 0 " : "";
            var subMetadata = $"-c:s {(chosenFormat == "WEBM" ? "webvtt" : "mov_text")} ";
            for (int i = 0; i < info.subtitles.Count(); i++)
            {
                subInput += $"-i {info.subtitles[i]} ";
                if (subMaps != "")
                {
                    subMaps += $"-map {i + 1} ";
                }
                var twoLetterCode = Regex.Match(info.subtitles[i], @"\w+\.(.*?)\.srt").Groups[1].Value;
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
        LoadBar.WriteLog($"Video \"{info.video.title}\" was downloaded to {Dirs.downloadsDir}.");
    }

    private async Task Play(string resolution)
    {
        active = false;
        await info.Play(chosenFormat, resolution);
        inactiveBecausePlaying = true;
    }

    protected override void OnUpdate()
    {
        if (inactiveBecausePlaying && info.mediaPlayer.HasExited)
        {
            inactiveBecausePlaying = false;
            Reset();
            Console.Clear();
        }
    }
}
