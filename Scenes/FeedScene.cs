using System.Xml;
using Newtonsoft.Json;
using YTCons.MenuBlocks;
using System.Collections.Concurrent;
using System.Text;

namespace YTCons.Scenes;

public class FeedScene : Scene
{
    private FeedScene() { }

    private List<MenuBlock> savedMenus = new();

    public ArchiveMode defaultArchive = ArchiveMode.ArchiveAll;
    public int maxArticleAge = 60;
    public int maxArticleNumber = 1000;

    public FolderMenu root = new FolderMenu("root", null, AnchorType.Center);

    public static async Task<FeedScene> CreateAsync()
    {
        var instance = new FeedScene();
        string path = Path.Combine(Dirs.feedsDir, "feeds.opml");
        if (!File.Exists(path))
        {
            path = Globals.ReadLine(0, 0, "Enter the path of the opml file you want to import: ");
            while (!File.Exists(path) && !path.EndsWith(".opml"))
            {
                path = Globals.ReadLine(0, 0, "Enter the path of the opml file you want to import: ");
            }
        }
        using (var stream = File.OpenRead(path))
        {
            using var xmlStream = XmlReader.Create(stream);
            ConcurrentBag<FeedChannelMenu> rootMenus = new();
            List<Task> menuTasks = new();
            FolderMenu curFolder = instance.root;
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
                curFolder.CheckForChannels(xmlStream, menuTasks);
            }
            await Task.WhenAll(menuTasks);
            instance.root.RecursiveFolderAdding();
            instance.root.options.Insert(0, new MenuOption("Back", instance.root, () => Task.Run(() =>
                            { LoadBar.visible = false; Globals.scenes.Pop(); })));
            instance.root.options[instance.root.cursor].selected = true;
            instance.PushMenu(instance.root);
        }
        instance.SaveAsXml();
        return instance;
    }

    public static async Task MakeFeedMenuAsync(ConcurrentBag<FeedChannelMenu> bag, FeedChannelMenu.PreMenuPacket packet)
    {
        var feedMenu = await FeedChannelMenu.CreateAsync(packet);
        bag.Add(feedMenu);
    }

    public void SaveAsXml()
    {
        root.SaveAsXml();
    }
}

public class FolderMenu : MenuBlock
{
    public string title;

    public List<FolderMenu> subFolders = new();
    public ConcurrentBag<FeedChannelMenu> menus = new();
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
                        count += option.counter;
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
        menuTasks.Add(FeedScene.MakeFeedMenuAsync(menus, packet));
    }

    public void RecursiveFolderAdding()
    {
        foreach (FeedChannelMenu menu in menus)
        {
            var option = new MenuOption(menu.title, this,
                    () => Task.Run(() => Globals.activeScene.PushMenu(menu)),
                    () => Task.Run(() => Globals.activeScene.PushMenu(new FeedChannelMenuAlt(menu))));
            option.useCounter = true;
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

            var option = new FolderOption("[folder]/" + subFolder.title, this, subFolder, () => Task.Run(() => Globals.activeScene.PushMenu(subFolder)));
            option.useCounter = true;
            options.Add(option);
        }
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
        if (writeOutlines) writer.WriteStartElement("outline");
        writer.WriteAttributeString("text", title);

        foreach (FolderMenu subFolder in subFolders)
        {
            subFolder.WriteBody(writer);
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
}
