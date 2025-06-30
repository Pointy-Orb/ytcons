namespace YTCons.MenuBlocks;

public class MenuOption
{
    public string? extraData = null;

    public bool selected = false;

    public bool HasAltSelect
    {
        get { return _altOnSelected != null; }
    }

    public string option;

    private Func<Task> _onSelected;

    private Func<Task>? _altOnSelected;

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

    public void Draw(int i, int j, int prevMenuOffset, out int nextMenuOffset)
    {
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
        SafeWrite(option, out drawX);
        for (int l = prevMenuOffset; l < drawX; l++)
        {
            SafeWrite(" ", out _);
        }
        nextMenuOffset = drawX;
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
