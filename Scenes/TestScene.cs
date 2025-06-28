using YTCons.MenuBlocks;
using YTSearch.NET;

namespace YTCons.Scenes;

public class TestScene : Scene
{
    public static async Task<TestScene> CreateAsync()
    {
        var instance = new TestScene();
        return instance;
    }

    public async Task DoSearch()
    {
        Console.Write($"you look weary, traveler.{Environment.NewLine}input the video you would like to watch: ");
        var query = Globals.ReadLine(0, Console.WindowHeight, "");
        Console.WriteLine($"You searched: {query}");
        Console.ReadKey();
        Console.WriteLine("starting the query");
        Globals.scenes.Push(await SearchScene.CreateAsync(query));
        Console.WriteLine("pushing the query");
    }

    protected override void OnUpdate()
    {
        Console.WriteLine("you came back, how was it");
        var query = Globals.ReadLine(0, Console.WindowHeight, "");
        Console.WriteLine("jk you can never leave");
    }

    private TestScene()
    {
    }
}
