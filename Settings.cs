using Newtonsoft.Json;

namespace YTCons;

public class Settings
{
    public enum MediaPlayer
    {
        mpv,
        VLC
    }

    public bool permasaveSubtitles { get; set; } = false;
    public bool systemLanguageAudio { get; set; } = false;
    public MediaPlayer mediaPlayer { get; set; } = MediaPlayer.mpv;
    public string chosenFormat { get; set; } = "mp4";
    public string chosenResolution { get; set; } = "720p";

    public void SaveSettings()
    {
        var settingsJson = JsonConvert.SerializeObject(this);
        File.WriteAllText(Path.Combine(Dirs.configDir, "settings.json"), settingsJson);
    }
}
