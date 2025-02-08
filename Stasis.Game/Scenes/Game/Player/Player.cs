using System.Numerics;
using Raylib_cs;
using Stasis.Content.Replays;
using Stasis.Engine;
using Stasis.Engine.Audio;
using Stasis.Engine.GFX;
using Stasis.Game.Scenes.Game.HUD;
using Stasis.Game.Scenes.Game.NoteObject;

namespace Stasis.Game.Scenes.Game.Player;

public class Player {
    public Score Score = new();
    public Camera Camera = new(new Vector3(0, 0, 7), Global.Settings.Camera.FOV);
    public Cursor Cursor = new(Vector3.Zero, new Vector3(90, 0, 180), Vector2.One * Global.Settings.Cursor.Scale, Global.GetAsset("Assets/Game/Cursor.png"));
    public ReplayManager ReplayManager;

    public AudioFX HitFX = new(Global.GetAsset("Assets/Game/hit.mp3"), Global.Settings.Audio.FXVolume);

    public Player() {
        if(Global.Replay is not null) ReplayManager = new ReplayManager(Global.Replay);
        else ReplayManager = new ReplayManager();
    }

    public void StartRender() {
        Camera.Start();
        Cursor.Render();
    }

    public void Hit(int _) {
        Score.Hits++;
        Score.ScoreValue += 25 * Score.Multipier;
        Score.Miniplier = Math.Min(8, Score.Miniplier + 1);
        if(Score.Miniplier >= 8 && Score.Multipier < 8) {
            Score.Miniplier = 0;
            Score.Multipier = Math.Min(8, Score.Multipier + 1);
        }

        Score.Combo++;
        Score.MaxCombo = Math.Max(Score.MaxCombo, Score.Combo);

        Score.HealthStep = Math.Max(Score.HealthStep / 1.45f, 15f);
        Score.Health = Math.Min(Score.Health + Score.HealthStep / 1.75f, 100f);
        HitFX.Play();
    }

    public void Miss(int _) {
        Score.Misses++;

        Score.Miniplier = 0;
        Score.Multipier = Math.Max(1, Score.Multipier - 1);

        Score.Combo = 0;

        Score.HealthStep += 1.2f;
        Score.Health = Math.Max(Score.Health - Score.HealthStep, 0);
        if(Score.Health <= 0 && !Global.Mods.NoFail) Score.Failed = true;
    }

    public void Update(double dt, Sprite grid, SyncAudioPlayer? music) {
        if(Global.Replay is null) {
            if(Global.Settings.Misc.EnableReplays) ReplayManager.UpdateFrameMaker(dt, Cursor, music, Score);
            Cursor.ProcessInput();
        }
        else ReplayManager.PlayFrame(Cursor, music, Score);

        Cursor.ApplyParallax(Camera, grid);
    }

    public void EndRender() {
        Camera.End();
    }
}