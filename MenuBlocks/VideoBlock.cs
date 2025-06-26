using System.Diagnostics;
using System.Text;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace YTCons.MenuBlocks;

public class VideoBlock : MenuBlock
{
    public readonly string videoID;
    public readonly string extraData;
    public readonly ExtractedVideoInfo videoInfo;
    private enum InactiveReason
    {
        Active,
        ShowingDeets,
        Playing,
        ShowingThumbnail
    }
    private InactiveReason activeBecause;
    string thumbnailPath;
    char[] desc;
    public VideoBlock(string id, AnchorType anchorType = AnchorType.Cursor) : base(anchorType)
    {
        videoID = id;
        videoInfo = new ExtractedVideoInfo(id);
        while (!videoInfo.gotVideo)
        {
            LoadBar.WriteLoad();
        }
        LoadBar.ClearLoad();
        thumbnailPath = Path.GetTempPath() + videoID + ".webp";
        extraData = MakeExtraData();
        desc = FinishConstructor();
    }

    public VideoBlock(ExtractedVideoInfo info, string id, AnchorType anchorType = AnchorType.Cursor) : base(anchorType)
    {
        videoID = id;
        videoInfo = info;
        thumbnailPath = Path.GetTempPath() + videoID + ".webp";
        extraData = MakeExtraData();
        desc = FinishConstructor();
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
        extraDataBuilder.Append(videoInfo.video.views.ToString("N0") + "  |  👍 Likes: ");
        extraDataBuilder.Append(videoInfo.video.likes.ToString("N0") + $"  {Ratio(videoInfo.video.likes, videoInfo.video.dislikes)}  👎 Dislikes: ");
        extraDataBuilder.Append(videoInfo.video.dislikes.ToString("N0"));
        extraDataBuilder.Append("  |📺 Channel: " + (videoInfo.video.uploaderVerified ? "✔️" : "") + videoInfo.video.uploader);
        extraDataBuilder.Append($" (目 Subs: {videoInfo.video.uploaderSubscriberCount.ToString("N0")})");
        extraDataBuilder.Append($"  | 𐁗  Uploaded on {videoInfo.video.uploadDate.ToString()}");
        extraDataBuilder.Append("  | 🕓 Duration: " + videoInfo.ParsedDuration());
        return extraDataBuilder.ToString();
    }

    private char[] FinishConstructor()
    {
        options.Add(new MenuOption("Play", this, () => Play(), () => Globals.activeScene.PushMenu(new ChooseFormat(videoInfo))));
        options.Add(new MenuOption("Show Description", this, () =>
        {
            active = false;
            activeBecause = InactiveReason.ShowingDeets;
            videoInfo.windowOpen = true;
        }));
        options.Add(new MenuOption("Show Thumbnail", this, () =>
        {
            active = false;
            activeBecause = InactiveReason.ShowingThumbnail;
            Task.Run(ShowThumbnail);
            while (!gotThumbnail)
            {
                LoadBar.loadMessage = "Getting thumbnail";
                LoadBar.WriteLoad();
            }
            LoadBar.ClearLoad();
        }));
        options[cursor].selected = true;
        return ParseDescription(videoInfo.video.description).ToCharArray();
    }

    private void Play()
    {
        active = false;
        videoInfo.Play();
        activeBecause = InactiveReason.Playing;
    }

    private Process imageViewer = new();

    bool gotThumbnail = false;
    private async Task ShowThumbnail()
    {
        try
        {
            if (!File.Exists(thumbnailPath))
            {
                using (HttpClient client = new HttpClient())
                {
                    var thumbnailUrl = new Uri(videoInfo.video.thumbnailUrl);
                    byte[] thumbnailBytes = await client.GetByteArrayAsync(thumbnailUrl);
                    await File.WriteAllBytesAsync(thumbnailPath, thumbnailBytes);
                }
            }
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
                imageViewer.StartInfo = new ProcessStartInfo
                {
                    FileName = "/usr/bin/open",
                    Arguments = thumbnailPath
                };
            }
            imageViewer.StartInfo.RedirectStandardInput = true;
            imageViewer.StartInfo.RedirectStandardError = true;
            imageViewer.Start();
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
        }
        if (activeBecause == InactiveReason.Playing && videoInfo.mediaPlayer.HasExited)
        {
            activeBecause = InactiveReason.Active;
            Reset();
        }
        if (activeBecause == InactiveReason.ShowingThumbnail)
        {
            activeBecause = InactiveReason.Active;
            Reset();
        }
    }

    int pageOffset = 0;

    protected override void PostDraw()
    {
        if (activeBecause == InactiveReason.Playing)
        {
            Globals.Write(0, Console.WindowHeight - 1, videoInfo.mediaPlayer.StandardError.ReadLine());
        }
        else
        {
            for (int i = 0; i < Console.WindowWidth; i++)
            {
                Globals.Write(i, Console.WindowHeight - 1, Convert.ToChar(" "));
            }
        }
        if (!videoInfo.windowOpen) return;
        int descriptionCharacter = 0;
        int extraDescChar = 0;
        int linkIndex = 0;
        bool blue = false;
        ConsoleColor? foreground = null;
        ConsoleColor? background = null;
        var desc = this.desc;
        if (pageDescription)
        {
            List<string> lines = new();
            for (int i = 0; i < desc.Length / windowWidth; i++)
            {
                string line = "";
                for (int j = i * windowWidth; j < (i + 1) * windowWidth; j++)
                {
                    line += desc[j];
                }
                lines.Add(line);
            }
            for (int i = 0; i < pageOffset; i++)
            {
                lines.RemoveAt(i);
            }
            if (lines.Count > windowHeight)
            {
                lines.RemoveAll(item => lines.IndexOf(item) > windowHeight);
            }
            string newDesc = "";
            foreach (string line in lines)
            {
                newDesc += line;
            }
            desc = newDesc.ToCharArray();
        }
        for (int j = Console.WindowTop; j < Console.WindowHeight; j++)
        {
            bool newLining = false;
            for (int i = Console.WindowLeft; i < Console.WindowWidth; i++)
            {
                if (InExtraWindow(i, j) && extraDescChar < extraData.Length)
                {
                    Globals.Write(i, j, extraData[extraDescChar]);
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
                if (desc[descriptionCharacter] == Convert.ToChar("¤") && CheckNoEscape(descriptionCharacter, desc))
                {
                    descriptionCharacter++;
                    newLining = true;
                    continue;
                }
                if (desc[descriptionCharacter] == Convert.ToChar("⁒") || desc[descriptionCharacter] == Convert.ToChar("▷") && CheckNoEscape(descriptionCharacter, desc))
                {
                    blue = !blue;
                    var red = false;
                    if (blue)
                    {
                        linkIndex++;
                        if (linkIndex == selectedLink)
                        {
                            foreground = ConsoleColor.White;
                            background = ConsoleColor.Blue;
                            Globals.Write(i, j, Convert.ToChar("⇲"));
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
                        }
                    }
                    else
                    {
                        foreground = null;
                        background = null;
                    }
                    descriptionCharacter++;
                    continue;
                }
                if (foreground != null)
                {
                    Globals.SetForegroundColor(i, j, (ConsoleColor)foreground);
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

    private bool CheckNoEscape(int descriptionCharacter, char[] desc)
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
        parsed = parsed.Replace("¤", @"\¤");
        parsed = parsed.Replace("⁒", @"\⁒");
        parsed = parsed.Replace("▷", @"\▷");
        parsed = parsed.Replace("<br>", "¤");
        linkNumber = Regex.Matches(parsed, "<a href=\"(.*?)\">").Count;
        parsed = Regex.Replace(parsed, "<a href=\"(.*?)\">", "[⁒$1⁒] ");
        parsed = parsed.Replace("</a>", "");
        parsed = parsed.Replace("&apos;", @"'");
        parsed = parsed.Replace("&quot;", "\"");
        parsed = parsed.Replace("&amp;", "&");
        parsed = parsed.Replace("&nbsp;", " ");
        parsed = Regex.Replace(parsed, @"(?<!\[⁒)http.?://[^ ¤]+(?!⁒\])", "");
        parsed = Regex.Replace(parsed, @"⁒https://www\.youtube\.com/watch\?v=([^&]+)&t=[^ ]+?", "▷$1");
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
        if (i == (int)((float)Console.WindowWidth * (5f / 6f)) || i == Console.WindowWidth / 6)
        {
            Globals.Write(i, j, Convert.ToChar("|"));
            return false;
        }
        if (j == Console.WindowHeight / 6 - heightMult - 1)
        {
            Globals.Write(i, j, Convert.ToChar("—"));
            return false;
        }
        return true;
    }

    private bool InWindow(int i, int j)
    {
        if ((float)i > (float)Console.WindowWidth * (5f / 6f) || (float)j > (float)Console.WindowHeight * (5f / 6f) + 1) return false;
        if (i < Console.WindowWidth / 6 || j < Console.WindowHeight / 6) return false;
        try
        {
            Globals.activeScene.protectedTile[i, j] = true;
        }
        catch { }
        if (i == (int)((float)Console.WindowWidth * (5f / 6f)) || i == Console.WindowWidth / 6)
        {
            Globals.Write(i, j, Convert.ToChar("|"));
            return false;
        }
        if (j == (int)((float)Console.WindowHeight * (5f / 6f) + 1) && i == Console.WindowWidth / 6 + 1)
        {
            Globals.Write(i, j, "Close window with \"q\".", out var newPos);
            if (linkNumber > 0)
            {
                Globals.Write(newPos, j, " Select and open links with ⬅, ➡, and Space.");
            }
            for (int l = i; l <= Console.CursorLeft; l++)
            {
                try
                {
                    Globals.activeScene.protectedTile[l, j] = true;
                }
                catch { }
            }
            return false;
        }
        if (j == Console.WindowHeight / 6 || j == (int)((float)Console.WindowHeight * (5f / 6f)))
        {
            Globals.Write(i, j, Convert.ToChar("—"));
            return false;
        }
        windowWidth = (int)((float)Console.WindowWidth * (5f / 6f)) - (Console.WindowWidth / 6);
        windowHeight = (int)((float)Console.WindowHeight * (5f / 6f)) - (Console.WindowHeight / 6);
        windowSpace = windowWidth * windowHeight;
        if (desc.Length > windowSpace)
        {
            pageDescription = true;
        }
        else
        {
            pageDescription = false;
        }
        return true;
    }
    int windowSpace = 0;
    int windowHeight = 0;
    bool pageDescription = false;

    protected override void OnCheckKeys(ConsoleKey key)
    {
        if (key == ConsoleKey.Q && activeBecause == InactiveReason.Playing)
        {
            videoInfo.mediaPlayer.Kill();
        }
        if (key == ConsoleKey.Backspace && activeBecause == InactiveReason.Playing)
        {
            activeBecause = InactiveReason.Active;
            Reset();
        }
        if (!videoInfo.windowOpen) return;
        if (key == ConsoleKey.Q)
        {
            videoInfo.windowOpen = false;
        }
        if (key == ConsoleKey.LeftArrow)
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
        if (key == ConsoleKey.RightArrow)
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
        if (key == ConsoleKey.UpArrow)
        {
            if (pageOffset > 0)
            {
                pageOffset--;
            }
        }
        if (key == ConsoleKey.DownArrow)
        {
            pageOffset++;
        }
        if (key == ConsoleKey.Enter || key == ConsoleKey.Spacebar && selectedLink > 0)
        {
            List<string> links = new();
            bool markingLink = false;
            string curLink = "";
            for (int i = 0; i < desc.Length; i++)
            {
                if (markingLink && !(desc[i] == Convert.ToChar("⁒") || desc[i] == Convert.ToChar("▷")) && CheckNoEscape(i, desc))
                {
                    curLink += desc[i];
                }
                if ((desc[i] == Convert.ToChar("⁒") || desc[i] == Convert.ToChar("▷")) && CheckNoEscape(i, desc))
                {
                    markingLink = !markingLink;
                    if (markingLink)
                    {
                        curLink = "";
                    }
                    else
                    {
                        links.Add(curLink);
                    }
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
                Globals.scenes.Push(new Scenes.DescriptionVideo(links[selectedLink - 1]));
            }
        }
    }
    int selectedLink = 0;
}
