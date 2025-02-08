using System.Security.Cryptography;
using Stasis.Content.Beatmaps;
using Stasis.Content.Replays;
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
        public Replay? Replay = null;
        public bool ReplayValid = false;
    }
    
    static LeaderboardEntry ReadEntry(string path) {
        var filePath = Directory.GetFiles(path)[0];
        var hashComputer = File.Open(filePath, FileMode.Open);
        byte[] hash = SHA256.Create().ComputeHash(hashComputer);
        var folderPath = Convert.ToHexString(hash);
        hashComputer.Close();
        var entry = new LeaderboardEntry();

        var replayDir = Directory.GetDirectories(path);
        if(replayDir.Length > 0) {
            entry.Replay = new Replay(Directory.GetFiles(replayDir[0])[0]);
            var replayHash = Util.GetSHA256(Global.SelectedMap?.Title + string.Concat(entry.Replay.Frames.Select(x => x.CursorPosition.X + x.CursorPosition.Y + x.Time) ?? []));
            entry.ReplayValid = entry.Replay.Hash.SequenceEqual(hash) && (Path.GetFileName(replayDir[0]) == replayHash);
        }

        if(Path.GetFileName(path) != folderPath) {
            Logger.Warn("INVALID SCORE HASH: ", Path.GetFileName(path), " != ", folderPath);
            entry.Valid = false;
            return entry;
        }

        var file = File.Open(filePath, FileMode.Open);
        entry.time = DateTime.FromFileTime(long.Parse(Path.GetFileName(filePath).Replace(".ss", "")));

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
        entry.Mods.Speed = BitConverter.ToSingle(buf);
        file.ReadExactly(buf);
        entry.timeStart = BitConverter.ToInt32(buf);
        file.ReadExactly(buf);
        entry.timeEnd = BitConverter.ToInt32(buf);

        return entry;
    }

    public static List<LeaderboardEntry> LoadLeaderboardFromMap(IBeatmapSet map) {
        var lb = new List<LeaderboardEntry>();
        var hash = Global.GetMapHash(map);
        var dirs = hash.Chunk(32).Select(x => new string(x)).ToList();
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