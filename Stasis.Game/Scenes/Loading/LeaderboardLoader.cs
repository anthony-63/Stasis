using System.Security.Cryptography.X509Certificates;
using Microsoft.VisualBasic;
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
    }
    
    static LeaderboardEntry ReadEntry(string path) {
        var file = File.Open(path, FileMode.Open);
        
        var entry = new LeaderboardEntry();

        entry.time = DateTime.FromFileTimeUtc(long.Parse(Path.GetFileName(path)));

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

        var files = Directory.GetFiles(parent);
        foreach(var path in files) {
            try {
                lb.Add(ReadEntry(path));
            } catch {}
        }

        return lb;
    }
}