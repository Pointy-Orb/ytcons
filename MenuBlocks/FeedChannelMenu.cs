using System.Xml;
using Newtonsoft.Json;

namespace YTCons.MenuBlocks;

public class FeedChannelMenu : MenuBlock
{
    private const string feedBaseUrl = "https://www.youtube.com/feeds/videos.xml?channel_id=";

    public static async Task<FeedChannelMenu> CreateAsync(string channelId)
    {
        var instance = new FeedChannelMenu();
        using (var client = new HttpClient())
        {
            using var xmlStream = await client.GetStreamAsync(feedBaseUrl + channelId);
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
                using var xmlOtherStream = await client.GetStreamAsync(feedBaseUrl + channelId);
                instance.options.Add(await MenuBlocks.FeedVideoOption.CreateAsync(instance, xmlOtherStream, i));
            }
        }
        instance.options[instance.cursor].selected = true;
        return instance;
    }

    private FeedChannelMenu(AnchorType anchorType = AnchorType.Cursor) : base(anchorType) { }
}
