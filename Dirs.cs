using YTCons.UserExp;
using static System.Environment;
using System.Runtime.InteropServices;

namespace YTCons;

public static class Dirs
{
    //Making this a property means that compatibility is much easier
    internal static string? ffmpeg
    {
        get
        {
            return GetPathApp("ffmpeg");
        }
    }

    internal static string? ytdlp
    {
        get
        {
            return GetPathApp("yt-dlp");
        }
    }

    //mpv is critical to the app's function, that's why it throws an exception and ffmpeg doesn't
    internal static string mpv
    {
        get
        {
            var mpvPath = GetPathApp("mpv");
            if (mpvPath == null)
            {
                throw new NullReferenceException("mpv not found in environment path");
            }
            return mpvPath;
        }
    }

    private static string? GetPathApp(string appname)
    {

        //Windows just had to be different
        var rawPath = System.Environment.GetEnvironmentVariable(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "Path" : "PATH");
        string[] paths = rawPath.Split(':');
        foreach (string path in paths)
        {
            var files = Directory.EnumerateFiles(path).ToList();
            string? ffmpegPath = files.Find(i => i.Contains(appname));
            if (ffmpegPath != null)
            {
                return ffmpegPath;
            }
        }
        return null;
    }

    internal static string downloadsDir
    {
        get
        {
            var downloads = Path.Combine(GetFolderPath(SpecialFolder.UserProfile), "Downloads", "ytcons");
            Directory.CreateDirectory(downloads);
            return downloads;
        }
    }

    internal static string configDir
    {
        get
        {
            var config = Path.Combine(GetFolderPath(SpecialFolder.ApplicationData, SpecialFolderOption.DoNotVerify), "ytcons");
            Directory.CreateDirectory(config);
            return config;
        }
    }

    internal static string localDir
    {
        get
        {
            var local = Path.Combine(GetFolderPath(SpecialFolder.LocalApplicationData, SpecialFolderOption.DoNotVerify), "ytcons");
            Directory.CreateDirectory(local);
            var noobGuide = Path.Combine(local, "explainthisplz.txt");
            if (!File.Exists(noobGuide))
            {
                //Write a file to the local directory that tells curious eyes what these variable are for.
                File.WriteAllLines(noobGuide, ExplainStuff.LocalFiles);
            }
            return local;
        }
    }

    internal static string playlistDir
    {
        get
        {
            var playlist = Path.Combine(configDir, "playlists");
            Directory.CreateDirectory(playlist);
            return playlist;
        }
    }

    /// <summary>
    /// Takes a string and converts it to a safe filename.
    /// More verbosely, it takes a string and changes all of the problematic characters
    /// mentioned in this Wikipedia page: https://en.wikipedia.org/wiki/Filename#Problematic_characters
    /// </summary>
    /// <param name="input">The string to make file-safe</param>
	/// <param name="strict">Setting to true will force Windows-specific filename restrictions to be applied regardless of OS</param>
	/// <param name="replaceSpace">Setting to true replaces spaces with underscores</param>
    /// <returns>A version of the string that is safe to write as a filename.</returns>
    internal static string MakeFileSafe(string input, bool strict = false, bool replaceSpace = false)
    {
        string output = input
            .Replace(":", "꞉")
            .Replace("..", ". .")
            .Replace("/", "⧸");
        if (input == " ")
        {
            //I promise this is a different unicode character
            output = "⠀";
        }
        if (replaceSpace)
        {
            output = output.Replace(" ", "_");
        }
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) || strict)
        {
            output = output
                .Replace(@"\", "⧹")
                .Replace("?", "？")
                .Replace("*", "꘎")
                .Replace("|", "∣")
                .Replace("\"", "''")
                .Replace("<", "＜")
                .Replace(">", "＞");
        }
        return output;
    }

    internal static string VideoIdFolder(string id)
    {
        var path = Path.Combine(Path.GetTempPath(), id);
        Directory.CreateDirectory(path);
        return path;
    }
}
