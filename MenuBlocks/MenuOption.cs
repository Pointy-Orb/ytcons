namespace YTCons.MenuBlocks;

public class MenuOption
{
    public string? extraData = null;

    public bool selected = false;

    public string option;

    public Action onSelected;

    public Action? altOnSelected;

    public MenuBlock parent;

    public MenuOption(string option, MenuBlock parent, Action onSelected, Action? altOnSelected = null)
    {
        this.option = option;
        this.parent = parent;
        this.onSelected = onSelected;
        this.altOnSelected = altOnSelected;
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
                Globals.Write(drawX, drawY, input, out newX);
            }
        }
        catch
        {
            Globals.Write(drawX, drawY, input, out newX);
        }
        newX += 1;
    }
}
