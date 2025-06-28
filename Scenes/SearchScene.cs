using YTCons.MenuBlocks;
using YTSearch.NET;

namespace YTCons.Scenes;

public class SearchScene : Scene
{
    public static async Task<SearchScene> CreateAsync(string query)
    {
        var instance = new SearchScene();
        Console.WriteLine("they say askfdjl;a like this is irresponsible");
        await instance.PushMenuAsync(new MenuBlock(AnchorType.Top));
        Console.WriteLine("they say debuggig like this is debuggig");
        instance.PeekMenu().drawOffset = 5;
        instance.PeekMenu().grayUnselected = true;
        instance.PeekMenu().options.Add(new MenuOption("Back", instance.PeekMenu(), () => Task.Run(Globals.scenes.Pop)));
        instance.PeekMenu().options[instance.PeekMenu().cursor].selected = true;
        if (query == null)
        {
            Globals.Exit(1);
        }
        instance.SearchQuery(query);
        return instance;
    }

    private SearchScene()
    {
    }

    public async Task SearchQuery(string query)
    {
        var client = new YouTubeSearchClient();
        var results = await client.SearchYoutubeVideoAsync(query);
        var resultMenus = new List<Task>();
        foreach (var result in results.Results)
        {
            resultMenus.Add(SearchQueryInner(result));
        }
        await Task.WhenAll(resultMenus);
        Globals.Write(0, Console.WindowHeight - 1, "Finished fetching results");
    }

    private async Task SearchQueryInner(SearchedYouTubeVideo result)
    {
        try
        {
            var menu = await VideoBlock.CreateAsync(result.VideoId);
            menus.Reverse().First().options.Add(new MenuOption($"{result.Author} | {result.Title} | {result.Length}", PeekMenu(), () => PushMenuAsync(menu)));
        }
        catch { }
    }

    internal override async Task<bool> OnCheckKeys(ConsoleKeyInfo key)
    {
        var menu = PeekMenu() as VideoBlock;
        if (menu != null && menu.videoInfo.playing && !menu.videoInfo.mediaPlayer.HasExited)
        {
            return false;
        }
        return true;
    }
}
