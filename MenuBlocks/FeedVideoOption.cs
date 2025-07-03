using System.Xml;
using Newtonsoft.Json;

namespace YTCons.MenuBlocks;

public class FeedVideoOption : MenuOption
{
    public class FeedData
    {
        public string description = "";
        public string id = "";
        public string title = "";
        public string channelId = "";
        public DateTime published = new DateTime();
        public bool read = false;
    }

    public FeedData feedData = new();

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
                    instance.feedData.id = await reader.ReadElementContentAsStringAsync();
                }
                if (reader.Name == "yt:channelId")
                {
                    instance.feedData.channelId = await reader.ReadElementContentAsStringAsync();
                }
                if (reader.Name == "title")
                {
                    instance.feedData.title = await reader.ReadElementContentAsStringAsync();
                }
                if (reader.Name == "published")
                {
                    var offset = DateTimeOffset.Parse(await reader.ReadElementContentAsStringAsync());
                    instance.feedData.published = offset.LocalDateTime;
                }
                if (reader.Name == "media:description")
                {
                    instance.feedData.description = await reader.ReadElementContentAsStringAsync();
                }
            }
        }
        if (File.Exists(instance.GetJsonPath()))
        {
            string data = await File.ReadAllTextAsync(instance.GetJsonPath());
            instance.feedData = JsonConvert.DeserializeObject<FeedData>(data);
        }
        else
        {
            var instanceJson = JsonConvert.SerializeObject(instance.feedData);
            await File.WriteAllTextAsync(instance.GetJsonPath(), instanceJson);
        }
        instance.OperationsAfterDataGot();
        return instance;
    }

    private readonly char newline = Convert.ToChar("\n");

    protected override void PostDraw(int i, int j)
    {
        if (!feedData.read)
        {
            int selectedOffset = selected && parent.confirmed ? 2 : 0;
            for (int l = i + 5 + selectedOffset; l < i + 11 + selectedOffset; l++)
            {
                Globals.SetForegroundColor(l, j, ConsoleColor.Yellow);
            }
        }
    }

    public override void PostDrawEverything()
    {
        if(!selected || parent.confirmed) return;
        int descriptionCharacter = 0;
        int linkIndex = 0;
        ConsoleColor? foreground = null;
        ConsoleColor? background = null;
        var desc = feedData.description.ToCharArray();
        List<char> softDesc = desc.ToList();
        desc = softDesc.ToArray();
        for (int l = Console.WindowTop; l < Console.WindowHeight; l++)
        {
            bool newLining = false;
            for (int k = Console.WindowLeft; k < Console.WindowWidth; k++)
            {
                if (!InWindow(k, l)) continue;
                try
                {
                    Globals.activeScene.protectedTile[k, l] = true;
                }
                catch { }
                if (newLining || descriptionCharacter >= desc.Length)
                {
                    Globals.ClearTile(k, l);
                    continue;
                }
                if (descriptionCharacter >= desc.Length) continue;
                if (desc[descriptionCharacter] == newline && Globals.CheckNoEscape(descriptionCharacter, desc))
                {
                    Globals.Write(k, l, Convert.ToChar(" "));
                    descriptionCharacter++;
                    newLining = true;
                    continue;
                }
                if (desc[descriptionCharacter] == Convert.ToChar("⁒") || desc[descriptionCharacter] == Convert.ToChar("▷") && Globals.CheckNoEscape(descriptionCharacter, desc))
                {
                    var red = false;
                    linkIndex++;
                    if (desc[descriptionCharacter] == Convert.ToChar("▷"))
                    {
                        foreground = ConsoleColor.Red;
                        red = true;
                    }
                    else
                    {
                        foreground = ConsoleColor.Blue;
                    }
                    Globals.Write(k, l, Convert.ToChar(red ? "⏵" : "⇱"));
                    Globals.SetForegroundColor(k, l, (ConsoleColor)foreground);
                    descriptionCharacter++;
                    continue;
                }
                else if (desc[descriptionCharacter] == Convert.ToChar("⭖") && Globals.CheckNoEscape(descriptionCharacter, desc))
                {
                    foreground = null;
                    background = null;
                    Globals.Write(k, l, Convert.ToChar(" "));
                    descriptionCharacter++;
                    continue;
                }
                if (foreground != null)
                {
                    Globals.SetForegroundColor(k, l, (ConsoleColor)foreground);
                }
                else
                {
                    Globals.SetForegroundColor(k, l, Globals.defaultForeground);
                }
                if (background != null)
                {
                    Globals.SetBackgroundColor(k, l, (ConsoleColor)background);
                }
                if (desc[descriptionCharacter] != Convert.ToChar(" "))
                {
                    for (int m = descriptionCharacter; m < desc.Length; m++)
                    {
                        if (desc[l] == Convert.ToChar(" "))
                        {
                            if (m - descriptionCharacter >= windowWidth)
                            {
                                newLining = false;
                            }
                            break;
                        }
                        if (!InWindow(k + m - descriptionCharacter, m))
                        {
                            newLining = true;
                        }
                    }
                }
                if (newLining) continue;
                if (descriptionCharacter == 0)
                {
                    Globals.Write(k, l, desc[descriptionCharacter]);
                }
                else if (descriptionCharacter != Convert.ToChar(@"\"))
                {
                    Globals.Write(k, l, desc[descriptionCharacter]);
                }
                descriptionCharacter++;
            }
        }
    }

    int windowWidth = 0;

    private bool InWindow(int i, int j)
    {
        var xPos = Console.WindowWidth - i;
        windowWidth = Console.WindowWidth / 3;
        if(xPos == Console.WindowWidth/3)
        {
            Globals.Write(i,j,"│ ");
        }
        return xPos < Console.WindowWidth/3;
    }

    private void OperationsAfterDataGot()
    {
        option = $"{(feedData.read ? "" : "(unread) ")}{feedData.title}";
    }

    private FeedVideoOption(string option, MenuBlock parent) : base(option, parent, () => Task.Run(() => { }))
    {
        _onSelected = () => OpenVideo();
    }

    private async Task OpenVideo()
    {
        var videoBlock = await VideoBlock.CreateAsync(feedData.id);
        Globals.activeScene.PushMenu(videoBlock);
        if (!feedData.read)
        {
            feedData.read = true;
            if (parent is FeedChannelMenu feedMenu)
            {
                feedMenu.DecrementUnread();
            }
            option = feedData.title;
            var feedJson = JsonConvert.SerializeObject(feedData);
            await File.WriteAllTextAsync(GetJsonPath(), feedJson);
        }
    }

    public string GetJsonPath()
    {
        var path = Path.Combine(Dirs.feedsDir, feedData.channelId);
        Directory.CreateDirectory(path);
        var json = Path.Combine(path, feedData.id + ".json");
        return json;
    }

    public FeedVideoOption(MenuBlock parent, string path) : base(path, parent, () => Task.Run(() => { }))
    {
        _onSelected = () => OpenVideo();
        string data = File.ReadAllText(path);
        feedData = JsonConvert.DeserializeObject<FeedData>(data);
        OperationsAfterDataGot();
    }
}
