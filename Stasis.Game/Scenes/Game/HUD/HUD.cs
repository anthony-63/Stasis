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
    public Health Health = new();
    public DebugStats DebugStats = new();

    public HUDRoot() {
        Timer.MaxValue = Global.SelectedMap?.Difficulties[0].Notes.Last().Time ?? 0f;
        AddChild(Accuracy);
        AddChild(Timer);
        AddChild(Misses);
        AddChild(FPS);
        AddChild(Progress);
        AddChild(ScoreValue);
        AddChild(Combo);
        AddChild(SongArtist);
        AddChild(Multiplier);
        AddChild(Health);
        if(Global.EnableDebugStats) AddChild(DebugStats);
    }

    public void Render() {
        // Timer.Render();
        base.Render(Raylib.GetRenderWidth(), Raylib.GetRenderHeight());
    }

    public void Update(
        double dt, 
        float currentTime, 
        Score score, bool 
        skippable, 
        int allocatedInstances, 
        int startProcess,
        int visibleCount,
        float health,
        float healthStep,
        int replayFrames
    ) {
        Timer.Update(currentTime, skippable);
        Health.Update(score);
        Accuracy.Update(score);
        Misses.Update(score);
        Progress.Update(currentTime);
        ScoreValue.Update(score);
        Combo.Update(score);
        Multiplier.Update(score);
        if(Global.EnableDebugStats) DebugStats.Update(allocatedInstances, startProcess, currentTime, visibleCount, health, healthStep, replayFrames);
        base.Update(dt);
    }
}