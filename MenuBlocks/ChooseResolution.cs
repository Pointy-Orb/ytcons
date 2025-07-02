using Iso639;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace YTCons.MenuBlocks;

public class ChooseResolution : MenuBlock
{
    public bool inactiveBecausePlaying = false;
    private ExtractedVideoInfo info;
    private string chosenFormat;

    public ChooseResolution(ExtractedVideoInfo info, string chosenFormat, bool download, AnchorType anchorType = AnchorType.Cursor) : base(anchorType)
    {
        this.info = info;
        this.chosenFormat = chosenFormat;
        List<string> qualities = new();
        foreach (FormatData format in info.video.Formats.Where(i => i.Protocol == "https").Where(i => i.Vcodec != "none"))
        {
            if (format.FormatNote == "") continue;
            if (!qualities.Contains(format.FormatNote))
            {
                qualities.Add(format.FormatNote);
            }
        }
        foreach (string quality in qualities)
        {
            options.Add(new MenuOption(quality, this, download ? () => Download(quality) : () => Play(quality)));
        }
        options[cursor].selected = true;
    }

    private async Task Download(string quality)
    {
        await info.Download(chosenFormat, quality);
    }

    private async Task Play(string quality)
    {
        active = false;
        await info.Play(chosenFormat, quality);
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
