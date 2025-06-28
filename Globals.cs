using YTCons.Scenes;
using YTCons.UserExp;
using static System.Environment;

namespace YTCons;

public enum AnchorType
{
    Top,
    Center,
    Bottom,
    Cursor
}

public static class Globals
{
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

    internal static Stack<Scene> scenes = new();

    internal static Scene activeScene => scenes.Peek();

    public static bool debug = false;
    //This flag is for debugging purposes and should not be set to true by the average user unless they like errors or are just that impatient
    public static bool noCheckStream = false;

    internal static async Task Init(string[] args)
    {
        debug = args.Contains("--debug");
        noCheckStream = args.Contains("--no-check-stream");
        await ExtractedVideoInfo.GetSites();
        var sceneTask = TestScene.CreateAsync();
        LoadBar.loadMessage = "Getting available sites";
        Console.ResetColor();
        defaultForeground = Console.ForegroundColor;
        defaultBackground = Console.BackgroundColor;
        var scene = await sceneTask;
        scenes.Push(scene);
        await scene.DoSearch();
    }

    internal static int oldWindowWidth = Console.WindowWidth;
    internal static int oldWindowHeight = Console.WindowHeight;

    //Same as ReadLine but won't try again if null
    internal static string? ReadLineNull(int i, int j, string prompt)
    {
        Console.CursorVisible = true;
        var value = ReadLineInner(i, j, prompt);
        Console.CursorVisible = false;
        return value;
    }

    internal static string ReadLine(int i, int j, string prompt)
    {
        Console.CursorVisible = true;
        var value = ReadLineInner(i, j, prompt);
        while (value == null)
        {
            value = ReadLineInner(i, j, prompt);
        }
        Console.CursorVisible = false;
        return value;
    }

    private static string? ReadLineInner(int i, int j, string prompt)
    {
        Console.SetCursorPosition(i, j);
        Console.Write(prompt);
        var input = Console.ReadLine();
        return input;
    }

    internal static async Task Update()
    {
        if (oldWindowWidth != Console.WindowWidth || oldWindowHeight != Console.WindowHeight)
        {
            Console.Clear();
            activeScene.ChangeWindowSize();
        }
        oldWindowWidth = Console.WindowWidth;
        oldWindowHeight = Console.WindowHeight;
        await activeScene.Update();
        if (Console.KeyAvailable)
        {
            CheckKeys();
        }
    }

    internal static void CheckKeys()
    {
        var key = Console.ReadKey(true);
        activeScene.CheckKeys(key);
        if (key.Key == ConsoleKey.Escape)
        {
            Globals.Exit(0);
        }
    }

    public static ConsoleColor? defaultForeground;
    public static ConsoleColor? defaultBackground;

    private static ConsoleColor?[,] foregroundColor = new ConsoleColor?[Console.WindowWidth, Console.WindowHeight];
    private static ConsoleColor?[,] backgroundColor = new ConsoleColor?[Console.WindowWidth, Console.WindowHeight];
    private static char[,] buffer = new char[Console.WindowWidth, Console.WindowHeight];
    private static bool[,] changed = new bool[Console.WindowWidth, Console.WindowHeight];
    private static List<(int x, int y)> chang = new();
    private static List<(int x, int y)> oldChang = new();

    internal static void Draw()
    {
        //Calling this at the beginning means that the default color can be referenced with Console.ForegroundColor and Console.BackgroundColor
        Console.ResetColor();
        if (Console.WindowWidth != oldWindowWidth || Console.WindowHeight != oldWindowHeight)
        {
            buffer = new char[Console.WindowWidth, Console.WindowHeight];
            backgroundColor = new ConsoleColor?[Console.WindowWidth, Console.WindowHeight];
            foregroundColor = new ConsoleColor?[Console.WindowWidth, Console.WindowHeight];
        }
        else
        {
            Array.Clear(buffer, 0, buffer.Length);
            Array.Clear(foregroundColor, 0, foregroundColor.Length);
            Array.Clear(backgroundColor, 0, backgroundColor.Length);
        }
        activeScene.Draw();
        try
        {
            //Remove all points that the old list and the new list share, so they are neither overwritten nor redrawn
            oldChang.RemoveAll(item => chang.Contains(item));
            foreach ((int x, int y) position in chang)
            {
                int i = position.x;
                int j = position.y;
                if (foregroundColor[i, j] == null)
                {
                    Console.ForegroundColor = (ConsoleColor)defaultForeground;
                }
                else
                {
                    Console.ForegroundColor = (ConsoleColor)foregroundColor[i, j];
                }
                if (backgroundColor[i, j] == null)
                {
                    Console.BackgroundColor = (ConsoleColor)defaultBackground;
                }
                else
                {
                    Console.BackgroundColor = (ConsoleColor)backgroundColor[i, j];
                }
                if (buffer[i, j] == Convert.ToChar("\u0000"))
                {
                    Console.SetCursorPosition(i, j);
                    Console.Write(" ");
                }
                else
                {
                    Console.SetCursorPosition(i, j);
                    Console.Write(buffer[i, j]);
                }
            }
            foreach ((int x, int y) position in oldChang)
            {
                var i = position.x;
                var j = position.y;
                Console.SetCursorPosition(i, j);
                Console.Write(" ");
            }
            oldChang = new List<(int x, int y)>(chang);
            chang.Clear();
            for (int j = 0; j < Console.WindowHeight; j++)
            {
                for (int i = 0; i < Console.WindowWidth; i++)
                {
                }
            }
        }
        catch
        {
            return;
        }
    }

    public static void Exit(int exitCode)
    {
        Console.CursorVisible = true;
        //Make debugging easier by only clearing the console if the program exited successfully
        if (exitCode == 0 && !debug)
        {
            Console.Clear();
        }
        Environment.Exit(exitCode);
    }

    internal static void Write(int i, int j, string? phrase)
    {
        if (phrase == null) return;
        for (int l = 0; l < phrase.Length; l++)
        {
            try
            {
                buffer[i + l, j] = phrase[l];
                if (!chang.Contains((i + l, j)))
                {
                    chang.Add((i + l, j));
                }
            }
            catch { }
        }
    }
    internal static void Write(int i, int j, string? phrase, out int newPos)
    {
        newPos = i;
        if (phrase == null)
        {
            return;
        }
        for (int l = 0; l < phrase.Length; l++)
        {
            try
            {
                buffer[i + l, j] = phrase[l];
                if (!chang.Contains((i + l, j)))
                {
                    chang.Add((i + l, j));
                }
            }
            catch { }
            newPos = i + l;
        }
    }

    internal static void Write(int i, int j, char letter)
    {
        try
        {
            buffer[i, j] = letter;
            if (!chang.Contains((i, j)))
            {
                chang.Add((i, j));
            }
        }
        catch { }
    }

    internal static void SetForegroundColor(int i, int j, ConsoleColor color)
    {
        try
        {
            if (foregroundColor[i, j] != color)
            {
                foregroundColor[i, j] = color;
                if (!chang.Contains((i, j)))
                {
                    chang.Add((i, j));
                }
            }
        }
        catch { }
    }

    internal static void SetBackgroundColor(int i, int j, ConsoleColor color)
    {
        try
        {
            backgroundColor[i, j] = color;
            if (!chang.Contains((i, j)))
            {
                chang.Add((i, j));
            }
        }
        catch { }
    }

    internal static void ClearTile(int i, int j)
    {
        try
        {
            buffer[i, j] = Convert.ToChar("\u0000");
        }
        catch
        {

        }
        oldChang.Add((i, j));
    }
}
