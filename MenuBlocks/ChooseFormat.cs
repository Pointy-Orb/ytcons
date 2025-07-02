using YoutubeDLSharp.Metadata;

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
            options.Add(new MenuOption(format.ToString(), this, () => Globals.activeScene.PushMenuAsync(new ChooseResolution(videoInfo, format, dowloading))));
        }
        options[cursor].selected = true;
    }
}
