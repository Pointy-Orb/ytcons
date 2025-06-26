using YTCons.MenuBlocks;

namespace YTCons.Scenes;

public class DescriptionVideo : Scene
{
    public DescriptionVideo(string id)
    {
        var info = new ExtractedVideoInfo(id);
        while (!info.gotVideo)
        {
            LoadBar.WriteLoad();
        }
        LoadBar.ClearLoad();
        MenuBlock block = new();
        block.options.Add(new MenuOption(info.video.title, block, () => menus.Push(new VideoBlock(info, id))));
        block.options.Add(new MenuOption("Back", block, () => Globals.scenes.Pop()));
        block.options[block.cursor].selected = true;
        PushMenu(block);
    }
}
