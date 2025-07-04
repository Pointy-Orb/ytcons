using System.Xml;
using Newtonsoft.Json;
using System.Collections.Concurrent;

namespace YTCons.MenuBlocks;

public class FeedChannelMenu : MenuBlock
{
    public MenuOption? optionParent;

    public string title = "";

    public string id = "";

    public int maxAritcleAge;

    public int maxArticleNumber;

    public bool destroyTheElderly = false;

    public bool notify = false;
    public bool lowPriority = false;

    public class PreMenuPacket
    {
        public required string url;
        public required string title;
        public int maxAritcleAge = 60;
        public int maxArticleNumber = 1000;
        public bool notify;
    }

    public static async Task<FeedChannelMenu> CreateAsync(PreMenuPacket packet)
    {
        var instance = new FeedChannelMenu()
        {
            title = packet.title,
            maxAritcleAge = packet.maxAritcleAge,
            maxArticleNumber = packet.maxArticleNumber,
            notify = packet.notify
        };
        instance.id = packet.url.Replace("https://www.youtube.com/feeds/videos.xml?channel_id=", "");
        if (instance.destroyTheElderly)
        {
            instance.CheckAnnihilation();
        }
        List<FeedVideoOption> videos = await FeedVideoOption.CreateGroupAsync(instance, packet.url);
        foreach (FeedVideoOption video in videos)
        {
            instance.options.Add(video);
        }
        var files = Directory.EnumerateFiles(Path.Combine(Dirs.feedsDir, instance.id));
        foreach (string filePath in files)
        {
            if (instance.options.Find(i => i is FeedVideoOption option && option.feedData.id == Path.GetFileNameWithoutExtension(filePath)) == null)
            {
                instance.options.Add(new FeedVideoOption(instance, filePath));
            }
        }
        instance.options.Sort((left, right) =>
        {
            var l = (FeedVideoOption)left;
            var r = (FeedVideoOption)right;

            return r.feedData.published.CompareTo(l.feedData.published);
        });
        instance.options[instance.cursor].selected = true;
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

    public void ChangePriority(MenuOption display)
    {
        lowPriority = !lowPriority;
        if (optionParent != null) optionParent.useCounter = !lowPriority;
        foreach (MenuOption option in options)
        {
            if (option is FeedVideoOption video)
            {
                video.UpdateReadStatus();
            }
        }
        display.option = $"Switch Priority (current: {(lowPriority ? "Low" : "Normal")})";
    }

    private void CheckAnnihilation()
    {
        var files = Directory.EnumerateFiles(Path.Combine(Dirs.feedsDir, id)).ToList();
        files.Sort((left, right) =>
        {
            var lTime = File.GetCreationTime(left);
            var rTime = File.GetCreationTime(right);
            return rTime.CompareTo(lTime);
        });
        int deleted = 0;
        for (int i = 0; i < files.Count(); i++)
        {
            if (DateTime.Now - File.GetCreationTime(files[i]) > TimeSpan.FromDays(maxAritcleAge))
            {
                File.Delete(files[i]);
                deleted++;
            }
            if (i - deleted >= maxArticleNumber)
            {
                File.Delete(files[i]);
                deleted++;
            }
        }
    }
}

public class FeedChannelMenuAlt : MenuBlock
{
    private bool queueReset = false;

    public FeedChannelMenuAlt(FeedChannelMenu original, AnchorType anchorType = AnchorType.Cursor) : base(anchorType)
    {
        options.Add(new MenuOption("Mark All as Read", this, () => Task.Run(() => original.MarkAllAsRead())));

        var priorityOption = new MenuOption($"Switch Priority (current: {(original.lowPriority ? "Low" : "Normal")})", this, () => Task.Run(() => { }));
        priorityOption.ChangeOnSelected(() => Task.Run(() => { queueReset = true; original.ChangePriority(priorityOption); }));
        priorityOption.tip = "Normal priority displays which articles are unread, low priority doesn't. Meant to reduce clutter from feeds that post frequently.";
        options.Add(priorityOption);

        options[cursor].selected = true;
    }

    protected override void OnUpdate()
    {
        if (queueReset)
        {
            queueReset = false;
            Reset();
        }
    }
}
