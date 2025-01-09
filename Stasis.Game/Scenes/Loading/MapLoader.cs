using Stasis.Content.Beatmaps;
using Stasis.Engine;

namespace Stasis.Game.Scenes.Loading;

public static class MapLoader {
    public static List<IBeatmapSet> Maps = new();

    public static void LoadMaps(string search_dir) {
        var map_files = Directory.EnumerateFiles(search_dir).ToArray();
        var map_dirs = Directory.EnumerateDirectories(search_dir).ToArray();

        foreach(string file in map_files) {
            if(file.EndsWith(".sspm")) {
                try {
                    Maps.Add(new SSPMap(file));
                } catch(Exception e) {
                    Logger.Warn("Failed to load ssp map: " + file + "\nReason: " + e.Message);
                }
            }
        }

        foreach(string folder in map_dirs) {
            if(File.Exists(folder + "/meta.json")) {
                try {
                    Maps.Add(new BeatmapSet(folder));
                } catch(Exception e) {
                    Logger.Warn("Failed to load Stasis map: " + folder + "\nReason: " + e.Message);
                }
            }
        }
    }
}