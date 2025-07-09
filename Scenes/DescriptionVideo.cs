using YTCons.MenuBlocks;

namespace YTCons.Scenes;

public class DescriptionVideo : Scene
{
    string id;
    public ExtractedVideoInfo info = null!;

    public static async Task<DescriptionVideo> CreateAsync(string id)
    {
        var instance = new DescriptionVideo(id);
        var info = await ExtractedVideoInfo.CreateAsync(id);
        instance.info = info;
        MenuBlock block = new();
        block.options.Add(new MenuOption(info.video.Title, block, () => Task.Run(() => Globals.activeScene.PushMenu(new VideoBlock(info)))));
        block.options.Add(new MenuOption("Back", block, () => Task.Run(() => { block.resetNextTick = true; Globals.scenes.Pop(); })));
        block.options[block.cursor].selected = true;
        instance.PushMenu(block);
        return instance;
    }

    private DescriptionVideo(string id)
    {
        this.id = id;
    }
}
