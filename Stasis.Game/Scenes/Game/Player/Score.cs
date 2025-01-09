namespace Stasis.Game.Scenes.Game.Player;

public class Score {
    public int Misses = 0;
    public int Hits = 0;

    public double Accuracy => Hits + Misses > 0 ? Hits / (double)(Misses + Hits) * 100.0 : 100.0;
}