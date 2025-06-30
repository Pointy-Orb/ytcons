using Newtonsoft.Json;

namespace YTCons.MenuBlocks;

public class PlaylistOptions : MenuBlock
{
    private string path;
    private MenuBlock rootMenu;

    public PlaylistOptions(string path, MenuBlock rootMenu, AnchorType anchorType = AnchorType.Cursor) : base(anchorType)
    {
        this.path = path;
        this.rootMenu = rootMenu;
        options.Add(new MenuOption("Rename", this, () => Task.Run(RenamePlaylist)));
        options.Add(new MenuOption("Delete", this, () => Task.Run(RemovePlaylist)));
        options[cursor].selected = true;
    }

    private void RenamePlaylist()
    {
        var newName = Globals.ReadLine(selectedDrawPos.x, selectedDrawPos.y, " >  Enter a new name for the playlist: ");
        newName = newName.Replace("_", " ");
        newName = Dirs.MakeFileSafe(newName);
        var listsRaw = Directory.EnumerateFiles(Dirs.playlistDir);
        List<string> lists = new();
        foreach (string rawList in listsRaw)
        {
            lists.Add(Globals.BeautifyPlaylistName(rawList));
        }
        while (lists.Contains(newName))
        {
            LoadBar.WriteLog("You can't use the name of a playlist that already exists.");
            newName = Globals.ReadLine(selectedDrawPos.x, selectedDrawPos.y, " >  Enter a new name for the playlist: ");
        }
        newName = Path.Combine(Dirs.playlistDir, newName.Replace(" ", "_") + ".json");
        var playlistOption = Globals.activeScene.menus.Reverse().First().options.Find(i => i.option == Globals.BeautifyPlaylistName(path));
        if (playlistOption != null)
        {
            playlistOption.option = Globals.BeautifyPlaylistName(newName);
        }
        if (playlistOption.childMenu != null)
        {
            foreach (MenuOption option in playlistOption.childMenu.options)
            {
                if (option.childMenu == null) continue;
                option.childMenu.options.Find(i => i.option == "Remove from Playlist").extraData = newName;
            }
        }
        File.Copy(path, newName);
        File.Delete(path);
        Globals.activeScene.PopMenu();
    }

    private void RemovePlaylist()
    {
        var confirm = new MenuBlock(AnchorType.Cursor);
        confirm.options.Add(new MenuOption("Cancel", confirm, () => Task.Run(() => Globals.activeScene.PopMenu())));
        confirm.options[confirm.cursor].selected = true;
        confirm.options.Add(new MenuOption("Confirm Deletion", confirm, () => Task.Run(RemovePlaylistInner)));
        Globals.activeScene.PushMenu(confirm);
    }

    private void RemovePlaylistInner()
    {
        File.Delete(path);
        rootMenu.options.RemoveAll(i => i.option == Globals.BeautifyPlaylistName(path));
        Globals.activeScene.PopMenu();
        Globals.activeScene.PopMenu();
        try
        {
            rootMenu.options[rootMenu.cursor].selected = true;
        }
        catch
        {
            rootMenu.cursor = rootMenu.options.Count() - 1;
            rootMenu.options[rootMenu.cursor].selected = true;
        }
        LoadBar.WriteLog($"Removed playlist \"{Globals.BeautifyPlaylistName(path)}\"");
    }

    private static void RemovePlaylistInner(string path, MenuBlock rootMenu)
    {
        File.Delete(path);
        rootMenu.options.RemoveAll(i => i.option == Globals.BeautifyPlaylistName(path));
        Globals.activeScene.PopMenu();
        Globals.activeScene.PopMenu();
        try
        {
            rootMenu.options[rootMenu.cursor].selected = true;
        }
        catch
        {
            rootMenu.cursor = rootMenu.options.Count() - 1;
            rootMenu.options[rootMenu.cursor].selected = true;
        }
        LoadBar.WriteLog($"Removed playlist \"{Globals.BeautifyPlaylistName(path)}\"");
    }

    public static async Task RemoveVideo(MenuOption option, MenuBlock menu, ExtractedVideoInfo videoInfo)
    {
        if (option.extraData == null) return;

        var listRaw = await File.ReadAllTextAsync(option.extraData);
        var list = JsonConvert.DeserializeObject<List<string>>(listRaw);
        list.Remove(videoInfo.id);
        menu.options.RemoveAll(i => i.option == videoInfo.video.title);
        var listJson = JsonConvert.SerializeObject(list);
        await File.WriteAllTextAsync(option.extraData, listJson);
        Globals.activeScene.PopMenu();
        if (menu.options.Count() > 0)
        {
            if (menu.cursor >= menu.options.Count())
            {
                menu.cursor = menu.options.Count - 1;
            }
            menu.options[menu.cursor].selected = true;
            LoadBar.WriteLog($"Video \"{videoInfo.video.title}\" was removed from playlist.");
        }
        else
        {
            RemovePlaylistInner(option.extraData, Globals.activeScene.menus.Reverse().First());
            LoadBar.WriteLog($"Video \"{videoInfo.video.title}\" was removed from playlist. Empty playlist was removed");
        }
    }
}
