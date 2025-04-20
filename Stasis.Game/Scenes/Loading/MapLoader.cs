using Stasis.Content.Beatmaps;
using Stasis.Engine;

namespace Stasis.Game.Scenes.Loading;

public static class MapLoader {
    public static List<IBeatmapSet> Maps = new();

    public static void LoadMap(string path) {
        if(path.EndsWith(".sspm")) {
            try {
                Maps.Add(new SSPMap(path));
            } catch(Exception e) {
                Logger.Warn("Failed to load ssp map: " + path + "\nReason: " + e.Message);
            }
        }
    }

    public static void LoadMaps(string search_dir) {
        var map_files = Directory.EnumerateFiles(search_dir).ToArray();
        var map_dirs = Directory.EnumerateDirectories(search_dir).ToArray();

        Parallel.ForEach(map_files, LoadMap);

        Parallel.ForEach(map_dirs, folder => {
            if(File.Exists(folder + "/meta.json")) {
                try {
                    Maps.Add(new BeatmapSet(folder));
                } catch(Exception e) {
                    Logger.Warn("Failed to load Stasis map: " + folder + "\nReason: " + e.Message);
                }
            }
        });
    }
}