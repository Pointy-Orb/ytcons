namespace YTCons.MenuBlocks;

public class MenuBlock
{
    public int cursor = 0;
    public int oldCursor = 0;
    public int drawOffset = 0;
    public AnchorType anchorType;
    public bool active { get; protected set; }
    private bool draw = false;
    public bool grayUnselected { get; set; }

    private int prevWindowHeight = 0;

    public bool confirmed = false;
    private bool realAltConfirmed = false;
    public bool altConfirmed
    {
        get
        {
            return confirmed && realAltConfirmed;
        }
        set
        {
            realAltConfirmed = value;
        }
    }
    public MenuOption? selectedOption = null;

    public List<MenuOption> options = new List<MenuOption>();

    public (int x, int y) selectedDrawPos
    {
        get
        {
            for (int j = 0; j < Console.WindowHeight; j++)
            {
                var pos = j - drawOffset;
                pos -= (Console.WindowHeight - options.Count - 1);
                if (pos >= 0 && pos < options.Count && options[pos].selected)
                {
                    return (options[pos].drawEnd, j);
                }
            }
            return (0, 0);
        }
    }

    public void ChangeWindowSize()
    {
        OnChangeWindowSize();
    }

    protected virtual void OnChangeWindowSize()
    {
    }

    public void Reset()
    {
        active = true;
        draw = true;
        confirmed = false;
        altConfirmed = false;
        selectedOption = null;
    }

    public MenuBlock(AnchorType anchorType = AnchorType.Center)
    {
        this.anchorType = anchorType;
        prevWindowHeight = Console.WindowHeight;
    }

    public async Task Update()
    {
        OnUpdate();
        if (!active)
        {
            return;
        }
        prevWindowHeight = Console.WindowHeight;
        if (confirmed)
        {
            selectedOption = options[cursor];
            if (altConfirmed && selectedOption.HasAltSelect)
            {
                await selectedOption.AltOnSelected();
            }
            else
            {
                await selectedOption.OnSelected();
            }
            active = false;
        }
        oldCursor = cursor;
        foreach (MenuOption option in options)
        {
            option.Update();
        }
    }

    protected virtual void OnUpdate()
    {
    }

    protected virtual bool PreDraw()
    {
        return true;
    }



    public void Draw(int prevMenuOffset, out int nextMenuOffset, int? prevDrawCursor, out int? drawCursor)
    {
        var winHeight = Console.WindowHeight;
        nextMenuOffset = prevMenuOffset;
        drawCursor = null;
        if (!draw || !PreDraw()) return;
        int i = prevMenuOffset;
        for (int j = Console.WindowTop; j < winHeight - 1; j++)
        {
            var pos = j - drawOffset;
            if (anchorType == AnchorType.Center || (anchorType == AnchorType.Cursor && prevDrawCursor == null))
            {
                pos -= winHeight / 2;
                pos += options.Count / 2;
            }
            if (anchorType == AnchorType.Cursor && prevDrawCursor != null)
            {
                pos += cursor;
                pos -= (int)prevDrawCursor;
            }
            if (anchorType == AnchorType.Bottom)
            {
                pos -= (winHeight - options.Count - 1);
            }
            if (pos < options.Count && pos >= 0)
            {
                options[pos].Draw(i, j, prevMenuOffset, out var testChildCursorOffset);
                if (nextMenuOffset < testChildCursorOffset && (!grayUnselected || !confirmed))
                {
                    nextMenuOffset = testChildCursorOffset;
                }
                if (grayUnselected && options[pos].selected && confirmed)
                {
                    nextMenuOffset = testChildCursorOffset;
                }
                if (options[pos].selected)
                {
                    drawCursor = j;
                }
            }
            /*
            else
            {
                for (int l = prevMenuOffset; l < nextMenuOffset; l++)
                {
                    if (j >= Globals.activeScene.protectedTile.GetLength(1) || l >= Globals.activeScene.protectedTile.GetLength(0)) continue;
                    if (!Globals.activeScene.protectedTile[l, j])
                    {
                        Globals.ClearTile(l, j);
                    }
                }
            }
			*/
        }
        if (Globals.activeScene.protectedTile.GetLength(0) != Console.WindowWidth || Globals.activeScene.protectedTile.GetLength(1) != winHeight)
        {
            Globals.activeScene.protectedTile = new bool[Console.WindowWidth, winHeight];
        }
        else
        {
            Array.Clear(Globals.activeScene.protectedTile, 0, Globals.activeScene.protectedTile.Length);
        }
        PostDraw();
    }

    protected virtual void PostDraw()
    {
    }

    public async Task CheckKeys(ConsoleKey key)
    {
        OnCheckKeys(key);
        if (!active) return;
        if (!confirmed)
        {
            if (key == ConsoleKey.K || key == ConsoleKey.UpArrow)
            {
                if (cursor > 0)
                {
                    cursor--;
                }
                else
                {
                    cursor = options.Count - 1;
                }
            }
            else if (key == ConsoleKey.J || key == ConsoleKey.DownArrow)
            {
                if (cursor < options.Count - 1)
                {
                    cursor++;
                }
                else
                {
                    cursor = 0;
                }
            }
            if (oldCursor != cursor)
            {
                options[oldCursor].selected = false;
                options[cursor].selected = true;
            }
            if ((key == ConsoleKey.H || key == ConsoleKey.LeftArrow))
            {
                Globals.activeScene.PopMenu();
            }
        }
        if (key == ConsoleKey.Spacebar || key == ConsoleKey.L || key == ConsoleKey.RightArrow)
        {
            confirmed = true;
        }
        if (key == ConsoleKey.Enter)
        {
            altConfirmed = true;
            confirmed = true;
        }
    }

    protected virtual async Task OnCheckKeys(ConsoleKey key)
    {
    }
}
