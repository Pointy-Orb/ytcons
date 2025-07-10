using Newtonsoft.Json;

namespace YTCons;

public class Settings
{
    public bool permasaveSubtitles { get; set; } = false;
    public bool systemLanguageAudio { get; set; } = false;
    public string chosenFormat { get; set; } = "mp4";
    public string chosenResolution { get; set; } = "Highest";

    public void SaveSettings()
    {
        var settingsJson = JsonConvert.SerializeObject(this);
        File.WriteAllText(Path.Combine(Dirs.configDir, "settings.json"), settingsJson);
    }
}
