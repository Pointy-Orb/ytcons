using YTCons.MenuBlocks;
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

    public void CheckForChannels(XmlReader xmlStream, List<Task> menuTasks)
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
        menuTasks.Add(FeedScene.MakeFeedMenuAsync(menuBag, packet));
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

    public void BagToList()
    {
        menus = menuBag.ToList();
        foreach (FolderMenu subFolder in subFolders)
        {
            subFolder.BagToList();
        }
    }

    public void RecursiveFolderAdding(List<MenuBlock>? parentAllFeeds = null)
    {
        BagToList();
        //TODO: Have this update dynamically as folders are moved around
        MenuBlock allFeeds = new(AnchorType.Cursor);
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
            foreach (MenuOption feedOption in menu.options)
            {
                allFeeds.options.Add(feedOption);
                if (parentAllFeeds != null)
                {
                    foreach (MenuBlock parentAllFeed in parentAllFeeds)
                    {
                        parentAllFeed.options.Add(feedOption);
                    }
                }
            }
        }
        var rAllFeeds = parentAllFeeds == null ? new List<MenuBlock>() : parentAllFeeds;
        rAllFeeds.Add(allFeeds);
        foreach (FolderMenu subFolder in subFolders)
        {
            subFolder.RecursiveFolderAdding(rAllFeeds);
            if (subFolder.options.Count() <= 0) continue;

            if (subFolder.options.Count() > 7) subFolder.grayUnselected = true;
            subFolder.options.Sort((left, right) => string.Compare(left.option, right.option));
            subFolder.options[subFolder.cursor].selected = true;

            var option = new FolderOption("[folder]/" + subFolder.title, this, subFolder, () => Task.Run(() => Globals.activeScene.PushMenu(subFolder)), () => Task.Run(() => Globals.activeScene.PushMenu(new FolderMenuAlt(subFolder))));
            option.tip = "Press Enter for folder options";
            option.useCounter = true;
            options.Add(option);
        }
        if (allFeeds.options.Count() <= 0) return;
        allFeeds.options.Sort((left, right) =>
        {
            var l = (FeedVideoOption)left;
            var r = (FeedVideoOption)right;

            return r.feedData.published.CompareTo(l.feedData.published);
        });
        allFeeds.grayUnselected = true;
        if (allFeeds.options.Count <= 0) return;
        options.Add(new MenuOption("[All Feeds]", this, () => Task.Run(() => Globals.activeScene.PushMenu(allFeeds))));
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

    private (MenuBlock menu, string title) _RecursiveFolderChoiceMenu(MenuOption parentOption, FolderChoiceMenuPurpose purpose, int depth = 1, FolderMenu? folderToMove = null, FeedChannelMenu? feedMenu = null)
    {
        var menu = new MenuBlock(AnchorType.Cursor);
        List<(MenuBlock menu, string title)> subMenus = new();
        foreach (FolderMenu subFolder in subFolders)
        {
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
                //TODO: Make this method
                break;
        }
        menu.options.Add(new MenuOption("Place Here" + clariflyRoot, menu, move));
        menu.options.Add(new MenuOption("Make Folder Here", menu, newFolder));
        foreach ((MenuBlock menu, string title) subMenu in subMenus)
        {
            menu.options.Add(new MenuOption(subMenu.title, menu, () => Task.Run(() => Globals.activeScene.PushMenu(subMenu.menu))));
        }
        return (menu, title);
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
        var option = new FolderMenu.FolderOption("[folder]/" + newFolder.title, parentFolder, newFolder, () => Task.Run(() => Globals.activeScene.PushMenu(newFolder)), () => Task.Run(() => Globals.activeScene.PushMenu(new FolderMenuAlt(newFolder))));
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
        for (int i = 0; i <= depth; i++)
        {
            Globals.activeScene.PopMenu();
        }
        root.SaveAsXml();
    }
}

public class FolderMenuAlt : MenuBlock
{
    public FolderMenuAlt(FolderMenu original, AnchorType anchorType = AnchorType.Cursor) : base(anchorType)
    {
        options.Add(new MenuOption("Rename", this, () => Task.Run(() => original.Rename(this))));
        options.Add(new MenuOption("Mark All as Read", this, () => Task.Run(() => original.MarkAllAsRead())));
        options.Add(new MenuOption("Remove Folder", this, ConfirmAction(() => Task.Run(() => original.Remove()))));
        options.Add(new MenuOption("Move Folder", this, () => Task.Run(() => original.MoveFolder())));
        //TODO: Add "Move Contents" option that works like Move To Folder but does it to all the contents 
    }
}
