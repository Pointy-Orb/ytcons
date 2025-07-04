namespace YTCons;

public static class LoadBar
{
    public static string loadMessage = "Loading";

    public static string loadMessageDebug
    {
        set
        {
            if (Globals.debug)
            {
                loadMessage = value;
            }
        }
    }

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

    private static bool wasVisible = false;

    public static void WriteLoad()
    {
        ActuallyWriteLog();
        //Make sure the bottom bar can be used for other things
        if (!visible && wasVisible) ClearLoad();
        wasVisible = visible;

        if (!visible) return;
        if (prevLoadMessage.Length > loadMessage.Length) ClearLoad();
        prevLoadMessage = loadMessage;

        Console.SetCursorPosition(0, Console.WindowHeight);
        Console.Write(loadMessage + frames[curFrame]);
        curFrame++;
        if (curFrame >= frames.Length)
        {
            curFrame = 0;
        }
        ActuallyWriteLog();
    }

    private static int logTime = 0;
    private static string log = "";

    private static void ActuallyWriteLog()
    {
        if (logTime <= 0) return;
        logTime--;
        ClearLoad();
        Console.SetCursorPosition(0, Console.WindowHeight);
        Console.Write(log);
        if (logTime == 0)
        {
            for (int i = 0; i < log.Length; i++)
            {
                Console.SetCursorPosition(i, Console.WindowHeight);
                Console.Write(" ");
            }
        }
    }

    public static void WriteLog(string log)
    {
        LoadBar.log = log;
        //Make it easier to read longer log messages
        logTime = log.Length * 3;
        if (logTime < 40) logTime = 40;
        Console.SetCursorPosition(0, Console.WindowHeight);
        Console.Write(log);
    }

    public static void WriteTip(string tip)
    {
        Globals.Write(0, Console.WindowHeight - 1, tip);
    }

    public static void ClearLoad()
    {
        Console.SetCursorPosition(0, Console.WindowHeight);
        Console.Write(empty);
    }
}
