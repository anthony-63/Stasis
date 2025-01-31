using System.Security.Cryptography;
using Stasis.Content.Beatmaps;
using Stasis.Engine;
using Stasis.Game.Scenes.Game.Player;

namespace Stasis.Game.Scenes.Loading;

public static class LeaderboardLoader {
    public class LeaderboardEntry {
        public Score Score = new();
        public Mods Mods = new();
        public DateTime time;

        public int timeStart;
        public int timeEnd;
        public bool Valid = true;
    }
    
    static LeaderboardEntry ReadEntry(string path) {
        var filePath = Directory.GetFiles(path)[0];
        var hashComputer = File.Open(filePath, FileMode.Open);
        var folderPath = Convert.ToHexString(SHA256.Create().ComputeHash(hashComputer));
        hashComputer.Close();
        var entry = new LeaderboardEntry();

        if(Path.GetFileName(path) != folderPath) {
            Logger.Warn("INVALID SCORE HASH: ", Path.GetFileName(path), " != ", folderPath);
            entry.Valid = false;
            return entry;
        }


        var file = File.Open(filePath, FileMode.Open);
        entry.time = DateTime.FromFileTime(long.Parse(Path.GetFileName(filePath)));

        var buf = new byte[4];
        var buf8 = new byte[8];

        entry.Score.Failed = file.ReadByte() == 1;
        file.ReadExactly(buf);
        entry.Score.Misses = BitConverter.ToInt32(buf);
        file.ReadExactly(buf);
        entry.Score.Hits = BitConverter.ToInt32(buf);
        file.ReadExactly(buf);
        entry.Score.MaxCombo = BitConverter.ToInt32(buf);
        file.ReadExactly(buf8);
        entry.Score.AccPlaceholder = BitConverter.ToDouble(buf8);
        file.ReadExactly(buf);
        entry.Score.ScoreValue = BitConverter.ToInt32(buf);
        entry.Mods.NoFail = file.ReadByte() == 1;
        file.ReadExactly(buf);
        entry.Mods.Speed = BitConverter.ToInt32(buf);
        file.ReadExactly(buf);
        entry.timeStart = BitConverter.ToInt32(buf);
        file.ReadExactly(buf);
        entry.timeEnd = BitConverter.ToInt32(buf);

        return entry;
    }

    public static List<LeaderboardEntry> LoadLeaderboardFromMap(IBeatmapSet map) {
        var lb = new List<LeaderboardEntry>();
        var hash = Global.GetMapHash(map);
        var dirs = hash.Chunk(8).Select(x => new string(x)).ToList();
        dirs.Reverse();

        var scoreExists = false;
        var parent = "Assets/Scores";
        foreach(var dir in dirs) {
            parent += "/" + dir;
            scoreExists = Directory.Exists(parent);
        }
        if(!scoreExists) return lb;

        var files = Directory.GetDirectories(parent);
        foreach(var path in files) {
            try {
                lb.Add(ReadEntry(path));
            } catch(Exception e) {
                Logger.Warn("Error loading score: ", e);
            }
        }

        return lb;
    }
}