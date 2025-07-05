using Newtonsoft.Json;
using YTCons.MenuBlocks;

namespace YTCons.Scenes;

public class PlaylistScene : Scene
{
    Task gettingPlaylists;
    bool gettingPlaylistsFlag = false;

    private async Task GetPlaylists()
    {
        var lists = Directory.EnumerateFiles(Dirs.playlistDir);
        var listTasks = new List<Task>();
        foreach (string path in lists)
        {
            listTasks.Add(GetPlaylistSingular(path));
        }
        totalLists = listTasks.Count();
        await Task.WhenAll(listTasks);
    }

    int finishedLists = 0;
    int totalLists = 0;
    int finishedVideos = 0;
    int totalVideos = 0;
    private async Task GetPlaylistSingular(string path)
    {
        var listRaw = await File.ReadAllTextAsync(path);
        var playlist = JsonConvert.DeserializeObject<List<string>>(listRaw);
        var menu = new MenuBlock(AnchorType.Cursor);
        var rootMenu = menus.Reverse().First();
        List<Task> videoTasks = new();
        foreach (string videoId in playlist)
        {
            videoTasks.Add(GetVideo(videoId, menu, path));
            totalVideos++;
        }
        await Task.WhenAll(videoTasks);
        rootMenu.options.Add(new MenuOption(Path.GetFileNameWithoutExtension(path).Replace("_", " "), rootMenu, () => Task.Run(() => PushMenu(menu)), menu, () => Task.Run(() => PushMenu(new PlaylistOptions(path, rootMenu)))));
        menu.options[menu.cursor].selected = true;
        if (rootMenu.options.Count() >= 10)
        {
            rootMenu.grayUnselected = true;
        }
        finishedLists++;
    }

    private async Task GetVideo(string videoId, MenuBlock menu, string path)
    {
        var video = await VideoBlock.CreateAsync(videoId);
        var removeOption = new MenuOption("Remove from Playlist", video, () => PlaylistOptions.RemoveVideo(new MenuOption("", video, () => Task.Run(() => { })), menu, video.videoInfo));
        removeOption.ChangeOnSelected(() => PlaylistOptions.RemoveVideo(removeOption, menu, video.videoInfo));
        removeOption.extraData = path;
        video.options.Insert(3, removeOption);
        menu.options.Add(new MenuOption(video.videoInfo.video.Title, menu, () => Task.Run(() => PushMenu(video)), video));
        if (menu.options.Count() >= 10) menu.grayUnselected = true;
        finishedVideos++;
    }

    protected override void OnUpdate()
    {
        //The extra flag is so that this method doesn't monopolize the loading bar
        if (gettingPlaylists.IsCompleted && gettingPlaylistsFlag)
        {
            LoadBar.visible = false;
            gettingPlaylistsFlag = false;
        }
        else if (!gettingPlaylists.IsCompleted)
        {
            LoadBar.loadMessage = $"({finishedLists}/{totalLists} playlists, {finishedVideos}/{totalVideos} videos) Getting playlists";
        }
    }

    public PlaylistScene()
    {
        var menu = new MenuBlock();
        menu.options.Add(new MenuOption("Back", menu, () => Task.Run(() => { LoadBar.visible = false; Globals.scenes.Pop(); })));
        menu.options[menu.cursor].selected = true;
        PushMenu(menu);
        LoadBar.loadMessage = "Getting playlists";
        LoadBar.visible = true;
        gettingPlaylists = GetPlaylists();
        gettingPlaylistsFlag = true;
    }
}
