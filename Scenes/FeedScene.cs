using System.Xml;
using Newtonsoft.Json;
using YTCons.MenuBlocks;

namespace YTCons.Scenes
{

    public class FeedScene : Scene
    {
        private FeedScene() { }

        public static async Task<FeedScene> CreateAsync()
        {
            var instance = new FeedScene();
            var menu = await FeedChannelMenu.CreateAsync("UC5Yo88QF-chdugJbAnB2tUw");
            instance.PushMenu(menu);
            return instance;
        }
    }
}
