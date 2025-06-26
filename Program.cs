namespace YTCons;

public static class Program
{
    public static void Main(string[] args)
    {
        Console.CursorVisible = false;
        Globals.Init(args);
        if (!Globals.debug)
        {
            Console.Clear();
        }
        while (true)
        {
            Globals.Draw();
            Globals.Update();
            Thread.Sleep(40);
        }
    }
}
