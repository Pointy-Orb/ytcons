namespace YTCons;

public static class Program
{
    public static async Task Main(string[] args)
    {
        Console.CursorVisible = false;
        await Globals.Init(args);
        if (!Globals.debug)
        {
            Console.Clear();
        }
        while (true)
        {
            Globals.Draw();
            await Globals.Update();
            Thread.Sleep(40);
        }
    }
}
