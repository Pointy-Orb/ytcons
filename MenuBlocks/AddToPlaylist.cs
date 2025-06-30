using YTCons.Scenes;
using Newtonsoft.Json;

namespace YTCons.MenuBlocks;

public class AddToPlaylist : MenuBlock
{
    ExtractedVideoInfo video;

    public AddToPlaylist(ExtractedVideoInfo video, AnchorType anchorType = AnchorType.Cursor) : base(anchorType)
    {
        this.video = video;
        var lists = Directory.EnumerateFiles(Dirs.playlistDir);
        options.Add(new MenuOption("New Playlist", this, () => NewPlaylist(video)));
        options[cursor].selected = true;
        foreach (string list in lists)
        {
            var name = Globals.BeautifyPlaylistName(list);
            options.Add(new MenuOption(name, this, () => OldPlaylist(list, video)));
        }
    }

    private async Task OldPlaylist(string path, ExtractedVideoInfo info)
    {
        var listData = await File.ReadAllTextAsync(path);
        List<string> list = JsonConvert.DeserializeObject<List<string>>(listData);
        if (list.Contains(video.id))
        {
            LoadBar.WriteLog($"Playlist \"{Globals.BeautifyPlaylistName(path)}\" already contains video");
            Globals.activeScene.PopMenu();
            return;
        }
        list.Add(video.id);
        string listJson = JsonConvert.SerializeObject(list);
        await File.WriteAllTextAsync(path, listJson);
        Globals.activeScene.PopMenu();
        LoadBar.WriteLog($"Video saved to playlist \"{Globals.BeautifyPlaylistName(path)}\"");
        if (Globals.activeScene as PlaylistScene != null)
        {
            var rootMenu = Globals.activeScene.menus.Reverse().First();
            var affectedList = rootMenu.options.Find(i => i.option == Globals.BeautifyPlaylistName(path));
            if (affectedList == null || affectedList.childMenu == null) return;
            var video = new VideoBlock(info);
            var removeOption = new MenuOption("Remove from Playlist", video, () => PlaylistOptions.RemoveVideo(new MenuOption("", video, () => Task.Run(() => { })), affectedList.childMenu, video.videoInfo));
            removeOption.ChangeOnSelected(() => PlaylistOptions.RemoveVideo(removeOption, affectedList.childMenu, video.videoInfo));
            removeOption.extraData = path;
            video.options.Insert(3, removeOption);
            affectedList.childMenu.options.Add(new MenuOption(info.video.title, affectedList.childMenu, () => Task.Run(() => Globals.activeScene.PushMenu(video))));
        }
    }

    bool reset = false;

    protected override void OnUpdate()
    {
        if (reset)
        {
            reset = false;
            active = true;
            Reset();
        }
    }

    private async Task NewPlaylist(ExtractedVideoInfo info)
    {
        var name = Globals.ReadLineNull(selectedDrawPos.x, selectedDrawPos.y, " >  Enter the name of the playlist (leave blank to cancel): ");
        if (name == null || name == "")
        {
            reset = true;
            return;
        }
        name = name.Replace(" ", "_");
        name = Dirs.MakeFileSafe(name);
        name = Path.Combine(Dirs.playlistDir, name + ".json");
        if (File.Exists(name))
        {
            LoadBar.WriteLog("A playlist by that name already exists.");
            reset = true;
            return;
        }
        var newList = new List<string>();
        newList.Add(video.id);
        string listJson = JsonConvert.SerializeObject(newList);
        await File.WriteAllTextAsync(name, listJson);
        Globals.activeScene.PopMenu();
        LoadBar.WriteLog($"Video saved to playlist \"{Globals.BeautifyPlaylistName(name)}\"");
        if (Globals.activeScene as PlaylistScene != null)
        {
            //Manually add the ability to remove the video from the playlist
            var videoBlock = new VideoBlock(info);
            var videoTitleBlock = new MenuBlock(AnchorType.Cursor);
            videoTitleBlock.options.Add(new MenuOption(info.video.title, videoTitleBlock, () => Task.Run(() => Globals.activeScene.PushMenu(videoBlock))));

            videoTitleBlock.options[videoTitleBlock.cursor].selected = true;
            videoBlock.options[videoBlock.cursor].selected = true;

            var removeOption = new MenuOption("Remove from Playlist", videoBlock, () => PlaylistOptions.RemoveVideo(new MenuOption("", videoTitleBlock, () => Task.Run(() => { })), videoTitleBlock, info));
            removeOption.ChangeOnSelected(() => PlaylistOptions.RemoveVideo(removeOption, videoTitleBlock, videoBlock.videoInfo));
            removeOption.extraData = name;
            videoBlock.options.Insert(3, removeOption);

            Globals.activeScene.menus.Reverse().First().options.Add(new MenuOption(
                Globals.BeautifyPlaylistName(name), Globals.activeScene.menus.Reverse().First(), () => Task.Run(() => Globals.activeScene.PushMenu(videoTitleBlock)), videoTitleBlock,
                () => Task.Run(() => Globals.activeScene.PushMenu(new PlaylistOptions(name, Globals.activeScene.menus.Reverse().First())))));
        }
    }
}
