using System.Xml;
using Newtonsoft.Json;
using YTCons.MenuBlocks;
using System.Collections.Concurrent;

namespace YTCons.Scenes;

public class FeedScene : Scene
{
    private FeedScene() { }

    private List<MenuBlock> savedMenus = new();

    public static async Task<FeedScene> CreateAsync()
    {
        var instance = new FeedScene();
        string path = Globals.ReadLine(0, 0, "Enter the path of the opml file you want to import: ");
        while (!File.Exists(path) && !path.EndsWith(".opml"))
        {
            path = Globals.ReadLine(0, 0, "Enter the path of the opml file you want to import: ");
        }
        using var stream = File.OpenRead(path);
        using var xmlStream = XmlReader.Create(stream);
        FolderMenu root = new FolderMenu("root", null, AnchorType.Center);
        ConcurrentBag<FeedChannelMenu> rootMenus = new();
        List<Task> menuTasks = new();
        FolderMenu curFolder = root;
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
        root.RecursiveFolderAdding();
        root.options.Insert(0, new MenuOption("Back", root, () => Task.Run(() =>
                        { LoadBar.visible = false; Globals.scenes.Pop(); })));
        root.options[root.cursor].selected = true;
        instance.PushMenu(root);
        return instance;
    }

    public static async Task MakeFeedMenuAsync(ConcurrentBag<FeedChannelMenu> bag, FeedChannelMenu.PreMenuPacket packet)
    {
        var feedMenu = await FeedChannelMenu.CreateAsync(packet);
        bag.Add(feedMenu);
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
            for (int l = i + 8 + selectedOffset + digits; l < i + 14 + digits + selectedOffset; l++)
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
            url = xmlStream.GetAttribute("xmlUrl"),
            title = xmlStream.GetAttribute("title"),
            notify = xmlStream.GetAttribute("useNotification") == "true"
        };
        if (xmlStream.GetAttribute("maxAritcleAge") != null)
        {
            packet.maxAritcleAge = Convert.ToInt32(xmlStream.GetAttribute("maxAritcleAge"));
        }
        if (xmlStream.GetAttribute("maxArticleNumber") != null)
        {
            packet.maxArticleNumber = Convert.ToInt32(xmlStream.GetAttribute("maxArticleNumber"));
        }
        string url = xmlStream.GetAttribute("xmlUrl");
        string text = xmlStream.GetAttribute("text");
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
}
