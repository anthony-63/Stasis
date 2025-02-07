using System.Text.Json;

namespace Stasis.Content.Settings;

public class Settings {
    public NoteSettings Note { get; set; } = new();
    public CursorSettings Cursor { get; set; } = new();
    public AudioSettings Audio { get; set; } = new();
    public CameraSettings Camera { get; set; } = new();

    public void Save(string output) {
        string json = JsonSerializer.Serialize(this);
        if(File.Exists(output)) File.Delete(output);
        File.WriteAllText(output, json);
    }

    public static Settings Load(string input) {
        try {
            return JsonSerializer.Deserialize<Settings>(File.ReadAllText(input)) ?? new Settings();
        } catch {
            return new Settings();
        }
    }
}