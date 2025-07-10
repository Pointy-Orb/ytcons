using System.Xml;
using YTCons.MenuBlocks;
using Newtonsoft.Json;
using System.Collections.Concurrent;


namespace YTCons.Scenes;

public class FeedScene : Scene
{
    private FeedScene() { }

    private List<MenuBlock> savedMenus = new();

    public class FeedSettings
    {
        public ArchiveMode defaultArchive = ArchiveMode.ArchiveAll;
        public int maxArticleAge = 60;
        public int maxArticleNumber = 1000;
    }

    public FeedSettings feedSettings = new();

    public FolderMenu root = new FolderMenu("root", null, AnchorType.Center);

    public static async Task<FeedScene> CreateAsync(bool promptNewFile = false, string? importFeedPath = null)
    {
        var instance = new FeedScene();
        string path = Path.Combine(Dirs.feedsDir, "feeds.opml");
        string? otherPath = importFeedPath;
        bool gotFeedsFromFile = false;
        if (!File.Exists(path) && !File.Exists(Path.Combine(Dirs.feedsDir, "feedsPending.json")))
        {
            path = Globals.ReadLine(0, 0, "Enter the path of the opml file you want to import: ");
            while (!File.Exists(path) && !path.EndsWith(".opml"))
            {
                path = Globals.ReadLine(0, 0, "Enter the path of the opml file you want to import: ");
            }
            gotFeedsFromFile = true;
        }
        else if (promptNewFile)
        {
            otherPath = Globals.ReadLineNull(0, 0, "Enter the path of the opml file you want to import: ");
            while (!String.IsNullOrEmpty(otherPath) && !File.Exists(otherPath) && !otherPath.EndsWith(".opml"))
            {
                otherPath = Globals.ReadLineNull(0, 0, "Enter the path of the opml file you want to import: ");
                if (String.IsNullOrEmpty(otherPath))
                {
                    otherPath = null;
                    break;
                }
            }
        }
        if (File.Exists(Path.Combine(Dirs.feedsDir, "feedSettings.json")))
        {
            var settingsJson = await File.ReadAllTextAsync(Path.Combine(Dirs.feedsDir, "feedSettings.json"));
            instance.feedSettings = JsonConvert.DeserializeObject<FeedSettings>(settingsJson);
        }
        List<Task> menuTasks = new();
        FolderMenu curFolder = instance.root;
        LoadBar.loadMessage = "Fetching feeds";
        LoadBar.StartLoad();
        if (File.Exists(path))
        {
            using var stream = File.OpenRead(path);
            using var xmlStream = XmlReader.Create(stream);
            while (xmlStream.Read())
            {
                if (xmlStream.NodeType == XmlNodeType.Element && xmlStream.GetAttribute("text") != null && xmlStream.GetAttribute("xmlUrl") == null)
                {
                    var folder = new FolderMenu(xmlStream.GetAttribute("text")!, curFolder);
                    curFolder.subFolders.Add(folder);
                    curFolder = folder;
                    continue;
                }
                if (xmlStream.NodeType == XmlNodeType.EndElement && xmlStream.Name == "outline")
                {
                    if (curFolder.parent != null)
                    {
                        curFolder = curFolder.parent;
                    }
                }
                curFolder.CheckForChannels(xmlStream, menuTasks, gotFeedsFromFile);
            }
        }
        if (File.Exists(otherPath))
        {
            using var stream = File.OpenRead(otherPath);
            using var xmlStream = XmlReader.Create(stream);
            var importRoot = new FolderMenu(Path.GetFileNameWithoutExtension(otherPath), instance.root);
            instance.root.subFolders.Add(importRoot);
            curFolder = importRoot;
            while (xmlStream.Read())
            {
                if (xmlStream.NodeType == XmlNodeType.Element && xmlStream.GetAttribute("text") != null && xmlStream.GetAttribute("xmlUrl") == null)
                {
                    var folder = new FolderMenu(xmlStream.GetAttribute("text")!, curFolder);
                    curFolder.subFolders.Add(folder);
                    curFolder = folder;
                    continue;
                }
                if (xmlStream.NodeType == XmlNodeType.EndElement && xmlStream.Name == "outline")
                {
                    if (curFolder.parent != null)
                    {
                        curFolder = curFolder.parent;
                    }
                }
                curFolder.CheckForChannels(xmlStream, menuTasks, true);
            }
        }
        if (File.Exists(Path.Combine(Dirs.feedsDir, "feedsPending.json")))
        {
            var queueJson = await File.ReadAllTextAsync(Path.Combine(Dirs.feedsDir, "feedsPending.json"));
            var pendingFeeds = JsonConvert.DeserializeObject<List<(string title, string id)>>(queueJson);
            if (pendingFeeds == null)
            {
                pendingFeeds = new();
            }
            foreach ((string title, string id) feed in pendingFeeds)
            {
                var packet = new FeedChannelMenu.PreMenuPacket
                {
                    title = feed.title,
                    url = "https://www.youtube.com/feeds/videos.xml?channel_id=" + feed.id
                };
                menuTasks.Add(MakeFeedMenuAsync(instance.root, instance.root.menuBag, packet, true));
            }
            File.Delete(Path.Combine(Dirs.feedsDir, "feedsPending.json"));
        }
        await Task.WhenAll(menuTasks);
        instance.root.RecursiveFolderAdding();
        instance.root.options.Sort((left, right) => string.Compare(left.option, right.option));
        instance.root.options.Insert(0, new MenuOption("Settings", instance.root, () => Task.Run(() => Globals.activeScene.PushMenu(instance.root.RootSettingsMenu(instance)))));
        instance.root.options.Insert(0, new MenuOption("Back", instance.root, () => Task.Run(() =>
                        { LoadBar.visible = false; Globals.scenes.Pop(); })));
        instance.root.options[instance.root.cursor].selected = true;
        instance.PushMenu(instance.root);
        instance.SaveAsXml();
        LoadBar.visible = false;
        LoadBar.ClearLoad();
        return instance;
    }

    public void SetDefaultArchiveMode(ArchiveMode archiveMode, MenuBlock inputPlacement, MenuOption display)
    {
        feedSettings.defaultArchive = archiveMode;
        if (archiveMode == ArchiveMode.LimitAge || archiveMode == ArchiveMode.LimitNumber)
        {
            int num = 0;
            while (num <= 0)
            {
                var selDrawPos = inputPlacement.selectedDrawPos;
                var isANumber = int.TryParse(Globals.ReadLine(selDrawPos.x, selDrawPos.y, $" > Enter the maximum article {(archiveMode == ArchiveMode.LimitAge ? "age in days" : "number")}: "), out var test);
                if (isANumber)
                {
                    num = test;
                }
            }
            switch (archiveMode)
            {
                case ArchiveMode.LimitAge:
                    feedSettings.maxArticleAge = num;
                    break;
                case ArchiveMode.LimitNumber:
                    feedSettings.maxArticleNumber = num;
                    break;
            }
        }
        SaveFeedSettings();
        display.option = $"Switch Default Archive Mode (current: {FeedChannelMenu.PrettifyArchiveModeStatic(archiveMode, true, feedSettings.maxArticleNumber, feedSettings.maxArticleAge)})";
        Globals.activeScene.PopMenu();
    }

    public void SaveFeedSettings()
    {
        var settingsJson = JsonConvert.SerializeObject(feedSettings);
        File.WriteAllText(Path.Combine(Dirs.feedsDir, "feedSettings.json"), settingsJson);
    }

    public static async Task MakeFeedMenuAsync(FolderMenu rootFolder, ConcurrentBag<FeedChannelMenu> bag, FeedChannelMenu.PreMenuPacket packet, bool checkDuplicates = false)
    {
        var feedMenu = await FeedChannelMenu.CreateAsync(packet);
        if (checkDuplicates && rootFolder.ContainsMenu(feedMenu))
        {
            return;
        }
        if (feedMenu.options.Count > 0)
        {
            bag.Add(feedMenu);
        }
    }

    public void SaveAsXml()
    {
        root.SaveAsXml();
    }
}

