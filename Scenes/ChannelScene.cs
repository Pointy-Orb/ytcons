using YTCons.MenuBlocks;
using System.Text.RegularExpressions;
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
            if (!File.Exists(Path.Combine(Path.GetTempPath(), $"{channelId}.json")) || channelId.Contains('@'))
            {
                var ytdlp = YtdlpFactory(channelId);
                using var jsonStream = File.Create(Path.Combine(Path.GetTempPath(), $"{channelId.Replace("@", "")}.json"));
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
            var dataJson = await File.ReadAllTextAsync(Path.Combine(Path.GetTempPath(), $"{channelId.Replace("@", "")}.json"));
            instance.channelData = JsonConvert.DeserializeObject<ChannelData>(dataJson)!;
            instance.channelId = instance.channelData.ChannelId;
            var menu = new MenuBlock(AnchorType.Top);
            menu.options.Add(new MenuOption("Back", menu, () => Task.Run(() => { LoadBar.visible = false; menu.resetNextTick = true; Globals.scenes.Pop(); })));
            foreach (VideoType value in typeof(VideoType).GetEnumValues())
            {
                var videoTypeMenu = instance.VideosMenu(value);
                if (videoTypeMenu.options.Count > 0)
                {
                    menu.options.Add(new MenuOption(value.ToString(), menu, () => Task.Run(() => instance.PushMenu(videoTypeMenu))));
                }
            }
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
            menu.options.Add(new MenuOption("Search", menu, () => Task.Run(() => instance.SearchChannel(menu))));
            if (addTheFeedOption)
            {
                var feedOption = new MenuOption("Add to Feeds", menu, () => Task.Run(() => { }));
                feedOption.ChangeOnSelected(() => Task.Run(() => instance.AddToFeed(feedOption)));
                menu.options.Add(feedOption);
            }
            instance.PushMenu(menu);
            return instance;
        }

        private void SearchChannel(MenuBlock menu)
        {
            var selDrawPos = menu.selectedDrawPos;
            menu.resetNextTick = true;
            var query = Globals.ReadLineNull(selDrawPos.x, selDrawPos.y, " >  Enter your search query (leave blank to cancel): ");
            if (query == null || query == "")
            {
                return;
            }
            var videos = VideosMenu(rawList: true);
            videos.options.AddRange(VideosMenu(VideoType.Shorts, true).options);
            videos.options.AddRange(VideosMenu(VideoType.Livestreams, true).options);
            if (videos.options == null || videos.options.Count <= 0)
            {
                return;
            }
            string[] keywords = query.Split(' ');
            videos.options.Sort((left, right) =>
            {
                int lPoints = 0;
                int rPoints = 0;
                for (int i = 0; i < keywords.Length; i++)
                {
                    int prev = Int32.Clamp(i - 1, 0, keywords.Length - 1);
                    if (Regex.Match(left.option, keywords[i], RegexOptions.IgnoreCase).Success)
                        lPoints += 3;
                    if (Regex.Match(right.option, keywords[i], RegexOptions.IgnoreCase).Success)
                        rPoints += 3;
                    if (left.option.Contains(keywords[i]))
                    {
                        lPoints++;
                    }
                    if (right.option.Contains(keywords[i]))
                    {
                        rPoints++;
                    }
                }
                if (left is MenuOptionWithEntry lOption && right is MenuOptionWithEntry rOption && lOption.entry.Description != null && rOption.entry.Description != null)
                {
                    for (int i = 0; i < keywords.Length; i++)
                    {
                        int prev = Int32.Clamp(i - 1, 0, keywords.Length - 1);
                        if (Regex.Match(lOption.entry.Description, keywords[i], RegexOptions.IgnoreCase).Success)
                            lPoints += 1;
                        if (Regex.Match(rOption.entry.Description, keywords[i], RegexOptions.IgnoreCase).Success)
                            rPoints += 1;
                    }
                }
                if (lPoints > rPoints) return -1;
                if (lPoints < rPoints) return 1;
                return 0;
            });
            PushMenu(videos);
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

        private enum VideoType
        {
            Videos,
            Shorts,
            Livestreams
        }

        private MenuBlock VideosMenu(VideoType videoType = VideoType.Videos, bool rawList = false)
        {
            bool hasLive = channelData.Entries.Count > 2;
            bool videosOnly = channelData.Channel != channelData.Title && channelData.Title.EndsWith("Videos");
            bool shortsOnly = channelData.Channel != channelData.Title && channelData.Title.EndsWith("Shorts");
            bool streamsOnly = channelData.Channel != channelData.Title && channelData.Title.EndsWith("Live");
            var menu = new MenuBlock(AnchorType.Cursor);
            menu.grayUnselected = true;
            switch (videoType)
            {
                case VideoType.Videos:
                    if (shortsOnly || streamsOnly)
                    {
                        return menu;
                    }
                    break;
                case VideoType.Shorts:
                    if (videosOnly || streamsOnly)
                    {
                        return menu;
                    }
                    break;
                case VideoType.Livestreams:
                    if (videosOnly || shortsOnly)
                    {
                        return menu;
                    }
                    break;
            }
            var entryType = -1;
            if (!videosOnly && !shortsOnly && !streamsOnly)
            {
                foreach (Entry entry in channelData.Entries)
                {
                    switch (videoType)
                    {
                        case VideoType.Videos:
                            if (entry.Title.EndsWith("Videos"))
                            {
                                entryType = channelData.Entries.IndexOf(entry);
                            }
                            break;
                        case VideoType.Shorts:
                            if (entry.Title.EndsWith("Shorts"))
                            {
                                entryType = channelData.Entries.IndexOf(entry);
                            }
                            break;
                        case VideoType.Livestreams:
                            if (entry.Title.EndsWith("Live"))
                            {
                                entryType = channelData.Entries.IndexOf(entry);
                            }
                            break;
                    }
                }
            }
            else
            {
                entryType = 0;
            }
            if (entryType <= -1)
            {
                return menu;
            }
            var entries = channelData.Entries[entryType].Entries;
            if (videosOnly || shortsOnly || streamsOnly)
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
            if (rawList)
            {
                menu.options.AddRange(videos);
                return menu;
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

        public override void PostDrawOptions()
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
                        Globals.SetForegroundColor(i, j, Globals.defaultForeground);
                        continue;
                    }
                    RootMenu.drawOffset = j + 5;
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
                            Globals.SetForegroundColor(i, j, Globals.defaultForeground);
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
                            Globals.SetForegroundColor(i, j, Globals.defaultForeground);
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
                            Globals.SetForegroundColor(i, j, Globals.defaultForeground);
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
            var link = $"https://www.youtube.com/channel/{id}";
            if (id.Contains('@'))
            {
                link = $"https://www.youtube.com/{id}";
            }
            if (id.Contains("https://"))
            {
                link = id;
            }
            process.StartInfo.Arguments = $"--flat-playlist -J {link}";
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
