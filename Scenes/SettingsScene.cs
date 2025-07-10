using YTCons.MenuBlocks;

namespace YTCons.Scenes;

public class SettingsScene : Scene
{
    public SettingsScene()
    {
        var menu = new MenuBlock(AnchorType.Center);
        menu.options.Add(new MenuOption("Back", menu, () => Task.Run(() => Globals.scenes.Pop())));

        var subtitlesOption = new MenuOption("Save subtitles to config directory: " + EnOrDis(Globals.settings.permasaveSubtitles), menu, () => Task.Run(() => { }));
        subtitlesOption.tip = "Enabling will cause subtitles to be saved in the config directory instead of in the temporary directory.";
        subtitlesOption.ChangeOnSelected(() => Task.Run(() => { Globals.settings.permasaveSubtitles = !Globals.settings.permasaveSubtitles; ChangeBoolAftermath(Globals.settings.permasaveSubtitles, subtitlesOption); }));
        menu.options.Add(subtitlesOption);

        var audioLangOption = new MenuOption("Prioritize Default Audio over Native: " + EnOrDis(Globals.settings.systemLanguageAudio), menu, () => Task.Run(() => { }));
        audioLangOption.tip = "Some videos will have multiple audio tracks. By default, the native audio is prioritized over all of them. Enabling will instead prioritize the audio of your system language.";
        audioLangOption.ChangeOnSelected(() => Task.Run(() => { Globals.settings.systemLanguageAudio = !Globals.settings.systemLanguageAudio; ChangeBoolAftermath(Globals.settings.systemLanguageAudio, audioLangOption); }));
        menu.options.Add(audioLangOption);

        var formatMenu = new MenuBlock(AnchorType.Cursor);
        var formatOption = new MenuOption($"Change Default Format and Quality (current: {Globals.settings.chosenFormat}, {Globals.settings.chosenResolution})", menu, () => Task.Run(() => Globals.activeScene.PushMenu(formatMenu)));
        formatMenu.options.Add(new MenuOption("mp4", formatMenu, () => Task.Run(() => Globals.activeScene.PushMenu(ResolutionMenu("mp4", formatOption)))));
        formatMenu.options.Add(new MenuOption("webm", formatMenu, () => Task.Run(() => Globals.activeScene.PushMenu(ResolutionMenu("webm", formatOption)))));
        menu.options.Add(formatOption);
        PushMenu(menu);
    }

    private MenuBlock ResolutionMenu(string chosenFormat, MenuOption display)
    {
        var menu = new MenuBlock(AnchorType.Cursor);
        string[] resolutions = new string[] { "Highest", "144p", "240p", "360p", "480p", "720p", "1080p" };
        foreach (string resolution in resolutions)
        {
            menu.options.Add(new MenuOption(resolution, menu, () => Task.Run(() => ChosenResolution(chosenFormat, resolution, display))));
        }
        return menu;
    }

    private void ChosenResolution(string chosenFormat, string chosenResolution, MenuOption display)
    {
        display.option = display.option.Replace(Globals.settings.chosenFormat, chosenFormat);
        display.option = display.option.Replace(Globals.settings.chosenResolution, chosenResolution);
        Globals.settings.chosenFormat = chosenFormat;
        Globals.settings.chosenResolution = chosenResolution;
        Globals.settings.SaveSettings();
        Globals.activeScene.PopMenu();
        Globals.activeScene.PopMenu();
    }

    private void ChangeBoolAftermath(bool target, MenuOption display)
    {
        display.option = display.option.Replace(EnOrDis(!target), EnOrDis(target));
        display.parent.resetNextTick = true;
        Globals.settings.SaveSettings();
    }

    private string EnOrDis(bool condition)
    {
        return condition ? "Enabled" : "Disabled";
    }
}
