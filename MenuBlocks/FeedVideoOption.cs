using System.Xml;
using Newtonsoft.Json;

namespace YTCons.MenuBlocks;

public class FeedVideoOption : MenuOption
{
    public class FeedData
    {
        public string description { get; set; } = "";
        public string id { get; set; } = "";
        public string title { get; set; } = "";
        public string channelId { get; set; } = "";
        public DateTime published { get; set; } = new DateTime();
        public bool isShort { get; set; } = false;
        public bool read { get; set; } = false;
    }

    public FeedData feedData = new();

    public bool DontDisplayUnread
    {
        get
        {
            if (parent is FeedChannelMenu channel)
            {
                return channel.lowPriority || feedData.read;
            }
            return false;
        }
    }

    public bool AgreesWithShortsStatus(ShortsStatus shortsStatus)
    {
        switch (shortsStatus)
        {
            case ShortsStatus.FullVideosOnly:
                return !feedData.isShort;
            case ShortsStatus.ShortsOnly:
                return feedData.isShort;
            default:
            case ShortsStatus.Unified:
                return true;
        }
    }

    public static async Task<List<FeedVideoOption>> CreateGroupAsync(MenuBlock parent, string sourceLink, ShortsStatus shortsStatus)
    {
        using var client = new HttpClient();
        using var source = await client.GetStreamAsync(sourceLink);
        List<FeedVideoOption> instances = new();
        FeedVideoOption instance = new FeedVideoOption("", parent);
        var settings = new XmlReaderSettings();
        settings.Async = true;
        using (XmlReader reader = XmlReader.Create(source, settings))
        {
            bool observingNode = false;
            bool skipObserve = false;
            while (await reader.ReadAsync())
            {
                if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName == "entry" && observingNode)
                {
                    observingNode = false;
                    if (File.Exists(instance.GetJsonPath()))
                    {
                        var data = await File.ReadAllTextAsync(instance.GetJsonPath());
                        instance.feedData = JsonConvert.DeserializeObject<FeedData>(data);
                    }
                    else
                    {
                        var instanceJson = JsonConvert.SerializeObject(instance.feedData);
                        await File.WriteAllTextAsync(instance.GetJsonPath(), instanceJson);
                    }
                    instance.OperationsAfterDataGot();
                    instances.Add(instance);
                }
                if (reader.NodeType != XmlNodeType.Element) continue;

                if (reader.LocalName == "entry")
                {
                    instance = new FeedVideoOption("", parent);
                    observingNode = true;
                    continue;
                }

                if (!observingNode || skipObserve) continue;
                //Actually get the data.
                if (reader.Name == "link" && reader.GetAttribute("href") != null)
                {
                    instance.feedData!.isShort = reader.GetAttribute("href")!.Contains("short");
                }
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
        return instances;
    }

    private readonly char newline = Convert.ToChar("\n");

    protected override void PostDraw(int i, int j)
    {
        if (!DontDisplayUnread)
        {
            int selectedOffset = selected && parent.confirmed ? 2 : 0;
            for (int l = i + 5 + selectedOffset; l < i + 11 + selectedOffset; l++)
            {
                Globals.SetForegroundColor(l, j, ConsoleColor.Yellow);
            }
        }
    }

    private bool InExtraWindow(int i, int j)
    {
        if (j == 1)
        {
            Globals.Write(i, j, '─');
            return false;
        }
        return j == 0;
    }

    public override void PostDrawEverything()
    {
        if (!selected || parent.confirmed) return;
        if (feedData.description == "") return;
        int descriptionCharacter = 0;
        int linkIndex = 0;
        ConsoleColor? foreground = null;
        ConsoleColor? background = null;
        var desc = feedData.description.ToCharArray();
        var extraDesc = $"Uploaded on {feedData.published.ToString()}".ToCharArray();
        var extraDescCharacter = 0;
        List<char> softDesc = desc.ToList();
        desc = softDesc.ToArray();
        for (int l = Console.WindowTop; l < Console.WindowHeight; l++)
        {
            bool newLining = false;
            for (int k = Console.WindowLeft; k < Console.WindowWidth; k++)
            {
                if (!InWindow(k, l)) continue;
                if (InExtraWindow(k, l))
                {
                    if (extraDescCharacter < extraDesc.Length)
                    {
                        Globals.Write(k, l, extraDesc[extraDescCharacter]);
                    }
                    else
                    {
                        Globals.Write(k, l, ' ');
                    }
                    extraDescCharacter++;
                    continue;
                }
                else if (l == 1) continue;
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
        if (xPos == Console.WindowWidth / 3)
        {
            Globals.Write(i, j, "│ ");
        }
        return xPos < Console.WindowWidth / 3;
    }

    private void OperationsAfterDataGot()
    {
        UpdateReadStatus();
    }

    public void UpdateReadStatus()
    {
        option = $"{(DontDisplayUnread ? "" : "(unread) ")}{feedData.title}";
    }

    private FeedVideoOption(string option, MenuBlock parent) : base(option, parent, () => Task.Run(() => { }))
    {
        _onSelected = () => OpenVideo();
    }

    private async Task OpenVideo()
    {
        LoadBar.loadMessage = "Opening video";
        LoadBar.StartLoad();
        var videoBlock = await VideoBlock.CreateAsync(feedData.id);
        LoadBar.visible = false;
        LoadBar.ClearLoad();
        if (videoBlock.selfDestruct)
        {
            videoBlock.selfDestruct = false;
            Globals.activeScene.PeekMenu().resetNextTick = true;
            return;
        }
        Globals.activeScene.PushMenu(videoBlock);
        if (!feedData.read)
        {
            feedData.read = true;
            if (parent is FeedChannelMenu feedMenu)
            {
                feedMenu.DecrementUnread();
            }
            UpdateReadStatus();
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
        var settings = new JsonSerializerSettings
        {
            ObjectCreationHandling = ObjectCreationHandling.Replace
        };
        feedData = JsonConvert.DeserializeObject<FeedData>(data, settings);
        OperationsAfterDataGot();
    }
}
