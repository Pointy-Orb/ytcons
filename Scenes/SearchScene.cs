using YTCons.MenuBlocks;
using YTSearch.NET;
using System.Text.RegularExpressions;

namespace YTCons.Scenes;

public class SearchScene : Scene
{
    public static async Task<SearchScene> CreateAsync(string query)
    {
        var instance = new SearchScene();
        instance.PushMenu(new MenuBlock(AnchorType.Top));
        instance.PeekMenu().drawOffset = 5;
        instance.PeekMenu().grayUnselected = true;
        instance.PeekMenu().options.Add(new MenuOption("Back", instance.PeekMenu(), () => Task.Run(() => { LoadBar.visible = false; Globals.scenes.Pop(); })));
        instance.PeekMenu().options[instance.PeekMenu().cursor].selected = true;
        if (query == null)
        {
            Globals.Exit(1);
        }
        LoadBar.loadMessage = "Fetching results";
        LoadBar.visible = true;
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
        LoadBar.visible = false;
        LoadBar.WriteLog("Finished getting results");
    }

    private async Task SearchQueryInner(SearchedYouTubeVideo result)
    {
        try
        {
            var menu = await VideoBlock.CreateAsync(result.VideoId);
            menus.Reverse().First().options.Add(new MenuOption($"{result.Author} | {result.Title} | {result.Length}", menus.Reverse().First(), () => Task.Run(() => PushMenu(menu))));
        }
        catch { }
    }
}
