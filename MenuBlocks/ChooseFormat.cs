namespace YTCons.MenuBlocks;

public class ChooseFormat : MenuBlock
{
    public ChooseFormat(ExtractedVideoInfo videoInfo, bool dowloading = false, AnchorType anchorType = AnchorType.Cursor) : base(anchorType)
    {
        List<string> formats = new();
        foreach (FormatData stream in videoInfo.video.Formats.Where(i => i.Vcodec != "none").Where(i => i.Protocol == "https"))
        {
            if (!formats.Contains(stream.Ext))
            {
                formats.Add(stream.Ext);
            }
        }
        foreach (string format in formats)
        {
            options.Add(new MenuOption(format.ToString(), this, () => Task.Run(() => Globals.activeScene.PushMenu(new ChooseResolution(videoInfo, format, dowloading)))));
        }
        if (dowloading)
        {
            options.Add(new MenuOption("Audio Only", this, () => DownloadAudio(videoInfo)));
        }
        options[cursor].selected = true;
    }

    private async Task DownloadAudio(ExtractedVideoInfo videoInfo)
    {
        await videoInfo.Download("mp3", "144p");
        Globals.activeScene.PopMenu();
        Globals.activeScene.PopMenu();
    }
}
