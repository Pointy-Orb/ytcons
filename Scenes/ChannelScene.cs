using YTCons.MenuBlocks;
using System.Diagnostics;
using Newtonsoft.Json;

namespace YTCons.Scenes
{
    public class ChannelScene : Scene
    {
        string channelId;
        bool popNextUpdate = false;
        public ChannelData channelData = null!;

        public static async Task<ChannelScene> CreateAsync(string channelId)
        {
            var instance = new ChannelScene(channelId);
            if (!File.Exists(Path.Combine(Path.GetTempPath(), $"{channelId}.json")))
            {
                var ytdlp = YtdlpFactory(channelId);
                using var jsonStream = File.Create(Path.Combine(Path.GetTempPath(), $"{channelId}.json"));
                using var jsonWriter = new StreamWriter(jsonStream);
                ytdlp.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
                {
                    if (!String.IsNullOrEmpty(e.Data))
                    {
                        jsonWriter.WriteLine(e.Data);
                    }
                });
                ytdlp.Start();
                ytdlp.BeginOutputReadLine();
                await ytdlp.WaitForExitAsync();
                if (ytdlp.ExitCode > 1)
                {
                    LoadBar.WriteLog("yt-dlp failed to grab channel data");
                    instance.popNextUpdate = true;
                    return instance;
                }
            }
            var dataJson = await File.ReadAllTextAsync(Path.Combine(Path.GetTempPath(), $"{channelId}.json"));
            instance.channelData = JsonConvert.DeserializeObject<ChannelData>(dataJson)!;
            var menu = new MenuBlock();
            menu.options.Add(new MenuOption("Back", menu, () => Task.Run(() => { LoadBar.visible = false; Globals.scenes.Pop(); })));
            var videosMenu = instance.VideosMenu();
            if (videosMenu.options.Count > 0)
            {
                menu.options.Add(new MenuOption("Videos", menu, () => Task.Run(() => instance.PushMenu(videosMenu))));
            }

            var shortsMenu = instance.VideosMenu(shorts: true);
            if (shortsMenu.options.Count > 0)
            {
                menu.options.Add(new MenuOption("Shorts", menu, () => Task.Run(() => instance.PushMenu(shortsMenu))));
            }
            //TODO: Add the ability to search a specific channel
            bool addTheFeedOption = true;
            if (File.Exists(Path.Combine(Dirs.feedsDir, "feedsPending.json")))
            {
                string feeds = await File.ReadAllTextAsync(Path.Combine(Dirs.feedsDir, "feedsPending.json"));
                if (feeds.Contains(instance.channelId))
                {
                    addTheFeedOption = false;
                }
            }
            if (File.Exists(Path.Combine(Dirs.feedsDir, "feeds.opml")))
            {
                string feeds = await File.ReadAllTextAsync(Path.Combine(Dirs.feedsDir, "feeds.opml"));
                if (feeds.Contains(instance.channelId))
                {
                    addTheFeedOption = false;
                }
            }
            if (addTheFeedOption)
            {
                var feedOption = new MenuOption("Add to Feeds", menu, () => Task.Run(() => { }));
                feedOption.ChangeOnSelected(() => Task.Run(() => instance.AddToFeed(feedOption)));
                menu.options.Add(feedOption);
            }
            instance.PushMenu(menu);
            return instance;
        }

        private void AddToFeed(MenuOption parentOption)
        {
            List<(string title, string id)> feedQueue = new();
            if (File.Exists(Path.Combine(Dirs.feedsDir, "feedsPending.json")))
            {
                string queueJson = File.ReadAllText(Path.Combine(Dirs.feedsDir, "feedsPending.json"));
                feedQueue = JsonConvert.DeserializeObject<List<(string title, string id)>>(queueJson);
            }
            feedQueue.Add((channelData.Channel, channelId));
            string queueOutJson = JsonConvert.SerializeObject(feedQueue);
            File.WriteAllText(Path.Combine(Dirs.feedsDir, "feedsPending.json"), queueOutJson);

            var parentMenu = parentOption.parent;
            parentMenu.options.Remove(parentOption);
            if (parentMenu.cursor >= parentMenu.options.Count)
            {
                parentMenu.cursor = parentMenu.options.Count - 1;
            }
            parentMenu.options[parentMenu.cursor].selected = true;

            LoadBar.WriteLog("Channel added to feeds.");
            parentMenu.resetNextTick = true;
        }

        class MenuOptionWithEntry : MenuOption
        {
            public Entry entry;
            public MenuOptionWithEntry(Entry entry, string option, MenuBlock parent, Func<Task> onSelected, Func<Task>? altOnSelected = null) : base(option, parent, onSelected, altOnSelected)
            {
                this.entry = entry;
            }
        }

        private MenuBlock VideosMenu(bool shorts = false)
        {
            //TODO: Add support for livestreams as well
            bool hasLive = channelData.Entries.Count > 2;
            bool videosOnly = channelData.Channel != channelData.Title && channelData.Title.EndsWith("Videos");
            bool shortsOnly = channelData.Channel != channelData.Title && channelData.Title.EndsWith("Shorts");
            var menu = new MenuBlock(AnchorType.Cursor);
            if (videosOnly && shorts)
            {
                return menu;
            }
            if (shortsOnly && !shorts)
            {
                return menu;
            }
            var entries = channelData.Entries[shorts ? (hasLive ? 2 : 1) : 0].Entries;
            if (videosOnly || shortsOnly)
            {
                entries = channelData.Entries;
            }
            List<MenuOption> videos = new();
            //Videos are listed by latest by default
            foreach (Entry entry in entries)
            {
                videos.Add(new MenuOptionWithEntry(entry, entry.Title, menu, () => Task.Run(async () =>
                {
                    LoadBar.loadMessage = "Getting video data";
                    LoadBar.StartLoad();
                    var videoMenu = await VideoBlock.CreateAsync(entry.Id);
                    LoadBar.visible = false;
                    LoadBar.ClearLoad();
                    PushMenu(videoMenu);
                })));
            }
            var byLatest = new MenuBlock(AnchorType.Cursor);
            byLatest.grayUnselected = true;
            var byPopular = new MenuBlock(AnchorType.Cursor);
            byPopular.grayUnselected = true;
            var byOldest = new MenuBlock(AnchorType.Cursor);
            byOldest.grayUnselected = true;
            byLatest.options = new List<MenuOption>(videos);
            byPopular.options = new List<MenuOption>(videos);
            byPopular.options.Sort((left, right) =>
            {
                if (left is MenuOptionWithEntry lOption && right is MenuOptionWithEntry rOption)
                {
                    if (lOption.entry.ViewCount > rOption.entry.ViewCount) return -1;
                    if (lOption.entry.ViewCount < rOption.entry.ViewCount) return 1;
                }
                return 0;
            });
            byOldest.options = new List<MenuOption>(videos);
            byOldest.options.Reverse();

            menu.options.Add(new MenuOption("By Latest", menu, () => Task.Run(() => PushMenu(byLatest))));
            menu.options.Add(new MenuOption("By Most Popular", menu, () => Task.Run(() => PushMenu(byPopular))));
            menu.options.Add(new MenuOption("By Oldest", menu, () => Task.Run(() => PushMenu(byOldest))));
            return menu;
        }

        protected override void OnUpdate()
        {
            if (popNextUpdate)
            {
                Globals.scenes.Pop();
                if (Globals.debug)
                {
                    Console.WriteLine("Something's awry!");
                }
            }
        }

        protected override void PostDraw()
        {
            int nameChar = 0;
            char[] channelName = channelData.Channel.Insert(0, "Channel: ").ToCharArray();
            int subChar = 0;
            char[] subCount = (channelData.ChannelFollowerCount.ToString("N0") + " subscribers").ToCharArray();
            int descChar = 0;
            char[] desc = channelData.Description.ToCharArray();
            bool writeFinalLine = false;
            for (int j = 0; j < Console.WindowHeight; j++)
            {
                if (writeFinalLine)
                {
                    for (int i = 0; i < Console.WindowWidth; i++)
                    {
                        Globals.Write(i, j, '─');
                        continue;
                    }
                    if (j > Console.WindowHeight / 2)
                    {
                        RootMenu.drawOffset = j + 5;
                    }
                    else
                    {
                        RootMenu.drawOffset = 0;
                    }
                    break;
                }
                bool newLining = false;
                for (int i = 0; i < Console.WindowWidth; i++)
                {
                    if (j == 0)
                    {
                        if (nameChar < channelName.Length)
                        {
                            Globals.Write(i, j, channelName[nameChar]);
                            nameChar++;
                        }
                        else
                        {
                            Globals.Write(i, j, ' ');
                        }
                    }
                    else if (j <= 2)
                    {
                        if (subChar < subCount.Length)
                        {
                            Globals.Write(i, j, subCount[subChar]);
                            subChar++;
                            continue;
                        }
                        else
                        {
                            Globals.Write(i, j, ' ');
                            continue;
                        }
                    }
                    else
                    {
                        if (newLining)
                        {
                            Globals.Write(i, j, ' ');
                            continue;
                        }
                        if (descChar < desc.Length)
                        {
                            if (desc[descChar] == '\n')
                            {
                                descChar++;
                                newLining = true;
                                Globals.Write(i, j, ' ');
                                continue;
                            }
                            if (desc[descChar] != ' ')
                            {
                                for (int m = descChar; m < desc.Length; m++)
                                {
                                    if (desc[m] == ' ' || desc[m] == '\n')
                                    {
                                        if (m - descChar >= Console.WindowWidth)
                                        {
                                            newLining = false;
                                        }
                                        break;
                                    }
                                    if ((i + m - descChar) > Console.WindowWidth)
                                    {
                                        newLining = true;
                                    }
                                }
                                if (newLining)
                                {
                                    continue;
                                }
                            }
                            Globals.Write(i, j, desc[descChar]);
                            descChar++;
                            continue;
                        }
                        else
                        {
                            Globals.Write(i, j, ' ');
                            writeFinalLine = true;
                            continue;
                        }
                    }
                }
            }
        }

        private static Process YtdlpFactory(string id)
        {
            var process = new Process();
            process.StartInfo.FileName = Dirs.GetPathApp("yt-dlp");
            process.StartInfo.Arguments = $"--flat-playlist -J https://www.youtube.com/channel/{id}";
            process.StartInfo.RedirectStandardOutput = true;
            if (!Globals.debug)
            {
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardInput = true;
            }
            else
            {
                Console.WriteLine(process.StartInfo.Arguments);
                Console.WriteLine(id);
            }
            return process;
        }

        private ChannelScene(string channelId)
        {
            this.channelId = channelId;
        }
    }
}
