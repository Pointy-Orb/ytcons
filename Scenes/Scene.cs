using YTCons.MenuBlocks;

namespace YTCons.Scenes;

public class Scene
{
    protected Stack<MenuBlock> menus = new();
    internal bool[,] protectedTile = new bool[Console.WindowWidth, Console.WindowHeight];

    int prevMenuCount = 0;

    public void Update()
    {
        OnUpdate();
        if (menus.TryPeek(out var result))
        {
            result.Update();
        }
        if (menus.Count > prevMenuCount)
        {
            menus.Peek().Reset();
        }
        prevMenuCount = menus.Count;
    }

    public virtual void OnUpdate()
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
            if (prevCursor == null)
            {
                menu.Draw(prevMenuOffset, out prevMenuOffset, 0, out prevCursor);
            }
            else
            {
                menu.Draw(prevMenuOffset, out prevMenuOffset, 0, out _);
            }
        }
        PostDraw();
    }

    public void PushMenu(MenuBlock menu)
    {
        menus.Push(menu);
        menus.Peek().Reset();
    }

    public MenuBlock PeekMenu()
    {
        return menus.Peek();
    }

    public void PopMenu(bool forced = false)
    {
        if (!forced && menus.Count <= 1)
        {
            return;
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

    internal void CheckKeys(ConsoleKeyInfo key)
    {
        if (!OnCheckKeys(key)) return;
        if (menus.TryPeek(out var result))
        {
            result.CheckKeys(key.Key);
        }
    }

    internal virtual bool OnCheckKeys(ConsoleKeyInfo key)
    { return true; }
}
