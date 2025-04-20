using System.Security.Cryptography;
using Stasis.Content.Beatmaps;
using Stasis.Engine;

namespace Stasis.Game.Scenes.Game.Player;

public static class EncouragementMessages {
    public static string[] Messages = {
        "Ehhh....",
        "Getting There!",
        "Doing Alright!",
        "Looking Better!",
        "Note Slayer!",
        "Note Destroyer!",
        "Note Annihilator!",
        "Note KING!",
    };
}

public class Score {
    public int Misses = 0;
    public int Hits = 0;

    public int Combo = 0;
    public int MaxCombo = 0;


    public int ScoreValue = 0;
    public int Multipier = 1;
    public int Miniplier = 0;

    public float Health = 100;
    public float HealthStep = 15;

    public bool Failed = false;
    public double AccPlaceholder = 0;

    public int TimeStart = 0;
    public int TimeEnd = 0;

    public double Accuracy => Hits + Misses > 0 ? Hits / (double)(Misses + Hits) * 100.0 : 100.0;
    
    public byte[] Hash = [];

    public string Serialize() {
        var mapHash = Global.GetMapHash(Global.SelectedMap ?? new BeatmapSet());
        var dirs = mapHash.Chunk(32).Select(x => new string(x)).ToList();
        dirs.Reverse();
        string last = dirs.Last();

        var parent = "Assets/Scores";
        foreach(var d in dirs) {
            var toCreate = parent + "/" + d;
            Directory.CreateDirectory(toCreate);
            parent = toCreate;
            if(d.Equals(last)) {
                var time = DateTime.Now.ToFileTime();
                var file = File.Create(toCreate + "/" + time.ToString() + ".ss");
                file.Write(BitConverter.GetBytes(Failed));
                file.Write(BitConverter.GetBytes(Misses));
                file.Write(BitConverter.GetBytes(Hits));
                file.Write(BitConverter.GetBytes(MaxCombo));
                file.Write(BitConverter.GetBytes(Accuracy));
                file.Write(BitConverter.GetBytes(ScoreValue));
                file.Write(BitConverter.GetBytes(Global.Mods.NoFail));
                file.Write(BitConverter.GetBytes(Global.Mods.Speed));
                file.Write(BitConverter.GetBytes(TimeStart));
                file.Write(BitConverter.GetBytes(TimeEnd));
                file.Flush();
                file.Close();

                var hashComputer = File.Open(toCreate + "/" + time.ToString() + ".ss", FileMode.Open);
                Hash = SHA256.Create().ComputeHash(hashComputer);
                var folderPath = toCreate + "/" + Convert.ToHexString(Hash);
                hashComputer.Close();
                Directory.CreateDirectory(folderPath);
                File.Move(file.Name, folderPath + "/" + Path.GetFileName(file.Name));
                return folderPath;
            }
        }
        return "";
    }
}