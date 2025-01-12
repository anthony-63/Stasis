using System.Text.Json;
using Stasis.Engine;

namespace Stasis.Content.Beatmaps;

public class BeatmapSet : IBeatmapSet {
    public ushort Version { get; set; }
    public string Title { get; set; } = "";
    public string Artist { get; set; } = "";
    public string[] Mappers { get; set; } = Array.Empty<string>();
    public Beatmap[] Difficulties { get; set; } = Array.Empty<Beatmap>();

    public byte[] AudioData { get; set; }
    
    public string Path { get; set; } = "";
    public byte[] Cover { get; set; } = [];

    public BeatmapSet(string folderPath) {
        Logger.Info("Loading map: ", folderPath);

        Path = folderPath;

        var metaDoc = JsonDocument.Parse(File.ReadAllText(folderPath + "/meta.json"));

        Version = metaDoc.RootElement.GetProperty("_version").GetUInt16();
        Title = metaDoc.RootElement.GetProperty("_title").GetString() ?? throw new JsonException("Failed to parse _title");
        Mappers = metaDoc.RootElement.GetProperty("_mappers").EnumerateArray().Select(elem => elem.GetString() ?? throw new JsonException("Failed to parse _mappers")).ToArray();

        AudioData = File.ReadAllBytes(folderPath + "/" + metaDoc.RootElement.GetProperty("_music").GetString() ?? throw new JsonException("Failed to parse _music"));

        Difficulties = GetDifficulties(metaDoc.RootElement.GetProperty("_difficulties").EnumerateArray().Select(elem => elem.GetString() ?? throw new JsonException("Failed to parse _difficulties")).ToArray());
    }

    Beatmap[] GetDifficulties(string[] files) {
        var beatmaps = new Beatmap[files.Length];

        for(int i = 0; i < files.Length; i++) {
            beatmaps[i] = new Beatmap();

            var diffDoc = JsonDocument.Parse(File.ReadAllText(Path + "/" + files[i]));
            beatmaps[i].Name = diffDoc.RootElement.GetProperty("_name").GetString() ?? throw new JsonException("Failed to parse _name in difficulty " + files[i]);
            
            var noteElements = diffDoc.RootElement.GetProperty("_notes").EnumerateArray().ToArray();
            beatmaps[i].Notes = new Note[noteElements.Length];

            int j = 0;
            foreach(JsonElement noteElem in noteElements) {
                beatmaps[i].Notes[j].Time = (float)noteElem.GetProperty("_time").GetDouble();
                beatmaps[i].Notes[j].X = noteElem.GetProperty("_x").GetSingle();
                beatmaps[i].Notes[j].Y = noteElem.GetProperty("_y").GetSingle();
                j++;
            }
            beatmaps[i].Notes = beatmaps[i].Notes.OrderBy(n => n.Time).ToArray();
        }
        return beatmaps;
    }
}