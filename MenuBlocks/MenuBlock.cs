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
    public bool resetNextTick = false;

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
                if (anchorType == AnchorType.Cursor && cursorAnchorPos != null)
                {
                    pos += (int)cursorAnchorPos;
                }
                else
                {
                    pos -= (Console.WindowHeight - options.Count - 1);
                }
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
        if (resetNextTick)
        {
            resetNextTick = false;
            Reset();
        }
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

    private int? cursorAnchorPos = null;

    public void Draw(int prevMenuOffset, out int nextMenuOffset, int? prevDrawCursor, out int? drawCursor)
    {
        var winHeight = Console.WindowHeight;
        nextMenuOffset = prevMenuOffset;
        drawCursor = null;
        if (!draw || !PreDraw()) return;
        int i = prevMenuOffset;
        bool overrideAnchor = options.Count() > winHeight;
        for (int j = Console.WindowTop; j < winHeight; j++)
        {
            var pos = j - drawOffset;
            if (anchorType == AnchorType.Center || ((anchorType == AnchorType.Cursor || overrideAnchor) && prevDrawCursor == null))
            {
                pos -= winHeight / 2;
                if (!overrideAnchor)
                    pos += options.Count / 2;
            }
            if (anchorType == AnchorType.Cursor || overrideAnchor)
            {
                pos += cursor;
                if (prevDrawCursor != null)
                {
                    pos -= (int)prevDrawCursor;
                    cursorAnchorPos = cursor - (int)prevDrawCursor;
                }
            }
            if (anchorType == AnchorType.Bottom && !overrideAnchor)
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
        foreach (MenuOption option in options)
        {
            option.PostDrawEverything();
        }
    }

    protected virtual void PostDraw()
    {
    }

    public async Task CheckKeys(ConsoleKey key)
    {
        await OnCheckKeys(key);
        if (!active) return;
        if (!confirmed)
        {
            if (key == ConsoleKey.K || key == ConsoleKey.UpArrow || key == ConsoleKey.W)
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
            else if (key == ConsoleKey.J || key == ConsoleKey.DownArrow || key == ConsoleKey.S)
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
                if (options.Count > oldCursor && options.Count > 0)
                {
                    options[oldCursor].selected = false;
                }
                if (options.Count <= cursor)
                {
                    cursor = 0;
                }
                if (options.Count > 0)
                {
                    options[cursor].selected = true;
                }
            }
            if ((key == ConsoleKey.H || key == ConsoleKey.LeftArrow || key == ConsoleKey.A))
            {
                Globals.activeScene.PopMenu();
            }
        }
        if (key == ConsoleKey.Spacebar || key == ConsoleKey.L || key == ConsoleKey.RightArrow || key == ConsoleKey.D)
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

    public static Func<Task> ConfirmAction(Func<Task> action)
    {
        return () => Task.Run(() =>
        {
            var newAction = action + (() => Task.Run(() => Globals.activeScene.PopMenu()));
            var block = new MenuBlock(AnchorType.Cursor);
            block.options.Add(new MenuOption("Abort", block, () => Task.Run(() => Globals.activeScene.PopMenu())));
            block.options.Add(new MenuOption("Confirm", block, action));
            Globals.activeScene.PushMenu(block);
        });
    }
}
