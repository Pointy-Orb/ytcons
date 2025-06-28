using Newtonsoft.Json;

namespace YTCons.MenuBlocks;

public class PlaylistOptions : MenuBlock
{
    private MenuBlock displaylist;
    private string path;
    private MenuBlock rootMenu;

    public PlaylistOptions(string path, MenuBlock rootMenu, MenuBlock displaylist, AnchorType anchorType = AnchorType.Cursor) : base(anchorType)
    {
        this.displaylist = displaylist;
        this.path = path;
        this.rootMenu = rootMenu;
        options.Add(new MenuOption("Remove", this, () => Task.Run(RemovePlaylist)));
        options[cursor].selected = true;
    }

    private void RemovePlaylist()
    {
        var confirm = new MenuBlock(AnchorType.Cursor);
        confirm.options.Add(new MenuOption("Cancel", confirm, () => Task.Run(() => Globals.activeScene.PopMenu())));
        confirm.options[confirm.cursor].selected = true;
        confirm.options.Add(new MenuOption("Confirm", confirm, () => Task.Run(RemovePlaylistInner)));
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
            rootMenu.cursor = 0;
            rootMenu.options[rootMenu.cursor].selected = true;
        }
        LoadBar.WriteLog($"Removed playlist \"{Globals.BeautifyPlaylistName(path)}\"");
    }

    public static async Task RemoveVideo(string path, MenuBlock menu, ExtractedVideoInfo videoInfo)
    {
        var listRaw = await File.ReadAllTextAsync(path);
        var list = JsonConvert.DeserializeObject<List<string>>(listRaw);
        list.Remove(videoInfo.id);
        menu.options.RemoveAll(i => i.option == videoInfo.video.title);
        var listJson = JsonConvert.SerializeObject(list);
        await File.WriteAllTextAsync(path, listJson);
        Globals.activeScene.PopMenu();
        menu.options[menu.cursor].selected = true;
        LoadBar.WriteLog($"Video \"{videoInfo.video.title}\" was removed from playlist.");
    }
}
