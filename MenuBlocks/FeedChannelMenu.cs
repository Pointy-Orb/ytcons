using System.Xml;
using Newtonsoft.Json;

namespace YTCons.MenuBlocks;

public class FeedChannelMenu : MenuBlock
{
    public MenuOption? optionParent;

    public static async Task<FeedChannelMenu> CreateAsync(string url)
    {
        var instance = new FeedChannelMenu();
        using (var client = new HttpClient())
        {
            using var xmlStream = await client.GetStreamAsync(url);
            int feedVideos = 0;
            using (var xmlCheck = XmlReader.Create(xmlStream, new XmlReaderSettings { Async = true }))
            {
                while (await xmlCheck.ReadAsync())
                {
                    if (xmlCheck.NodeType == XmlNodeType.Element && xmlCheck.Name == "entry")
                    {
                        feedVideos++;
                    }
                }
            }
            for (int i = 0; i < feedVideos; i++)
            {
                using var xmlOtherStream = await client.GetStreamAsync(url);
                instance.options.Add(await MenuBlocks.FeedVideoOption.CreateAsync(instance, xmlOtherStream, i));
            }
        }
        instance.options[instance.cursor].selected = true;
        var files = Directory.EnumerateFiles(Path.Combine(Dirs.feedsDir, url.Replace("https://www.youtube.com/feeds/videos.xml?channel_id=", "")));
        //Pick up the stragglers that weren't included in the online feed.
        foreach (string filePath in files)
        {
            if (instance.options.Find(i => i is FeedVideoOption option && option.feedData.id == Path.GetFileNameWithoutExtension(filePath)) == null)
            {
                instance.options.Add(new FeedVideoOption(instance, filePath));
            }
        }
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
}

public class FeedChannelMenuAlt : MenuBlock
{
    public FeedChannelMenuAlt(FeedChannelMenu original, AnchorType anchorType = AnchorType.Cursor) : base(anchorType)
    {
        options.Add(new MenuOption("Mark All as Read", this, () => Task.Run(() => original.MarkAllAsRead())));
        options[cursor].selected = true;
    }
}
