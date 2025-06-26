using YTCons.MenuBlocks;

namespace YTCons.Scenes;

public class TestScene : Scene
{
    public TestScene()
    {
        PushMenu(new VideoBlock("0dJzhnejrF4"));
    }

    internal override bool OnCheckKeys(ConsoleKeyInfo key)
    {
        var menu = PeekMenu() as VideoBlock;
        if (menu != null && menu.videoInfo.playing && !menu.videoInfo.mediaPlayer.HasExited)
        {
            return false;
        }
        return true;
    }
}
