using System.Xml;
using Newtonsoft.Json;

namespace YTCons.MenuBlocks;

public class FeedVideoOption : MenuOption
{
    public string description = "";
    public string id = "";
    public string title = "";
    public string channelId = "";
    public DateTime published = new DateTime();

    public static async Task<FeedVideoOption> CreateAsync(MenuBlock parent, Stream source, int index)
    {
        FeedVideoOption instance = new FeedVideoOption("", parent);
        var settings = new XmlReaderSettings();
        settings.Async = true;
        using (XmlReader reader = XmlReader.Create(source, settings))
        {
            int i = 0;
            bool observingNode = false;
            while (await reader.ReadAsync())
            {
                if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName == "entry" && observingNode)
                {
                    //No reason to keep iterating if we already exhausted the contents of the node we want to observe.
                    observingNode = false;
                    break;
                }
                if (reader.NodeType != XmlNodeType.Element) continue;

                //Iterate until we find the contents of the node we want to observe.
                if (reader.LocalName == "entry")
                {
                    if (i != index)
                    {
                        i++;
                        continue;
                    }
                    observingNode = true;
                    continue;
                }

                if (!observingNode) continue;
                //Actually get the data.
                if (reader.Name == "yt:videoId")
                {
                    instance.id = await reader.ReadElementContentAsStringAsync();
                }
                if (reader.Name == "yt:channelId")
                {
                    instance.channelId = await reader.ReadElementContentAsStringAsync();
                }
                if (reader.Name == "title")
                {
                    instance.title = await reader.ReadElementContentAsStringAsync();
                }
                if (reader.Name == "published")
                {
                    var offset = DateTimeOffset.Parse(await reader.ReadElementContentAsStringAsync());
                    instance.published = offset.LocalDateTime;
                }
                if (reader.Name == "media:description")
                {
                    instance.description = await reader.ReadElementContentAsStringAsync();
                }
            }
        }
        if (!File.Exists(instance.GetJsonPath()))
        {
            var instanceJson = JsonConvert.SerializeObject(instance);
            await File.WriteAllTextAsync(instance.GetJsonPath(), instanceJson);
        }
        instance.option = instance.title;
        return instance;
    }

    private FeedVideoOption(string option, MenuBlock parent) : base(option, parent, () => Task.Run(() => { }))
    {
        _onSelected = () => OpenVideo();
    }

    private async Task OpenVideo()
    {
        var videoBlock = await VideoBlock.CreateAsync(id);
        Globals.activeScene.PushMenu(videoBlock);
    }

    public string GetJsonPath()
    {
        var path = Path.Combine(Dirs.feedsDir, channelId);
        Directory.CreateDirectory(path);
        var json = Path.Combine(path, id + ".json");
        return json;
    }
}
