using YTCons.UserExp;
using static System.Environment;
using System.Runtime.InteropServices;

namespace YTCons;

public static class Dirs
{
    public static string? TryGetPathApp(string appname)
    {
        //Windows just had to be different
        var rawPath = System.Environment.GetEnvironmentVariable(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "Path" : "PATH");
        string[] paths = rawPath.Split(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ';' : ':');
        foreach (string path in paths)
        {
            if(!Directory.Exists(path))
            {
                continue;
            }
            var files = Directory.EnumerateFiles(path).ToList();
            string? ffmpegPath = files.Find(i => i.Contains(appname));
            if (ffmpegPath != null)
            {
                return ffmpegPath;
            }
        }
        return null;
    }

    public static string GetPathApp(string appname)
    {
        var pathapp = TryGetPathApp(appname);
        if (pathapp == null)
        {
            throw new FileNotFoundException($"App {appname} was not found on the environment path.");
        }
        return pathapp;
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

    internal static string feedsDir
    {
        get
        {
            var feeds = Path.Combine(localDir, "feeds");
			Directory.CreateDirectory(feeds);
            return feeds;
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
            .Replace("\"", "''")
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

    internal static string VideoInfoJson(string id)
    {
        if (File.Exists(Path.Combine(VideoIdFolder(id), id + ".webm.info.json")))
        {
            //The stream links expire, so every now and then they need to be updated. But the File.Move method will throw an error if the destination already exists
            //So we have to make sure that it doesn't  >:)
            if (File.Exists(Path.Combine(VideoIdFolder(id), id + ".info.json")))
            {
                File.Delete(Path.Combine(VideoIdFolder(id), id + ".info.json"));
            }
            File.Move(Path.Combine(VideoIdFolder(id), id + ".webm.info.json"), Path.Combine(VideoIdFolder(id), id + ".info.json"));
        }
        return Path.Combine(VideoIdFolder(id), id + ".info.json");
    }
}
