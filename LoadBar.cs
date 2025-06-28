namespace YTCons;

public static class LoadBar
{
    public static string loadMessage = "Loading";

    private static string[] frames = new string[]
    {
        "    [⬤•••••••••••••]",
        "    [•⬤••••••••••••]",
        "    [••⬤•••••••••••]",
        "    [•••⬤••••••••••]",
        "    [••••⬤•••••••••]",
        ".   [•••••⬤••••••••]",
        ".   [••••••⬤•••••••]",
        ".   [•••••••⬤••••••]",
        ".   [••••••••⬤•••••]",
        ".   [•••••••••⬤••••]",
        ".   [••••••••••⬤•••]",
        "..  [•••••••••••⬤••]",
        "..  [••••••••••••⬤•]",
        "..  [•••••••••••••⬤]",
        "..  [••••••••••••⬤•]",
        "..  [•••••••••••⬤••]",
        "..  [••••••••••⬤•••]",
        "..  [•••••••••⬤••••]",
        "... [••••••••⬤•••••]",
        "... [•••••••⬤••••••]",
        "... [••••••⬤•••••••]",
        "... [•••••⬤••••••••]",
        "... [••••⬤•••••••••]",
        "... [•••⬤••••••••••]",
        "... [••⬤•••••••••••]",
        "... [•⬤••••••••••••]",
    };

    private static string empty
    {
        get
        {
            var preEmpty = "";
            for (int i = 0; i < frames[0].Length + prevLoadMessage.Length; i++)
            {
                preEmpty += " ";
            }
            return preEmpty;
        }
    }

    public static bool visible = false;

    private static int curFrame = 0;

    private static string prevLoadMessage = "Loading";

    public static void WriteLoad()
    {
        ClearLoad();
        if (!visible) return;
        prevLoadMessage = loadMessage;
        Console.SetCursorPosition(0, Console.WindowHeight);
        Console.Write(loadMessage + frames[curFrame]);
        curFrame++;
        if (curFrame >= frames.Length)
        {
            curFrame = 0;
        }
        Thread.Sleep(50);
    }

    public static void ClearLoad()
    {
        Console.SetCursorPosition(0, Console.WindowHeight);
        Console.Write(empty);
    }
}
