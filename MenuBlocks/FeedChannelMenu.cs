using System.Xml;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using YTCons.Scenes;

namespace YTCons.MenuBlocks;

public enum ArchiveMode
{
    Default,
    LimitAge,
    LimitNumber,
    ArchiveAll,
    DontArchive
}

public class FeedChannelMenu : MenuBlock
{
    public MenuOption? optionParent;

    public string title = "";

    public string id = "";

    public string url = "";

    private int _maxArticleAge;

    private int _maxArticleNumber;

    public int MaxArticleAge
    {
        get
        {
            if (archiveMode == ArchiveMode.Default && Globals.activeScene is FeedScene feed)
            {
                return feed.maxArticleAge;
            }
            return _maxArticleAge;
        }
    }

    public int MaxArticleAgeRaw { get => _maxArticleAge; }
    public int MaxArticleNumberRaw { get => _maxArticleNumber; }

    public int MaxArticleNumber
    {
        get
        {
            if (archiveMode == ArchiveMode.Default && Globals.activeScene is FeedScene feed)
            {
                return feed.maxArticleNumber;
            }
            return _maxArticleNumber;
        }
    }

    public bool notify = false;
    public bool lowPriority = false;

    private bool LimitNumber
    {
        get
        {
            return archiveMode == ArchiveMode.LimitNumber || (archiveMode == ArchiveMode.Default && Globals.activeScene is FeedScene feed && feed.defaultArchive == ArchiveMode.LimitNumber);
        }
    }

    private bool LimitAge
    {
        get
        {
            return archiveMode == ArchiveMode.LimitAge || (archiveMode == ArchiveMode.Default && Globals.activeScene is FeedScene feed && feed.defaultArchive == ArchiveMode.LimitAge);
        }
    }

    private bool DontArchive
    {
        get
        {
            return archiveMode == ArchiveMode.DontArchive || (archiveMode == ArchiveMode.Default && Globals.activeScene is FeedScene feed && feed.defaultArchive == ArchiveMode.DontArchive);
        }
    }

    public ArchiveMode archiveMode = ArchiveMode.Default;

    private readonly char[] caps = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

    public string PrettifyArchiveMode(ArchiveMode archiveMode, bool addNumbers = true)
    {
        switch (archiveMode)
        {
            case ArchiveMode.LimitNumber:
                var prettyNum = $"Limit Article Number";
                if (addNumbers)
                {
                    prettyNum += $" ({_maxArticleNumber})";
                }
                return prettyNum;
            case ArchiveMode.LimitAge:
                var prettyAge = $"Limit Article Age";
                if (addNumbers)
                {
                    prettyAge += $" ({_maxArticleAge} days)";
                }
                return prettyAge;
            case ArchiveMode.DontArchive:
                return "Don't Archive";
            case ArchiveMode.Default:
            case ArchiveMode.ArchiveAll:
            default:
                var pretty = "";
                var archiveString = archiveMode.ToString().ToCharArray();
                for (int i = 0; i < archiveString.Length; i++)
                {
                    if (caps.Contains(archiveString[i]) && i > 0)
                    {
                        pretty += " ";
                    }
                    pretty += archiveString[i];
                }
                return pretty;
        }
    }

    public string PrettyArchiveMode
    {
        get
        {
            return PrettifyArchiveMode(archiveMode);
        }
    }

    public class PreMenuPacket
    {
        public required string url;
        public required string title;
        public int maxArticleAge = 60;
        public int maxArticleNumber = 1000;
        public bool notify;
        public bool lowPriority = false;
        public ArchiveMode archiveMode = ArchiveMode.Default;
    }

    public static async Task<FeedChannelMenu> CreateAsync(PreMenuPacket packet)
    {
        var instance = new FeedChannelMenu()
        {
            title = packet.title,
            url = packet.url,
            _maxArticleAge = packet.maxArticleAge,
            _maxArticleNumber = packet.maxArticleNumber,
            notify = packet.notify,
            archiveMode = packet.archiveMode,
            lowPriority = packet.lowPriority
        };
        instance.id = packet.url.Replace("https://www.youtube.com/feeds/videos.xml?channel_id=", "");
        instance.CheckAnnihilation();
        List<FeedVideoOption> videos = await FeedVideoOption.CreateGroupAsync(instance, packet.url);
        foreach (FeedVideoOption video in videos)
        {
            instance.options.Add(video);
        }
        var files = Directory.EnumerateFiles(Path.Combine(Dirs.feedsDir, instance.id));
        foreach (string filePath in files)
        {
            if (instance.options.Find(i => i is FeedVideoOption option && option.feedData.id == Path.GetFileNameWithoutExtension(filePath)) == null)
            {
                if (instance.DontArchive)
                {
                    File.Delete(filePath);
                }
                else
                {
                    instance.options.Add(new FeedVideoOption(instance, filePath));
                }
            }
        }
        instance.options.Sort((left, right) =>
        {
            var l = (FeedVideoOption)left;
            var r = (FeedVideoOption)right;

            return r.feedData.published.CompareTo(l.feedData.published);
        });
        instance.grayUnselected = true;
        return instance;
    }

    private FeedChannelMenu(AnchorType anchorType = AnchorType.Cursor) : base(anchorType) { }

    public void CheckUnreadOptions()
    {
        if (optionParent == null) return;
        foreach (MenuOption option in options)
        {
            if (option is FeedVideoOption vidOption && !vidOption.feedData.read)
            {
                optionParent.counter++;
            }
        }
    }

    public void DecrementUnread()
    {
        if (optionParent == null) return;
        optionParent.counter--;
    }

    public void MarkAllAsRead()
    {
        foreach (MenuOption option in options)
        {
            if (option is FeedVideoOption vidOption && !vidOption.feedData.read)
            {
                vidOption.feedData.read = true;
                vidOption.option = vidOption.feedData.title;
                var feedJson = JsonConvert.SerializeObject(vidOption.feedData);
                File.WriteAllText(vidOption.GetJsonPath(), feedJson);
            }
        }
        if (optionParent != null)
        {
            optionParent.counter = 0;
        }
        Globals.activeScene.PopMenu();
    }

    public void SetArchiveMode(ArchiveMode archiveMode, MenuBlock inputPlacement, MenuOption display)
    {
        this.archiveMode = archiveMode;
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
                    _maxArticleAge = num;
                    break;
                case ArchiveMode.LimitNumber:
                    _maxArticleNumber = num;
                    break;
            }
        }
        if (Globals.activeScene is FeedScene feed)
        {
            feed.SaveAsXml();
        }
        display.option = $"Switch Archive Mode (current: {PrettyArchiveMode})";
        Globals.activeScene.PopMenu();
    }

    public void ChangePriority(MenuOption display)
    {
        lowPriority = !lowPriority;
        if (optionParent != null) optionParent.useCounter = !lowPriority;
        foreach (MenuOption option in options)
        {
            if (option is FeedVideoOption video)
            {
                video.UpdateReadStatus();
            }
        }
        display.option = $"Switch Priority (current: {(lowPriority ? "Low" : "Normal")})";
        if (Globals.activeScene is FeedScene feed)
        {
            feed.SaveAsXml();
        }
    }

    private void CheckAnnihilation()
    {
        var files = Directory.EnumerateFiles(Path.Combine(Dirs.feedsDir, id)).ToList();
        files.Sort((left, right) =>
        {
            var lTime = File.GetCreationTime(left);
            var rTime = File.GetCreationTime(right);
            return rTime.CompareTo(lTime);
        });
        int deleted = 0;
        for (int i = 0; i < files.Count(); i++)
        {
            if (LimitAge && DateTime.Now - File.GetCreationTime(files[i]) > TimeSpan.FromDays(MaxArticleAge))
            {
                File.Delete(files[i]);
                deleted++;
            }
            if (LimitNumber && i - deleted >= MaxArticleNumber)
            {
                File.Delete(files[i]);
                deleted++;
            }
        }
    }

    public void MoveToFolder(MenuOption parentOption, FolderMenu rootFolder)
    {
        var menu = rootFolder.RecursiveFolderChoiceMenu(this, parentOption);
        Globals.activeScene.PushMenu(menu);
    }


    public void MakeNewFolder(FolderMenu parentFolder, MenuOption parentOption, int depth)
    {
        var newFolder = FolderMenu.NewFolder(parentFolder);
        if (newFolder.aborted)
        {
            return;
        }
        MoveToFolderDirect(newFolder.newFolder, parentOption, depth);
    }

    public void MoveToFolderDirect(FolderMenu folder, MenuOption parentOption, int depth)
    {
        parentOption.parent.options.Remove(parentOption);
        if (parentOption.parent is FolderMenu parentFolder)
        {
            parentFolder.menus.Remove(this);
            if (parentFolder.options.Count() <= 1)
            {
                parentFolder.Remove(false);
            }
        }
        parentOption.parent = folder;
        folder.menus.Add(this);
        folder.options.Add(parentOption);
        for (int i = 0; i <= depth; i++)
        {
            Globals.activeScene.PopMenu();
        }
        var newCurMenu = Globals.activeScene.PeekMenu();
        if (newCurMenu.cursor >= newCurMenu.options.Count())
        {
            newCurMenu.cursor = 0;
        }
        if (Globals.activeScene is FeedScene feed)
        {
            feed.SaveAsXml();
        }
        newCurMenu.options[newCurMenu.cursor].selected = true;
        parentOption.selected = false;
    }
}

public class FeedChannelMenuAlt : MenuBlock
{
    private bool queueReset = false;

    public FeedChannelMenuAlt(FeedChannelMenu original, MenuOption parentOption, FolderMenu rootFolder, AnchorType anchorType = AnchorType.Cursor) : base(anchorType)
    {
        options.Add(new MenuOption("Mark All as Read", this, () => Task.Run(() => original.MarkAllAsRead())));

        var priorityOption = new MenuOption($"Switch Priority (current: {(original.lowPriority ? "Low" : "Normal")})", this, () => Task.Run(() => { }));
        priorityOption.ChangeOnSelected(() => Task.Run(() => { queueReset = true; original.ChangePriority(priorityOption); }));
        priorityOption.tip = "Normal priority displays which articles are unread, low priority doesn't. Meant to reduce clutter from feeds that post frequently.";
        options.Add(priorityOption);

        var setArchiveOption = new MenuOption($"Switch Archive Mode (current: {original.PrettyArchiveMode})", this, () => Task.Run(() => { }));
        var archiveModeMenu = SetArchiveModeMenu(original, setArchiveOption);
        setArchiveOption.ChangeOnSelected(() => Task.Run(() => Globals.activeScene.PushMenu(archiveModeMenu)));
        options.Add(setArchiveOption);

        options.Add(new MenuOption("Move to Folder", this, () => Task.Run(() => original.MoveToFolder(parentOption, rootFolder))));

        options[cursor].selected = true;
    }

    private MenuBlock SetArchiveModeMenu(FeedChannelMenu original, MenuOption parentOption)
    {
        var menu = new MenuBlock(AnchorType.Cursor);
        parentOption.childMenu = menu;
        foreach (ArchiveMode mode in Enum.GetValues(typeof(ArchiveMode)))
        {
            var option = new MenuOption(original.PrettifyArchiveMode(mode, false), menu, () => Task.Run(() => original.SetArchiveMode(mode, menu, parentOption)));
            switch (mode)
            {
                case ArchiveMode.Default:
                    option.tip = "Uses the archive default defined in global feed settings";
                    break;
                case ArchiveMode.ArchiveAll:
                    option.tip = "Preserves all feed entries indefinently";
                    break;
                case ArchiveMode.DontArchive:
                    option.tip = "No entries that aren't included in the feed URL are preserved";
                    break;
                case ArchiveMode.LimitAge:
                    option.tip = "Removes entries that are older than a set amount of days";
                    break;
                case ArchiveMode.LimitNumber:
                    option.tip = "Removes the oldest entries after the number of entries exceeds a predefined capacity";
                    break;
            }
            menu.options.Add(option);
        }
        menu.options[menu.cursor].selected = true;
        return menu;
    }

    protected override void OnUpdate()
    {
        if (queueReset)
        {
            queueReset = false;
            Reset();
        }
    }
}
