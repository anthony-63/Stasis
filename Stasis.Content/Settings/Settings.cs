using System.Text.Json;
using Tomlyn;

namespace Stasis.Content.Settings;

public class Settings {
    public NoteSettings Note { get; set; } = new();
    public CursorSettings Cursor { get; set; } = new();
    public AudioSettings Audio { get; set; } = new();
    public CameraSettings Camera { get; set; } = new();

    public MiscSettings Misc { get; set; } = new();
    
    public void Save(string output) {
        var text = Toml.FromModel(this);
        File.WriteAllText(output, text);
    }

    public static Settings Load(string input) {
        try {
            return Toml.Parse(File.ReadAllText(input)).ToModel<Settings>() ?? new Settings();
        } catch {
            return new Settings();
        }
    }
}