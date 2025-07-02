using YTCons.MenuBlocks;

namespace YTCons.Scenes;

public class DescriptionVideo : Scene
{
    public static async Task<DescriptionVideo> CreateAsync(string id)
    {
        var instance = new DescriptionVideo(id);
        var info = await ExtractedVideoInfo.CreateAsync(id);
        MenuBlock block = new();
        block.options.Add(new MenuOption(info.video.Title, block, () => Globals.activeScene.PushMenuAsync(new VideoBlock(info))));
        block.options.Add(new MenuOption("Back", block, () => Task.Run(() => Globals.scenes.Pop())));
        block.options[block.cursor].selected = true;
        instance.PushMenu(block);
        return instance;
    }

    private DescriptionVideo(string id)
    {
    }
}
