using YTCons.MenuBlocks;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Xml;
using System.Collections.Concurrent;

namespace YTCons.Scenes;

public class FolderMenu : MenuBlock
{
    public string title;

    public List<FolderMenu> subFolders = new();
    public ConcurrentBag<FeedChannelMenu> menuBag = new();
    public List<FeedChannelMenu> menus = new();
    public FolderMenu? parent = null;

    public class FolderOption : MenuOption
    {
        public override int counter
        {
            get
            {
                if (childMenu is FolderMenu folder)
                {
                    int count = 0;
                    foreach (MenuOption option in folder.options)
                    {
                        //Some of the options are menus that display all the feeds, adding those to the counter would be stupid.
                        if ((option.childMenu is FeedChannelMenu menu || option.childMenu is FolderMenu child) && option.useCounter)
                        {
                            count += option.counter;
                        }
                    }
                    return count;
                }
                return base.counter;
            }
            set
            { }
        }

        public FolderOption(string option, FolderMenu parent, FolderMenu child, Func<Task> onSelected, Func<Task>? altOnselected = null) : base(option, parent, onSelected, altOnselected)
        {
            this.childMenu = child;
        }

        protected override void PostDraw(int i, int j)
        {
            int selectedOffset = selected && parent.confirmed && parent.active ? 2 : 0;

            int digits = MenuOption.DivideWithPowers(counter, 10) + 1;

            int useCounterOffset = counter == 0 ? 0 : 4;
            for (int l = i + 4 + useCounterOffset + selectedOffset + digits; l < i + 10 + useCounterOffset + digits + selectedOffset; l++)
            {
                Globals.SetForegroundColor(l, j, ConsoleColor.Blue);
            }
        }
    }

    public FolderMenu(string title, FolderMenu? parent, AnchorType anchorType = AnchorType.Cursor) : base(anchorType)
    {
        this.title = title;
        this.parent = parent;
    }

    public void CheckForChannels(XmlReader xmlStream, List<Task> menuTasks, bool checkDuplicates = false)
    {
        if (xmlStream.NodeType != XmlNodeType.Element) return;
        if (!xmlStream.HasAttributes) return;
        if (xmlStream.GetAttribute("xmlUrl") == null) return;
        if (!xmlStream.GetAttribute("xmlUrl").Contains("https://www.youtube.com/feeds")) return;
        var packet = new FeedChannelMenu.PreMenuPacket()
        {
            url = xmlStream.GetAttribute("xmlUrl")!,
            title = xmlStream.GetAttribute("text"),
            notify = xmlStream.GetAttribute("useNotification") == "true",
            lowPriority = xmlStream.GetAttribute("markImmediatelyAsRead") == "true"
        };
        if (xmlStream.GetAttribute("maxArticleAge") != null)
        {
            packet.maxArticleAge = Convert.ToInt32(xmlStream.GetAttribute("maxArticleAge"));
        }
        if (xmlStream.GetAttribute("maxArticleNumber") != null)
        {
            packet.maxArticleNumber = Convert.ToInt32(xmlStream.GetAttribute("maxArticleNumber"));
        }
        if (xmlStream.GetAttribute("archiveMode") != null)
        {
            string archiveMode = xmlStream.GetAttribute("archiveMode")!;
            switch (archiveMode)
            {
                case "limitArticleNumber":
                    packet.archiveMode = ArchiveMode.LimitNumber;
                    break;
                case "limitArticleAge":
                    packet.archiveMode = ArchiveMode.LimitAge;
                    break;
                case "keepAllArticles":
                    packet.archiveMode = ArchiveMode.ArchiveAll;
                    break;
                case "disableArchiving":
                    packet.archiveMode = ArchiveMode.DontArchive;
                    break;
            }
        }
        if (xmlStream.GetAttribute("shortsStatus") != null)
        {
            switch (xmlStream.GetAttribute("shortsStatus"))
            {
                case "shortsOnly":
                    packet.shortsStatus = ShortsStatus.ShortsOnly;
                    break;
                case "fullVideosOnly":
                    packet.shortsStatus = ShortsStatus.FullVideosOnly;
                    break;
            }
        }
        menuTasks.Add(FeedScene.MakeFeedMenuAsync(root, menuBag, packet, checkDuplicates));
    }

    public FolderMenu root
    {
        get
        {
            var rootV = this;
            while (rootV.parent != null)
            {
                rootV = rootV.parent;
            }
            return rootV;
        }
    }

    public FolderOption? parentOption { get; private set; } = null;

    public void BagToList()
    {
        menus = menuBag.ToList();
        foreach (FolderMenu subFolder in subFolders)
        {
            subFolder.BagToList();
        }
    }

    public void RecursiveFolderAdding()
    {
        BagToList();
        foreach (FeedChannelMenu menu in menus)
        {
            var option = new MenuOption(menu.title, this,
                    () => Task.Run(() => Globals.activeScene.PushMenu(menu)),
                    () => Task.Run(() => { }));
            option.ChangeAltOnSelected(() => Task.Run(() => Globals.activeScene.PushMenu(new FeedChannelMenuAlt(menu, option, root))));
            option.useCounter = !menu.lowPriority;
            option.childMenu = menu;
            option.tip = "Press Enter for feed options.";
            menu.optionParent = option;
            menu.CheckUnreadOptions();
            options.Add(option);
        }
        foreach (FolderMenu subFolder in subFolders)
        {
            subFolder.RecursiveFolderAdding();
            if (subFolder.options.Count() <= 0) continue;

            if (subFolder.options.Count() > 7) subFolder.grayUnselected = true;
            subFolder.options.Sort((left, right) => string.Compare(left.option, right.option));
            subFolder.options[subFolder.cursor].selected = true;

            var option = new FolderOption("[folder]/" + subFolder.title, this, subFolder, () => Task.Run(() => Globals.activeScene.PushMenu(subFolder)), () => Task.Run(() => { }));
            var subFolderAlt = new FolderMenuAlt(subFolder, option);
            option.ChangeAltOnSelected(() => Task.Run(() => Globals.activeScene.PushMenu(subFolderAlt)));
            subFolder.parentOption = option;
            option.tip = "Press Enter for folder options";
            option.useCounter = true;
            options.Add(option);
        }
        options.Add(new MenuOption("[All Feeds]", this, () => Task.Run(() => Globals.activeScene.PushMenu(AllFeeds()))));
    }

    private MenuBlock AllFeeds()
    {
        var menu = new MenuBlock(AnchorType.Cursor);
        menu.options = RecursiveOptionList();
        menu.options.Sort((left, right) =>
        {
            var l = (FeedVideoOption)left;
            var r = (FeedVideoOption)right;

            return r.feedData.published.CompareTo(l.feedData.published);
        });
        return menu;
    }

    private List<MenuOption> RecursiveOptionList()
    {
        var list = new List<MenuOption>();
        foreach (FolderMenu subFolder in subFolders)
        {
            var childList = subFolder.RecursiveOptionList();
            list.AddRange(childList);
        }
        foreach (FeedChannelMenu feed in menus)
        {
            list.AddRange(feed.options);
        }
        return list;
    }

    public void SaveAsXml()
    {
        var settings = new XmlWriterSettings
        {
            Indent = true
        };
        settings.ConformanceLevel = ConformanceLevel.Document;
        using var xmlFile = File.Create(Path.Combine(Dirs.feedsDir, "feeds.opml"));
        using var writer = XmlWriter.Create(xmlFile, settings);
        WriteHeader(writer);
        WriteBody(writer, false);
        WriteFooter(writer);
    }

    private void WriteBody(XmlWriter writer, bool writeOutlines = true)
    {
        if (writeOutlines)
        {
            writer.WriteStartElement("outline");
            writer.WriteAttributeString("text", title);
        }

        foreach (FolderMenu subFolder in subFolders)
        {
            if (subFolder.options.Count() > 0)
            {
                subFolder.WriteBody(writer);
            }
        }
        foreach (FeedChannelMenu menu in menus)
        {
            writer.WriteStartElement("outline");
            writer.WriteAttributeString("text", menu.title);
            //writer.WriteAttributeString("title",menu.title);
            writer.WriteAttributeString("xmlUrl", menu.url);
            writer.WriteAttributeString("htmlUrl", $"https://www.youtube.com/channel/{menu.id}");
            writer.WriteAttributeString("archiveMode", XmlArchiveMode(menu.archiveMode));
            writer.WriteAttributeString("maxArticleAge", menu.MaxArticleAgeRaw.ToString());
            writer.WriteAttributeString("markImmediatelyAsRead", menu.lowPriority ? "true" : "false");
            writer.WriteAttributeString("maxArticleNumber", menu.MaxArticleNumberRaw.ToString());
            if (menu.shortsStatus != ShortsStatus.Unified)
            {
                writer.WriteAttributeString("shortsStatus", Globals.ToCamelCase(menu.shortsStatus.ToString()));
            }
            writer.WriteAttributeString("type", "rss");
            writer.WriteAttributeString("version", "RSS");
            writer.WriteEndElement();
        }

        if (writeOutlines) writer.WriteEndElement();
    }

    private string XmlArchiveMode(ArchiveMode archiveMode)
    {
        switch (archiveMode)
        {
            case ArchiveMode.LimitAge:
                return "limitArticleAge";
            case ArchiveMode.DontArchive:
                return "disableArchiving";
            case ArchiveMode.ArchiveAll:
                return "keepAllArticles";
            case ArchiveMode.LimitNumber:
                return "limitArticleNumber";
            case ArchiveMode.Default:
            default:
                return "globalDefault";
        }
    }

    private void WriteHeader(XmlWriter writer)
    {
        writer.WriteStartElement("opml");
        writer.WriteAttributeString("version", "1.0");
        writer.WriteStartElement("head");
        writer.WriteStartElement("text");
        writer.WriteEndElement();
        writer.WriteEndElement();
        writer.WriteStartElement("body");
    }

    private void WriteFooter(XmlWriter writer)
    {
        writer.WriteEndElement();
        writer.WriteEndElement();
    }

    public void Rename(MenuBlock selDrawPosBlock)
    {
        var selDrawPos = selDrawPosBlock.selectedDrawPos;
        var newName = Globals.ReadLineNull(selDrawPos.x, selDrawPos.y, " >  Enter the new name for the folder: ");
        if (newName == null || newName == "")
        {
            selDrawPosBlock.resetNextTick = true;
            return;
        }
        title = newName;
        if (parent != null)
        {
            var parentOption = parent.options.Find(i => i is FolderOption folder && folder.childMenu is FolderMenu folderMenu && folderMenu.title == title);
            if (parentOption != null)
            {
                parentOption.option = "[folder]/" + title;
            }
        }
        root.SaveAsXml();
        Globals.activeScene.PopMenu();
    }

    public void MarkAllAsRead(bool calledByItself = false)
    {
        foreach (FolderMenu subFolder in subFolders)
        {
            subFolder.MarkAllAsRead(true);
        }
        foreach (FeedChannelMenu menu in menus)
        {
            menu.MarkAllAsRead();
        }
        if (!calledByItself)
        {
            Globals.activeScene.PopMenu();
        }
    }

    public void Remove(bool writeChange = true)
    {
        if (parent == null)
        {
            return;
        }
        parent.subFolders.Remove(this);
        var parentFolderOption = parent.options.Find(i => i is FolderMenu.FolderOption folderOption && folderOption.childMenu == this);
        if (parentFolderOption != null)
        {
            parent.options.Remove(parentFolderOption);
            Globals.activeScene.PopMenu();
        }
        if (writeChange)
        {
            root.SaveAsXml();
        }
    }

    public void MoveFolder()
    {
        var menu = root.RecursiveFolderChoiceMenu(this, parent.options.Find(i => i is FolderOption folder && folder.selected));
        Globals.activeScene.PushMenu(menu);
    }

    public void MoveContents()
    {
        var menu = root.RecursiveMoveContentsMenu(this, parent.options.Find(i => i is FolderOption folder && folder.selected));
        Globals.activeScene.PushMenu(menu);
    }

    private enum FolderChoiceMenuPurpose
    {
        MoveFeedChannel,
        MoveFolder,
        MoveFolderContents
    }

    public MenuBlock RecursiveFolderChoiceMenu(FolderMenu folderToMove, MenuOption parentOption)
    {
        return _RecursiveFolderChoiceMenu(parentOption, FolderChoiceMenuPurpose.MoveFolder, folderToMove: folderToMove).menu;
    }

    public MenuBlock RecursiveFolderChoiceMenu(FeedChannelMenu feedMenu, MenuOption parentOption)
    {
        return _RecursiveFolderChoiceMenu(parentOption, FolderChoiceMenuPurpose.MoveFeedChannel, feedMenu: feedMenu).menu;
    }

    public MenuBlock RecursiveMoveContentsMenu(FolderMenu folderToMove, MenuOption parentOption)
    {
        return _RecursiveFolderChoiceMenu(parentOption, FolderChoiceMenuPurpose.MoveFolderContents, folderToMove: folderToMove).menu;
    }

    private (MenuBlock menu, string title) _RecursiveFolderChoiceMenu(MenuOption parentOption, FolderChoiceMenuPurpose purpose, int depth = 1, FolderMenu? folderToMove = null, FeedChannelMenu? feedMenu = null)
    {
        var menu = new MenuBlock(AnchorType.Cursor);
        List<(MenuBlock menu, string title)> subMenus = new();
        foreach (FolderMenu subFolder in subFolders)
        {
            if (subFolder == folderToMove)
            {
                continue;
            }
            var subMenu = subFolder._RecursiveFolderChoiceMenu(parentOption, purpose, depth + 1, folderToMove, feedMenu);
            subMenus.Add(subMenu);
        }
        var clariflyRoot = this.Equals(root) ? " (no folder)" : "";
        Func<Task> move = () => Task.Run(() => { });
        Func<Task> newFolder = () => Task.Run(() => { });
        switch (purpose)
        {
            case FolderChoiceMenuPurpose.MoveFeedChannel:
                move = () => Task.Run(() => feedMenu.MoveToFolderDirect(this, parentOption, depth));
                newFolder = () => Task.Run(() => feedMenu.MakeNewFolder(this, parentOption, depth));
                break;
            case FolderChoiceMenuPurpose.MoveFolder:
                move = () => Task.Run(() => folderToMove.MoveToOtherFolder(this, parentOption, depth));
                newFolder = () => Task.Run(() => folderToMove.NewFolderThenMoveToIt(this, parentOption, depth));
                break;
            case FolderChoiceMenuPurpose.MoveFolderContents:
                move = () => Task.Run(() => folderToMove._MoveContents(this, parentOption, depth));
                newFolder = () => Task.Run(() => folderToMove.NewFolderThenMoveToIt(this, parentOption, depth));
                break;
        }
        if (feedMenu == null || !menus.Contains(feedMenu))
        {
            menu.options.Add(new MenuOption("Place Here" + clariflyRoot, menu, move));
        }
        menu.options.Add(new MenuOption("Make Folder Here", menu, newFolder));
        foreach ((MenuBlock menu, string title) subMenu in subMenus)
        {
            menu.options.Add(new MenuOption(subMenu.title, menu, () => Task.Run(() => Globals.activeScene.PushMenu(subMenu.menu))));
        }
        return (menu, title);
    }

    private void NewFolderThenMoveContents(FolderMenu parentFolder, MenuOption parentOption, int depth)
    {
        var newFolder = NewFolder(parentFolder);
        if (newFolder.aborted)
        {
            return;
        }
        _MoveContents(newFolder.newFolder, parentOption, depth);
    }

    private void _MoveContents(FolderMenu target, MenuOption parentOption, int depth)
    {
        for (int i = menus.Count - 1; i >= 0; i--)
        {
            if (menus[i].optionParent != null)
            {
                options.Remove(menus[i].optionParent!);
                target.options.Add(menus[i].optionParent!);
            }
            target.menus.Add(menus[i]);
            menus.RemoveAt(i);
        }
        for (int i = subFolders.Count - 1; i >= 0; i--)
        {
            if (subFolders[i].parentOption != null)
            {
                options.Remove(subFolders[i].parentOption!);
                target.options.Add(subFolders[i].parentOption!);
                target.subFolders.Add(subFolders[i]);
                subFolders.RemoveAt(i);
            }
        }
        Remove();
        Globals.activeScene.PopMenu();
    }

    private void NewFolderThenMoveToIt(FolderMenu parentFolder, MenuOption parentOption, int depth)
    {
        var newFolder = NewFolder(parentFolder);
        if (newFolder.aborted)
        {
            return;
        }
        MoveToOtherFolder(newFolder.newFolder, parentOption, depth);
    }

    public bool ContainsMenu(FeedChannelMenu item)
    {
        bool contains = false;
        foreach (FolderMenu subFolder in subFolders)
        {
            contains = subFolder.ContainsMenu(item);
            if (contains)
            {
                return contains;
            }
        }
        return menuBag.ToList().Find(i => i.id == item.id && i.shortsStatus == item.shortsStatus) != null;
    }

    public static (FolderMenu newFolder, bool aborted) NewFolder(FolderMenu parentFolder)
    {
        var selDrawPos = Globals.activeScene.PeekMenu().selectedDrawPos;
        string? folderName = Globals.ReadLineNull(selDrawPos.x, selDrawPos.y, " >  Enter the name of the new folder: ");
        if (folderName == null || folderName == "")
        {
            Globals.activeScene.PeekMenu().resetNextTick = true;
            return (new FolderMenu("", parentFolder), true);
        }
        var newFolder = new FolderMenu(folderName, parentFolder);
        parentFolder.subFolders.Add(newFolder);
        var option = new FolderMenu.FolderOption("[folder]/" + newFolder.title, parentFolder, newFolder, () => Task.Run(() => Globals.activeScene.PushMenu(newFolder)), () => Task.Run(() => { }));
        var newFolderAlt = new FolderMenuAlt(newFolder, option);
        option.ChangeAltOnSelected(() => Task.Run(() => Globals.activeScene.PushMenu(newFolderAlt)));
        option.useCounter = true;
        parentFolder.options.Add(option);
        return (newFolder, false);
    }

    private void MoveToOtherFolder(FolderMenu target, MenuOption parentOption, int depth)
    {
        if (parentOption.parent is FolderMenu parentFolder)
        {
            parentFolder.subFolders.Remove(this);
            target.subFolders.Add(this);
            if (parentFolder.options.Count < 1)
            {
                parentFolder.Remove(false);
            }
        }
        this.parent = target;
        parentOption.selected = false;
        parentOption.parent.options.Remove(parentOption);
        if (parentOption.parent.cursor >= parentOption.parent.options.Count)
        {
            parentOption.parent.cursor = 0;
        }
        if (parentOption.parent.options.Count > 1)
        {
            parentOption.parent.options[parentOption.parent.cursor].selected = true;
        }

        target.options.Add(parentOption);
        parentOption.parent = target;
        target.options.Sort((left, right) =>
        {
            if (left.option == "Back" && right.option != "Back") return -1;
            if (right.option == "Back" && left.option != "Back") return 1;
            if (left.option == "Settings" && right.option != "Settings") return -1;
            if (right.option == "Settings" && left.option != "Settings") return 1;

            return String.Compare(left.option, right.option);
        });
        for (int i = 0; i <= depth; i++)
        {
            Globals.activeScene.PopMenu();
        }
        root.SaveAsXml();
    }

    private async Task ImportFeed(MenuBlock parentMenu)
    {
        parentMenu.resetNextTick = true;
        var selDrawPos = parentMenu.selectedDrawPos;
        string? otherPath = Globals.ReadLineNull(selDrawPos.x, selDrawPos.y, " >  Enter the path of the opml file you want to import: ");
        while (!String.IsNullOrEmpty(otherPath) && !File.Exists(otherPath) && !otherPath.EndsWith(".opml"))
        {
            otherPath = Globals.ReadLineNull(selDrawPos.x, selDrawPos.y, " >  Enter the path of the opml file you want to import: ");
        }
        if (String.IsNullOrEmpty(otherPath))
        {
            otherPath = null;
            return;
        }
        await ReFetchFeeds(otherPath);
    }

    public MenuBlock RootSettingsMenu(FeedScene scene)
    {
        var menu = new MenuBlock(AnchorType.Cursor);
        menu.options.Add(new MenuOption("Mark All as Read", this, () => Task.Run(() => MarkAllAsRead())));

        menu.options.Add(new MenuOption("Fetch All Feeds", this, () => ReFetchFeeds()));

        menu.options.Add(new MenuOption("Import feed from OPML", menu, () => ImportFeed(menu)));

        menu.options.Add(new MenuOption("Add channel from handle", menu, () => AddChannelFromHandle(menu)));

        var setArchiveOption = new MenuOption($"Switch Default Archive Mode (current: {FeedChannelMenu.PrettifyArchiveModeStatic(scene.feedSettings.defaultArchive, true, scene.feedSettings.maxArticleNumber, scene.feedSettings.maxArticleAge)})", this, () => Task.Run(() => { }));
        var archiveModeMenu = SetArchiveModeMenu(scene, setArchiveOption);
        setArchiveOption.ChangeOnSelected(() => Task.Run(() => Globals.activeScene.PushMenu(archiveModeMenu)));
        menu.options.Add(setArchiveOption);

        return menu;
    }

    private async Task AddChannelFromHandle(MenuBlock display)
    {
        display.resetNextTick = true;
        var selDrawPos = display.selectedDrawPos;
        var handle = Globals.ReadLineNull(selDrawPos.x, selDrawPos.y, " > Enter the handle of the channel you want to import: @");
        if (handle == null)
        {
            return;
        }
        using (var client = new HttpClient())
        {
            using var response = await client.GetAsync("https://youtube.com/@" + handle);
            if (!response.IsSuccessStatusCode)
            {
                LoadBar.WriteLog("Invalid handle");
                return;
            }
            var dataString = await client.GetStringAsync("https://youtube.com/@" + handle);
            var channelTitle = Regex.Match(dataString, @"\<meta itemprop=""name"" content=""(.*?)""\>").Groups[1].Value;
            var channelId = Regex.Match(dataString, "channel_id=([^\"]+)").Groups[1].Value;
            List<(string title, string id)> feedQueue = new();
            if (File.Exists(Path.Combine(Dirs.feedsDir, "feedsPending.json")))
            {
                string queueJson = File.ReadAllText(Path.Combine(Dirs.feedsDir, "feedsPending.json"));
                feedQueue = JsonConvert.DeserializeObject<List<(string title, string id)>>(queueJson);
            }
            feedQueue.Add((channelTitle, channelId));
            string queueOutJson = JsonConvert.SerializeObject(feedQueue);
            File.WriteAllText(Path.Combine(Dirs.feedsDir, "feedsPending.json"), queueOutJson);
        }
        await ReFetchFeeds();
    }

    private async Task ReFetchFeeds(string? otherPath = null)
    {
        SaveAsXml();
        var newFeedScene = await FeedScene.CreateAsync(importFeedPath: otherPath);
        Globals.scenes.Pop();
        Globals.scenes.Push(newFeedScene);
    }

    private MenuBlock SetArchiveModeMenu(FeedScene original, MenuOption parentOption)
    {
        var menu = new MenuBlock(AnchorType.Cursor);
        parentOption.childMenu = menu;
        foreach (ArchiveMode mode in Enum.GetValues(typeof(ArchiveMode)))
        {
            if (mode == ArchiveMode.Default)
            {
                continue;
            }
            var option = new MenuOption(FeedChannelMenu.PrettifyArchiveModeStatic(mode, false, original.feedSettings.maxArticleNumber, original.feedSettings.maxArticleAge), menu, () => Task.Run(() => original.SetDefaultArchiveMode(mode, menu, parentOption)));
            switch (mode)
            {
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

    public bool CheckRemoval()
    {
        if (options.Count <= 1 && parent != null)
        {
            Remove();
            parent.CheckRemoval();
            return true;
        }
        return false;
    }
}

public class FolderMenuAlt : MenuBlock
{
    public FolderMenuAlt(FolderMenu original, MenuOption parentOption, AnchorType anchorType = AnchorType.Cursor) : base(anchorType)
    {
        options.Add(new MenuOption("Rename", this, () => Task.Run(() => original.Rename(this))));
        options.Add(new MenuOption("Mark All as Read", this, () => Task.Run(() => original.MarkAllAsRead())));
        options.Add(new MenuOption("Move Folder", this, () => Task.Run(() => original.MoveFolder())));
        options.Add(new MenuOption("Remove Folder", this, ConfirmAction(() => Task.Run(() =>
        {
            Globals.activeScene.PopMenu();
            original.Remove();
            if (parentOption.parent is FolderMenu parentFolder && !parentFolder.CheckRemoval())
            {
                parentOption.parent.cursor = Int32.Clamp(parentOption.parent.cursor, 0, parentOption.parent.options.Count - 1);
                parentOption.parent.options[parentOption.parent.cursor].selected = true;
            }
        }))));
        options.Add(new MenuOption("Move Contents and Remove", this, () => Task.Run(() => original.MoveContents())));
    }
}
