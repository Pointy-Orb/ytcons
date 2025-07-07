using System.Xml;
using YTCons.MenuBlocks;
using System.Collections.Concurrent;


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
            //ConcurrentBag<FeedChannelMenu> rootMenus = new();
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
            instance.root.options.Sort((left, right) => string.Compare(left.option, right.option));
            instance.root.options.Insert(0, new MenuOption("Back", instance.root, () => Task.Run(() =>
                            { LoadBar.visible = false; Globals.scenes.Pop(); })));
            instance.root.options[instance.root.cursor].selected = true;
            instance.PushMenu(instance.root);
        }
        //TODO: Add global settings that allows the ability to import files more cleanly and change default archive settings
        instance.SaveAsXml();
        return instance;
    }

    public static async Task MakeFeedMenuAsync(ConcurrentBag<FeedChannelMenu> bag, FeedChannelMenu.PreMenuPacket packet)
    {
        var feedMenu = await FeedChannelMenu.CreateAsync(packet);
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

