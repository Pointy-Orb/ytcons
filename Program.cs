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
            if (Globals.debug)
            {
                Console.SetCursorPosition(0, 2);
                Console.WriteLine("i updated " + DateTime.Now.ToString());
            }
            Thread.Sleep(40);
            if (Globals.debug)
            {
                Console.SetCursorPosition(0, 4);
                Console.WriteLine("i waited " + DateTime.Now.ToString());
            }
        }
    }
}
