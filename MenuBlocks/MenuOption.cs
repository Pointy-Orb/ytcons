namespace YTCons.MenuBlocks;

public class MenuOption
{
    public virtual int counter { get; set; } = 0;
    public bool useCounter = false;

    public string? extraData = null;

    //Set this to not null to have a tip about the item display at the bottom of the screen
    public string? tip = null;

    public bool selected = false;

    public bool HasAltSelect
    {
        get { return _altOnSelected != null; }
    }

    public string option;

    protected Func<Task> _onSelected;

    protected Func<Task>? _altOnSelected;

    public MenuBlock parent;

    public MenuBlock? childMenu;

    public int drawEnd { get; private set; }

    public MenuOption(string option, MenuBlock parent, Func<Task> onSelected, Func<Task>? altOnSelected = null)
    {
        this.option = option;
        this.parent = parent;
        this._onSelected = onSelected;
        this._altOnSelected = altOnSelected;
    }

    public MenuOption(string option, MenuBlock parent, Func<Task> onSelected, MenuBlock childMenu, Func<Task>? altOnSelected = null)
    {
        this.option = option;
        this.parent = parent;
        this._onSelected = onSelected;
        this._altOnSelected = altOnSelected;
        this.childMenu = childMenu;
    }

    public void ChangeOnSelected(Func<Task> newAction)
    {
        _onSelected = newAction;
    }

    public async Task OnSelected()
    {
        Task task = _onSelected();
        await task;
    }

    public async Task AltOnSelected()
    {
        Task task = _altOnSelected();
        await task;
    }

    public void Update()
    {

    }

    int drawX;
    int drawY;

    protected virtual void PostDraw(int i, int j)
    { }

    public virtual void PostDrawEverything() { }

    protected virtual void PreDraw(int i, int j) { }

    public static int DivideWithPowers(int n, int d)
    {
        int divisor = d;
        int i = 0;
        while ((float)n / (float)divisor >= 1)
        {
            i++;
            divisor *= d;
        }
        return i;
    }

    public void Draw(int i, int j, int prevMenuOffset, out int nextMenuOffset)
    {
        if (selected && !parent.confirmed && tip != null)
        {
            LoadBar.WriteTip(tip);
        }
        drawX = i;
        drawY = j;
        if (selected)
        {
            if (parent.confirmed && parent == Globals.activeScene.PeekMenu())
            {
                SafeWrite(" ->   ", out drawX);
            }
            else
            {
                SafeWrite(" >  ", out drawX);
            }
        }
        else
        {
            SafeWrite("    ", out drawX);
        }
        if (useCounter && counter > 0)
        {
            int digits = DivideWithPowers(counter, 10) + 1;
            int preDrawX = drawX;
            SafeWrite($"({counter}) ", out drawX);
            for (int l = preDrawX + 1; l <= preDrawX + digits; l++)
            {
                Globals.SetForegroundColor(l, j, ConsoleColor.Yellow);
            }
        }
        SafeWrite(option, out drawX);
        for (int l = prevMenuOffset; l < drawX; l++)
        {
            SafeWrite(" ", out _);
        }
        nextMenuOffset = drawX;
        PostDraw(i, j);
    }

    private void SafeWrite(string input, out int newX)
    {
        newX = drawX;
        try
        {
            if (!Globals.activeScene.protectedTile[drawX, drawY])
            {
                if (!selected && parent.confirmed && parent.grayUnselected)
                {
                    for (int i = drawX; i < input.Length + drawX; i++)
                    {
                        Globals.SetForegroundColor(i, drawY, ConsoleColor.DarkGray);
                    }
                }
                else if (!parent.grayUnselected)
                {
                    for (int i = drawX; i < input.Length + drawX; i++)
                    {
                        Globals.SetForegroundColor(i, drawY, Globals.defaultForeground);
                    }
                }
                Globals.Write(drawX, drawY, input, out newX);
            }
        }
        catch
        {
            if (!selected && parent.confirmed && parent.grayUnselected)
            {
                for (int i = drawX; i < input.Length + drawX; i++)
                {
                    Globals.SetForegroundColor(i, drawY, ConsoleColor.DarkGray);
                }
            }
            else if (!parent.grayUnselected)
            {
                for (int i = drawX; i < input.Length + drawX; i++)
                {
                    Globals.SetForegroundColor(i, drawY, Globals.defaultForeground);
                }
            }
            Globals.Write(drawX, drawY, input, out newX);
        }
        newX += 1;
        drawEnd = newX;
    }
}
