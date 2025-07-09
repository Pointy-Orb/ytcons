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

        var mediaPlayerMenu = new MenuBlock(AnchorType.Cursor);
        var mediaPlayerOption = new MenuOption($"Change Media Player (current: {Globals.settings.mediaPlayer})", menu, () => Task.Run(() => Globals.activeScene.PushMenu(mediaPlayerMenu)));
        foreach (Settings.MediaPlayer mediaPlayer in typeof(Settings.MediaPlayer).GetEnumValues())
        {
            if (Dirs.TryGetPathApp(mediaPlayer.ToString().ToLower()) != null)
            {
                mediaPlayerMenu.options.Add(new MenuOption(mediaPlayer.ToString(), mediaPlayerMenu, () => Task.Run(() => ChangeMediaPlayer(mediaPlayer, mediaPlayerOption))));
            }
        }

        var formatMenu = new MenuBlock(AnchorType.Cursor);
    }

    private MenuBlock ResolutionMenu(string chosenFormat)
    {
        var menu = new MenuBlock();
        return menu;
    }

    private void ChangeMediaPlayer(Settings.MediaPlayer target, MenuOption display)
    {
        display.option = display.option.Replace(Globals.settings.mediaPlayer.ToString(), target.ToString());
        Globals.settings.mediaPlayer = target;
        Globals.settings.SaveSettings();
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
