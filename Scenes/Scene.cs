using YTCons.MenuBlocks;

namespace YTCons.Scenes;

public class Scene
{
    public MenuBlock RootMenu
    {
        get
        {
            return menus.Reverse().First();
        }
    }

    public Stack<MenuBlock> menus = new();
    internal bool[,] protectedTile = new bool[Console.WindowWidth, Console.WindowHeight];

    int prevMenuCount = 0;

    public void ChangeWindowSize()
    {
        PeekMenu().ChangeWindowSize();
    }

    public async Task Update()
    {
        LoadBar.WriteLoad();
        if (menus.TryPeek(out var result))
        {
            await result.Update();
        }
        if (menus.Count > prevMenuCount)
        {
            menus.Peek().Reset();
        }
        prevMenuCount = menus.Count;
        OnUpdate();
    }

    protected virtual void OnUpdate()
    {
    }

    public void Draw()
    {
        if (Globals.oldWindowWidth != Console.WindowWidth || Globals.oldWindowHeight != Console.WindowHeight)
        {
            protectedTile = new bool[Console.WindowWidth, Console.WindowHeight];
        }
        else
        {
            Array.Clear(protectedTile, 0, protectedTile.Length);
        }
        var prevMenuOffset = 0;
        int? prevCursor = null;
        foreach (var menu in menus.Reverse())
        {
            menu.Draw(prevMenuOffset, out prevMenuOffset, prevCursor, out prevCursor);
        }
        PostDraw();
    }

    public void PushMenu(MenuBlock menu)
    {
        menus.Push(menu);
        if(menu.options.Count > 0)
        {
            menu.options[menu.cursor].selected = true;
        }
        menus.Peek().Reset();
    }

    public MenuBlock PeekMenu()
    {
        return menus.Peek();
    }

    public async Task PopMenuAsync(bool forced = false)
    {
        if (menus.Count <= 1)
        {
            CloseRootMenu();
            if (!forced)
            {
                return;
            }
        }
        menus.Pop();
        if (menus.TryPeek(out var menu))
        {
            menu.Reset();
        }
    }

    protected virtual void CloseRootMenu()
    {

    }

    public void PopMenu(bool forced = false)
    {
        if (!forced && menus.Count() <= 1)
        {
            return;
        }
        if (PeekMenu().cursor >= PeekMenu().options.Count())
        {
            PeekMenu().cursor = 0;
        }
        if (PeekMenu().options.Count() > 0)
        {
            PeekMenu().options[PeekMenu().cursor].selected = false;
        }
        menus.Pop();
        if (menus.TryPeek(out var menu))
        {
            menu.Reset();
        }
    }

    protected virtual void PostDraw()
    {
    }

    internal async Task CheckKeys(ConsoleKeyInfo key)
    {
        var overrider = OnCheckKeys(key);
        if (!await overrider) return;
        if (menus.TryPeek(out var result))
        {
            await result.CheckKeys(key.Key);
        }
    }

    internal virtual async Task<bool> OnCheckKeys(ConsoleKeyInfo key)
    { return true; }
}
