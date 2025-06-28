namespace YTCons.MenuBlocks;

public class ChooseFormat : MenuBlock
{
    public ChooseFormat(ExtractedVideoInfo videoInfo, AnchorType anchorType = AnchorType.Cursor) : base(anchorType)
    {
        List<string> formats = new List<string>();
        foreach (VideoStream stream in videoInfo.video.videoStreams)
        {
            if (!formats.Contains(stream.format))
            {
                formats.Add(stream.format);
            }
        }
        foreach (string format in formats)
        {
            options.Add(new MenuOption(format, this, () => Globals.activeScene.PushMenuAsync(new ChooseResolution(videoInfo, format))));
        }
        options[cursor].selected = true;
    }
}
