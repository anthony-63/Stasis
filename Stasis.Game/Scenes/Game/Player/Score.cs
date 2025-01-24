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

    public double Accuracy => Hits + Misses > 0 ? Hits / (double)(Misses + Hits) * 100.0 : 100.0;
}