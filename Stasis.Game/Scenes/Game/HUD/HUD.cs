using Raylib_cs;
using Stasis.Engine.UI;
using Stasis.Engine.UI.Elements;
using Stasis.Game.Scenes.Game.Player;

namespace Stasis.Game.Scenes.Game.HUD;

public class HUDRoot: UiRoot {
    public Timer Timer = new();
    public Accuracy Accuracy = new();
    public Misses Misses = new();
    public FPS FPS = new();
    public Progress Progress = new();
    public ScoreValue ScoreValue = new();
    public Combo Combo = new();
    public SongArtist SongArtist = new();
    public Multiplier Multiplier = new();

    public HUDRoot() {
        Timer.EndTime = Global.SelectedMap?.Difficulties[0].Notes.Last().Time ?? 0f;
        AddChild(Accuracy);
        AddChild(Misses);
        AddChild(FPS);
        AddChild(Progress);
        AddChild(ScoreValue);
        AddChild(Combo);
        AddChild(SongArtist);
        AddChild(Multiplier);
    }

    public void Render() {
        Timer.Render();
        base.Render(Raylib.GetRenderWidth(), Raylib.GetRenderHeight());
    }

    public void Update(double dt, float currentTime, Score score) {
        Timer.Update(currentTime);
        Accuracy.Update(dt, score);
        Misses.Update(dt, score);
        Progress.Update(dt, currentTime);
        ScoreValue.Update(dt, score);
        Combo.Update(dt, score);
        Multiplier.Update(dt, score);
        base.Update(dt);
    }
}