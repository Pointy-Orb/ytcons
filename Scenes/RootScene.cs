using YTCons.MenuBlocks;
using System.Reflection;

namespace YTCons.Scenes;

public class RootScene : Scene
{
    bool childSceneOpen = false;
    string[] logo;

    public RootScene()
    {
        logo = GetLogo();
        var menu = new MenuBlock(AnchorType.Bottom);
        menu.options.Add(new MenuOption("Search", menu, () => Search()));
        menu.options.Add(new MenuOption("Playlists", menu, () => Task.Run(PlaylistSceneIfPlaylistsExist)));
        menu.options.Add(new MenuOption("Exit", menu, () => Task.Run(() => Globals.Exit(0))));
        menu.options[menu.cursor].selected = true;
        PushMenu(menu);
    }

    private void PlaylistSceneIfPlaylistsExist()
    {
        childSceneOpen = true;
        if (Directory.EnumerateFileSystemEntries(Dirs.playlistDir).Count() > 0)
        {
            Globals.scenes.Push(new PlaylistScene());
        }
        else
        {
            LoadBar.WriteLog("No playlists were found.");
        }
    }

    private async Task Search()
    {
        var selDrawPos = PeekMenu().selectedDrawPos;
        string? query = Globals.ReadLineNull(selDrawPos.x, selDrawPos.y, " >  Enter your search query (leave blank to cancel): ");
        childSceneOpen = true;
        if (query == null || query == "")
        {
            return;
        }
        else
        {
            var search = await SearchScene.CreateAsync(query);
            Globals.scenes.Push(search);
        }
    }

    private string[] GetLogo()
    {
        var info = Assembly.GetExecutingAssembly().GetName();
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"{info.Name}.logo.txt");
        using var streamReader = new StreamReader(stream, System.Text.Encoding.UTF8);
        var rawLogo = streamReader.ReadToEnd();
        List<string> logoPieces = new();
        string logoPiece = "";
        for (int i = 0; i < rawLogo.Length; i++)
        {
            if (rawLogo[i] == Convert.ToChar(Environment.NewLine))
            {
                logoPieces.Add(logoPiece);
                logoPiece = "";
                continue;
            }
            logoPiece += rawLogo[i];
        }
        return logoPieces.ToArray();
    }

    protected override void PostDraw()
    {
        int cursorX = Console.WindowWidth / 2;
        int cursorY = Console.WindowHeight / 2;
        cursorX -= logo[8].Length / 2;
        cursorY -= logo.Length / 2;
        for (int i = 0; i < logo.Length; i++)
        {
            Globals.Write(cursorX, cursorY + i, logo[i]);
        }
        for (int i = 0; i < logo.Length; i++)
        {
            for (int j = 0; j < logo[i].Length; j++)
            {
                if (logo[i][j] == Convert.ToChar("`"))
                {
                    Globals.SetForegroundColor(cursorX + j, cursorY + i, ConsoleColor.Red);
                }
            }
        }
    }

    protected override void OnUpdate()
    {
        if (childSceneOpen)
        {
            childSceneOpen = false;
            PeekMenu().Reset();
        }
    }
}
