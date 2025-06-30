using System.Diagnostics;
using System.Text;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using ImageMagick;

namespace YTCons.MenuBlocks;

public class VideoBlock : MenuBlock
{
    public readonly string videoID;
    public string extraData = "";
    public ExtractedVideoInfo videoInfo = ExtractedVideoInfo.Empty;
    private Task gettingThumbnail = new Task(() => { });
    private enum InactiveReason
    {
        Active,
        ShowingDeets,
        Playing,
        ShowingThumbnail,
        Playlisting
    }
    private InactiveReason activeBecause;
    string thumbnailPath = "";
    char[] sourceDesc = new char[0];
    bool paged = false;

    public static async Task<VideoBlock> CreateAsync(string id)
    {
        var instance = new VideoBlock(id);
        instance.videoInfo = await ExtractedVideoInfo.CreateAsync(id);
        instance.thumbnailPath = Path.GetTempPath() + instance.videoID + ".webp";
        instance.extraData = instance.MakeExtraData();
        instance.sourceDesc = instance.FinishConstructor();
        instance.ChangeWindowSize();
        instance.gettingThumbnail = instance.ShowThumbnailInner();
        return instance;
    }

    private VideoBlock(string id, AnchorType anchorType = AnchorType.Cursor) : base(anchorType)
    {
        videoID = id;
    }

    public VideoBlock(ExtractedVideoInfo info, AnchorType anchorType = AnchorType.Cursor) : base(anchorType)
    {
        videoID = info.id;
        videoInfo = info;
        thumbnailPath = Path.GetTempPath() + videoID + ".webp";
        extraData = MakeExtraData();
        sourceDesc = FinishConstructor();
        gettingThumbnail = ShowThumbnailInner();
        ChangeWindowSize();
    }

    private string Ratio(int num1, int num2)
    {
        if (num1 == num2) return "1:1";
        int biggerNum = num1 > num2 ? num1 : num2;
        var gcd = 1;
        for (int i = 1; i < biggerNum / 2; i++)
        {
            if (num1 % i == 0 && num2 % i == 0)
            {
                gcd = i;
            }
        }
        if (gcd == 1)
        {
            return "";
        }
        return $"{num1 / gcd}:{num2 / gcd}";
    }

    private string MakeExtraData()
    {
        StringBuilder extraDataBuilder = new("👁 Views: ");
        extraDataBuilder.Append(videoInfo.video.views.ToString("N0") + "  │  👍 Likes: ");
        extraDataBuilder.Append(videoInfo.video.likes.ToString("N0") + $"  {Ratio(videoInfo.video.likes, videoInfo.video.dislikes)}  👎 Dislikes: ");
        extraDataBuilder.Append(videoInfo.video.dislikes.ToString("N0"));
        extraDataBuilder.Append("  │📺 Channel: " + (videoInfo.video.uploaderVerified ? "✔️" : "") + videoInfo.video.uploader);
        extraDataBuilder.Append($" (目 Subs: {videoInfo.video.uploaderSubscriberCount.ToString("N0")})");
        extraDataBuilder.Append($"  │ 𐁗  Uploaded on {videoInfo.video.uploadDate.ToString()}");
        extraDataBuilder.Append("  │ 🕓 Duration: " + videoInfo.ParsedDuration());
        return extraDataBuilder.ToString();
    }

    private char[] FinishConstructor()
    {
        options.Add(new MenuOption("Play", this, () => PlayAsync(), () => Globals.activeScene.PushMenuAsync(new ChooseFormat(videoInfo))));
        if (Dirs.ytdlp != null)
        {
            options.Add(new MenuOption("Download", this, () => Task.Run(() => Globals.activeScene.PushMenu(new ChooseFormat(videoInfo, true)))));
        }
        else if (Dirs.ffmpeg != null)
        {
            options.Add(new MenuOption("Download", this, () => Task.Run(() => Globals.activeScene.PushMenu(new ChooseResolution(videoInfo, "MPEG_4", true)))));
        }
        options.Add(new MenuOption("Add To Playlist", this, () => Task.Run(() => Globals.activeScene.PushMenu(new AddToPlaylist(videoInfo)))));
        options.Add(new MenuOption("Show Description", this, () => Task.Run(() =>
        {
            active = false;
            activeBecause = InactiveReason.ShowingDeets;
            videoInfo.windowOpen = true;
        })));
        options.Add(new MenuOption("Show Thumbnail", this, () => Task.Run(() =>
        {
            active = false;
            activeBecause = InactiveReason.ShowingThumbnail;
            var showThumbail = ShowThumbnail();
            while (!showThumbail.IsCompleted)
            {
                LoadBar.loadMessage = "Getting thumbnail";
                LoadBar.WriteLoad();
            }
            LoadBar.ClearLoad();
        }), () => Task.Run(() =>
        {
            var confirmDownload = new MenuBlock(AnchorType.Cursor);
            confirmDownload.options.Add(new MenuOption("Download", confirmDownload, () => DownloadThumbnail()));
            confirmDownload.options[confirmDownload.cursor].selected = true;
            Globals.activeScene.PushMenu(confirmDownload);
        })));
        options[cursor].selected = true;
        return ParseDescription(videoInfo.video.description).ToCharArray();
    }

    private async Task PlayAsync()
    {
        active = false;
        activeBecause = InactiveReason.Playing;
        await videoInfo.Play();
    }

    private Process imageViewer = new();

    private async Task DownloadThumbnail()
    {
        if (!gettingThumbnail.IsCompleted)
        {
            await gettingThumbnail;
        }
        var webp = await File.ReadAllBytesAsync(thumbnailPath);
        using var image = new MagickImage(webp);
        var thumbnailDownloadPath = Path.Combine(Dirs.downloadsDir, "thumbnails");
        Directory.CreateDirectory(thumbnailDownloadPath);
        image.Write(Path.Combine(thumbnailDownloadPath, $"{Dirs.MakeFileSafe(videoInfo.video.title, true)}.png"));
        LoadBar.WriteLog($"Thumbnail downloaded to {Path.Combine(thumbnailDownloadPath, $"{videoInfo.video.title.Replace(":", ",")}.png")}");
        Globals.activeScene.PopMenu();
    }

    private async Task ShowThumbnailInner()
    {
        using (HttpClient client = new HttpClient())
        {
            var thumbnailUrl = new Uri(videoInfo.video.thumbnailUrl);
            byte[] thumbnailBytes = await client.GetByteArrayAsync(thumbnailUrl);
            await File.WriteAllBytesAsync(thumbnailPath, thumbnailBytes);
        }
    }

    bool gotThumbnail = false;
    private async Task ShowThumbnail()
    {
        if (!gettingThumbnail.IsCompleted)
        {
            await gettingThumbnail;
        }
        try
        {
            gotThumbnail = true;
            imageViewer = new();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                imageViewer.StartInfo = new ProcessStartInfo
                {
                    FileName = thumbnailPath,
                    UseShellExecute = true
                };
            }
            else
            {
                var process = File.Exists("/usr/bin/chafa") ? "/usr/bin/chafa" : "/usr/bin/open";
                Console.SetCursorPosition(Console.WindowWidth / 2, 0);
                imageViewer.StartInfo = new ProcessStartInfo
                {
                    FileName = process,
                    Arguments = thumbnailPath
                };
            }
            imageViewer.StartInfo.RedirectStandardInput = true;
            imageViewer.StartInfo.RedirectStandardError = true;
            imageViewer.Start();
            var key = Console.ReadKey();
            while (key.Key != ConsoleKey.Enter && key.Key != ConsoleKey.Spacebar && key.Key != ConsoleKey.Q)
            {
                key = Console.ReadKey();
            }
            Console.Clear();
        }
        catch
        {
            if (gotThumbnail) Console.Write("Error opening image application.");
            else Console.Write("Error getting thumbnail");
            Console.Write(" Press enter to resume program.");
            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                {
                    break;
                }
            }
        }
    }

    protected override void OnUpdate()
    {
        if (activeBecause == InactiveReason.ShowingDeets && !videoInfo.windowOpen)
        {
            activeBecause = InactiveReason.Active;
            Reset();
            Console.Clear();
        }
        if (activeBecause == InactiveReason.Playing && videoInfo.mediaPlayer.HasExited)
        {
            activeBecause = InactiveReason.Active;
            Reset();
            Console.Clear();
        }
        if (activeBecause == InactiveReason.ShowingThumbnail)
        {
            activeBecause = InactiveReason.Active;
            Reset();
        }
    }

    int pageOffset = 0;

    static readonly char newline = Convert.ToChar("¤");

    private bool CheckNeedsPage()
    {
        //Set the window width and height values if they aren't already
        if (windowWidth == 0 || windowHeight == 0)
        {
            InWindow(0, 0);
        }
        int lines = 0;
        int charsSinceNewline = 0;
        for (int i = 0; i < sourceDesc.Length; i++)
        {
            charsSinceNewline++;
            if (sourceDesc[i] == newline)
            {
                lines++;
                charsSinceNewline = 0;
            }
            if (charsSinceNewline > windowWidth)
            {
                lines++;
                charsSinceNewline = 0;
            }
        }
        return lines > windowHeight;
    }


    protected override void OnChangeWindowSize()
    {
        paged = CheckNeedsPage();
    }

    protected override void PostDraw()
    {
        if (!videoInfo.windowOpen) return;
        int descriptionCharacter = 0;
        int extraDescChar = 0;
        int linkIndex = 0;
        ConsoleColor? foreground = null;
        ConsoleColor? background = null;
        var desc = sourceDesc;
        List<char> softDesc = desc.ToList();
        if (paged)
        {
            for (int i = 0; i < pageOffset; i++)
            {
                if (softDesc.Count <= windowWidth)
                {
                    break;
                }
                for (int j = 0; j < windowWidth; j++)
                {
                    bool isNewline = softDesc[0] == newline;
                    if (softDesc[0] == Convert.ToChar("⁒") || softDesc[0] == Convert.ToChar("▷"))
                    {
                        linkIndex++;
                    }
                    softDesc.RemoveAt(0);
                    if (isNewline) break;
                }
            }
        }
        desc = softDesc.ToArray();
        for (int j = Console.WindowTop; j < Console.WindowHeight; j++)
        {
            bool newLining = false;
            for (int i = Console.WindowLeft; i < Console.WindowWidth; i++)
            {
                if (InExtraWindow(i, j) && extraDescChar < extraData.Length)
                {
                    Globals.Write(i, j, extraData[extraDescChar]);
                    Globals.SetForegroundColor(i, j, Globals.defaultForeground);
                    extraDescChar++;
                }
                if (!InWindow(i, j)) continue;
                try
                {
                    Globals.activeScene.protectedTile[i, j] = true;
                }
                catch { }
                if (newLining || descriptionCharacter >= desc.Length)
                {
                    Globals.ClearTile(i, j);
                    continue;
                }
                if (descriptionCharacter >= desc.Length) continue;
                if (desc[descriptionCharacter] == newline && CheckNoEscape(descriptionCharacter, desc))
                {
                    Globals.Write(i, j, Convert.ToChar(" "));
                    descriptionCharacter++;
                    newLining = true;
                    continue;
                }
                if (desc[descriptionCharacter] == Convert.ToChar("⁒") || desc[descriptionCharacter] == Convert.ToChar("▷") && CheckNoEscape(descriptionCharacter, desc))
                {
                    var red = false;
                    linkIndex++;
                    if (linkIndex == selectedLink)
                    {
                        foreground = ConsoleColor.White;
                        background = ConsoleColor.Blue;
                        Globals.Write(i, j, Convert.ToChar("⇲"));
                        Globals.SetForegroundColor(i, j, (ConsoleColor)foreground);
                    }
                    else
                    {
                        if (desc[descriptionCharacter] == Convert.ToChar("▷"))
                        {
                            foreground = ConsoleColor.Red;
                            red = true;
                        }
                        else
                        {
                            foreground = ConsoleColor.Blue;
                        }
                        Globals.Write(i, j, Convert.ToChar(red ? "⏵" : "⇱"));
                        Globals.SetForegroundColor(i, j, (ConsoleColor)foreground);
                    }
                    descriptionCharacter++;
                    continue;
                }
                else if (desc[descriptionCharacter] == Convert.ToChar("⭖") && CheckNoEscape(descriptionCharacter, desc))
                {
                    foreground = null;
                    background = null;
                    Globals.Write(i, j, Convert.ToChar(" "));
                    descriptionCharacter++;
                    continue;
                }
                if (foreground != null)
                {
                    Globals.SetForegroundColor(i, j, (ConsoleColor)foreground);
                }
                else
                {
                    Globals.SetForegroundColor(i, j, Globals.defaultForeground);
                }
                if (background != null)
                {
                    Globals.SetBackgroundColor(i, j, (ConsoleColor)background);
                }
                if (desc[descriptionCharacter] != Convert.ToChar(" "))
                {
                    for (int l = descriptionCharacter; l < desc.Length; l++)
                    {
                        if (desc[l] == Convert.ToChar(" "))
                        {
                            if (l - descriptionCharacter >= windowWidth)
                            {
                                newLining = false;
                            }
                            break;
                        }
                        if (!InWindow(i + l - descriptionCharacter, j))
                        {
                            newLining = true;
                        }
                    }
                }
                if (newLining) continue;
                if (descriptionCharacter == 0)
                {
                    Globals.Write(i, j, desc[descriptionCharacter]);
                }
                else if (descriptionCharacter != Convert.ToChar(@"\"))
                {
                    Globals.Write(i, j, desc[descriptionCharacter]);
                }
                descriptionCharacter++;
            }
        }
    }

    public static bool CheckNoEscape(int descriptionCharacter, char[] desc)
    {
        if (descriptionCharacter - 1 < 0)
        {
            return true;
        }
        else if (desc[descriptionCharacter - 1] != Convert.ToChar(@"\"))
        {
            return true;
        }
        return false;
    }

    int linkNumber = 0;

    //Convert naked HTML tags into more pleasing substitutes
    private string ParseDescription(string description)
    {
        var parsed = description;
        parsed = parsed.Replace("⭖", @"\⭖");
        parsed = parsed.Replace("¤", @"\¤");
        parsed = parsed.Replace("⁒", @"\⁒");
        parsed = parsed.Replace("▷", @"\▷");
        parsed = parsed.Replace("<br>", "¤");
        parsed = parsed.Replace("<b>", "");
        parsed = parsed.Replace("</b>", "");
        linkNumber = Regex.Matches(parsed, "<a href=\"(.*?)\">").Count;
        parsed = Regex.Replace(parsed, "<a href=\"(.*?)\">", "[⁒$1⭖] ");
        parsed = parsed.Replace("</a>", "");
        parsed = parsed.Replace("&apos;", @"'");
        parsed = parsed.Replace("&quot;", "\"");
        parsed = parsed.Replace("&amp;", "&");
        parsed = parsed.Replace("&nbsp;", " ");
        parsed = Regex.Replace(parsed, @"(?<!\[⁒)http.?://[^ ¤]+(?!\])", "");
        parsed = Regex.Replace(parsed, @"⁒https://www\.youtube\.com/watch\?v=([^&]+)&t=[^ ]+?", "▷$1");
        parsed = Regex.Replace(parsed, @"⁒https://www\.youtube\.com/shorts/([^ ]+)", "▷$1");
        return parsed;
    }

    private int windowWidth = 0;

    private bool InExtraWindow(int i, int j)
    {
        if ((float)i > (float)Console.WindowWidth * (5f / 6f) || i < Console.WindowWidth / 6) return false;
        var extraWindowWidth = (int)((float)Console.WindowWidth * (5f / 6f)) - (Console.WindowWidth / 6);
        int heightMult = 1;
        while (extraData.Length > extraWindowWidth * heightMult)
        {
            heightMult++;
        }
        if (j < Console.WindowHeight / 6 - heightMult - 1 || j >= Console.WindowHeight / 6) return false;
        if (j == Console.WindowHeight / 6 - heightMult - 1 && i == Console.WindowWidth / 6)
        {
            Globals.Write(i, j, Convert.ToChar("┌"));
            Globals.SetForegroundColor(i, j, Globals.defaultForeground);
            return false;
        }
        if (j == Console.WindowHeight / 6 - heightMult - 1 && i == (int)((float)Console.WindowWidth * (5f / 6f)))
        {
            Globals.Write(i, j, Convert.ToChar("┐"));
            Globals.SetForegroundColor(i, j, Globals.defaultForeground);
            return false;
        }
        if (i == (int)((float)Console.WindowWidth * (5f / 6f)) || i == Console.WindowWidth / 6)
        {
            Globals.Write(i, j, "│ ");
            Globals.SetForegroundColor(i, j, Globals.defaultForeground);
            return false;
        }
        if (j == Console.WindowHeight / 6 - heightMult - 1)
        {
            Globals.Write(i, j, Convert.ToChar("─"));
            Globals.SetForegroundColor(i, j, Globals.defaultForeground);
            return false;
        }
        return true;
    }

    private bool InWindow(int i, int j)
    {
        if ((float)i > (float)Console.WindowWidth * (5f / 6f) || (float)j > (float)Console.WindowHeight * (5f / 6f) + 2) return false;
        if (i < Console.WindowWidth / 6 || j < Console.WindowHeight / 6) return false;
        try
        {
            Globals.activeScene.protectedTile[i, j] = true;
        }
        catch { }
        //Bottom left corner
        if (j == (int)((float)Console.WindowHeight * (5f / 6f)) + 2 && i == Console.WindowWidth / 6)
        {
            Globals.SetForegroundColor(i, j, Globals.defaultForeground);
            Globals.Write(i, j, "└");
            return false;
        }
        //Bottom right corner
        if (j == (int)((float)Console.WindowHeight * (5f / 6f)) + 2 && i == (int)((float)Console.WindowWidth * (5f / 6f)))
        {
            Globals.SetForegroundColor(i, j, Globals.defaultForeground);
            Globals.Write(i, j, "┘");
            return false;
        }
        //Left and right edges
        if (i == (int)((float)Console.WindowWidth * (5f / 6f)) || i == Console.WindowWidth / 6)
        {
            Globals.SetForegroundColor(i, j, Globals.defaultForeground);
            Globals.Write(i, j, "│ ");
            return false;
        }
        //Tip
        if (j == (int)((float)Console.WindowHeight * (5f / 6f) + 1) && i == Console.WindowWidth / 6 + 1)
        {
            Globals.Write(i, j, "Close window with \"q\".", out var newPos);
            if (linkNumber > 0)
            {
                Globals.Write(newPos, j, ". Select and open links with ⬅, ➡, and Space.");
            }
            return false;
        }
        if (j == (int)((float)Console.WindowHeight * (5f / 6f) + 1))
        {
            return false;
        }
        if (j == Console.WindowHeight / 6 || j == (int)((float)Console.WindowHeight * (5f / 6f)) || j == (int)((float)Console.WindowHeight * (5f / 6f)) + 2)
        {
            Globals.Write(i, j, Convert.ToChar("─"));
            Globals.SetForegroundColor(i, j, Globals.defaultForeground);
            return false;
        }
        windowWidth = (int)((float)Console.WindowWidth * (5f / 6f)) - (Console.WindowWidth / 6);
        windowHeight = (int)((float)Console.WindowHeight * (5f / 6f)) - (Console.WindowHeight / 6);
        windowSpace = windowWidth * windowHeight;
        return true;
    }
    int windowSpace = 0;
    int windowHeight = 0;

    protected override async Task OnCheckKeys(ConsoleKey key)
    {
        if (key == ConsoleKey.Q && activeBecause == InactiveReason.Playing)
        {
            videoInfo.mediaPlayer.Kill();
        }
        if (key == ConsoleKey.OemPlus && activeBecause == InactiveReason.Playing)
        {
            activeBecause = InactiveReason.Active;
            Reset();
            Console.Clear();
        }
        if (!videoInfo.windowOpen) return;
        if (key == ConsoleKey.Q)
        {
            videoInfo.windowOpen = false;
        }
        if (key == ConsoleKey.LeftArrow || key == ConsoleKey.H || key == ConsoleKey.A)
        {
            if (selectedLink > 1)
            {
                selectedLink--;
            }
            else
            {
                selectedLink = linkNumber;
            }
        }
        if (key == ConsoleKey.RightArrow || key == ConsoleKey.L || key == ConsoleKey.D)
        {
            if (selectedLink < linkNumber)
            {
                selectedLink++;
            }
            else
            {
                selectedLink = 1;
            }
        }
        if (key == ConsoleKey.UpArrow || key == ConsoleKey.K || key == ConsoleKey.W)
        {
            if (pageOffset > 0)
            {
                pageOffset--;
            }
            if (Globals.debug)
            {
                Console.WriteLine(pageOffset);
            }
        }
        if (key == ConsoleKey.DownArrow || key == ConsoleKey.J || key == ConsoleKey.S)
        {
            pageOffset++;
            if (Globals.debug)
            {
                Console.WriteLine(pageOffset);
            }
        }
        if (key == ConsoleKey.Enter || key == ConsoleKey.Spacebar && selectedLink > 0)
        {
            List<string> links = new();
            bool markingLink = false;
            string curLink = "";
            for (int i = 0; i < sourceDesc.Length; i++)
            {
                if (markingLink && !(sourceDesc[i] == Convert.ToChar("⁒") || sourceDesc[i] == Convert.ToChar("▷") || sourceDesc[i] == Convert.ToChar("⭖")) && CheckNoEscape(i, sourceDesc))
                {
                    curLink += sourceDesc[i];
                }
                if ((sourceDesc[i] == Convert.ToChar("⁒") || sourceDesc[i] == Convert.ToChar("▷")) && CheckNoEscape(i, sourceDesc))
                {
                    markingLink = true;
                    curLink = "";
                }
                else if (sourceDesc[i] == Convert.ToChar("⭖") && CheckNoEscape(i, sourceDesc))
                {
                    markingLink = false;
                    links.Add(curLink);
                }
            }
            if (links[selectedLink - 1].Contains("https://"))
            {
                var browser = new Process();
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    browser.StartInfo.FileName = links[selectedLink - 1];
                    browser.StartInfo.UseShellExecute = true;
                }
                else
                {
                    browser.StartInfo.FileName = "open";
                    browser.StartInfo.Arguments = links[selectedLink - 1];
                }
                browser.Start();
            }
            else
            {
                LoadBar.loadMessage = "Opening video";
                LoadBar.visible = true;
                var descriptionVideo = await Scenes.DescriptionVideo.CreateAsync(links[selectedLink - 1]);
                LoadBar.visible = false;
                Globals.scenes.Push(descriptionVideo);
            }
        }
    }
    int selectedLink = 0;
}
