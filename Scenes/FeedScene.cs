using System.Xml;
using Newtonsoft.Json;
using YTCons.MenuBlocks;

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
        var menu = new MenuBlock();
        menu.options.Add(new MenuOption("Back", menu, () => Task.Run(() => { LoadBar.visible = false; Globals.scenes.Pop(); })));
        while (xmlStream.Read())
        {
            if (xmlStream.NodeType != XmlNodeType.Element) continue;
            if (!xmlStream.HasAttributes) continue;
            if (xmlStream.GetAttribute("xmlUrl") == null) continue;
            if (!xmlStream.GetAttribute("xmlUrl").Contains("https://www.youtube.com/feeds")) continue;
            var feedMenu = await FeedChannelMenu.CreateAsync(xmlStream.GetAttribute("xmlUrl"));
            var option = new MenuOption(xmlStream.GetAttribute("text"), menu, () => Task.Run(() => instance.PushMenu(feedMenu)), () => Task.Run(() => instance.PushMenu(new FeedChannelMenuAlt(feedMenu))));
            option.useCounter = true;
            option.childMenu = feedMenu;
            feedMenu.optionParent = option;
            feedMenu.CheckUnreadOptions();
            menu.options.Add(option);
        }
        instance.PushMenu(menu);
        return instance;
    }
}
