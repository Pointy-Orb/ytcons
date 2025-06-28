namespace YTCons.MenuBlocks;

public class ChooseResolution : MenuBlock
{
    public bool inactiveBecausePlaying = false;
    private ExtractedVideoInfo info;
    private string chosenFormat;

    public ChooseResolution(ExtractedVideoInfo info, string chosenFormat, AnchorType anchorType = AnchorType.Cursor) : base(anchorType)
    {
        this.info = info;
        this.chosenFormat = chosenFormat;
        List<string> resolutions = new List<string>();
        foreach (VideoStream stream in info.video.videoStreams)
        {
            if (!resolutions.Contains(stream.quality) && stream.format == chosenFormat)
            {
                resolutions.Add(stream.quality);
            }
        }
        foreach (string resolution in resolutions)
        {
            options.Add(new MenuOption(resolution, this, () => Play(resolution)));
        }
        options[cursor].selected = true;
    }

    private async Task Play(string resolution)
    {
        active = false;
        await info.Play(chosenFormat, resolution);
        inactiveBecausePlaying = true;
    }

    protected override void OnUpdate()
    {
        if (inactiveBecausePlaying && info.mediaPlayer.HasExited)
        {
            inactiveBecausePlaying = false;
            Reset();
            Console.Clear();
        }
    }
}
