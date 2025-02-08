using System.Numerics;
using Raylib_cs;
using Stasis.Content.Beatmaps;
using Stasis.Engine;
using Stasis.Engine.Audio;
using Stasis.Engine.GFX;
using Stasis.Engine.Scene;
using Stasis.Engine.UI;
using Stasis.Engine.UI.Elements;
using Stasis.Game.Scenes.Game.HUD;
using Stasis.Game.Scenes.Game.NoteObject;
using Stasis.Game.Scenes.MapInfo;

namespace Stasis.Game.Scenes.Game;

public class GameScene : Scene {
    public Player.Player Player = new();
    public Sprite Grid = Sprite.MakePlane(new Vector3(0, 0, 0), new Vector3(90, 0, 180), new Vector2(6, 6), Global.GetAsset("Assets/Game/Grid.png"));

    public SyncAudioPlayer Music = new(Global.SelectedMap?.AudioData ?? null, Global.Settings.Audio.Volume, Global.Mods.Speed);

    public NoteObjectManager? NoteManager = null;
    public NoteObjectRenderer? NoteRenderer = null;

    public bool Ending = false;
    public float EndTimer = 0f;
    public Frame FadeFrame = new() {
        Size = UDim2.Fill,
        Color = Raylib.ColorAlpha(Color.Black, 0f),
    };

    HUDRoot HUD = new();

    public GameScene() {
        HUD.AddChild(FadeFrame);
        InputManager.HideCursor();

        var modText = Global.GetModText(Global.Mods);
        Global.Discord.SetPresence(new DiscordRPC.RichPresence() {
            Details = "Playing Map" + (modText == "" ? "" : " with " + modText),
            State = Global.SelectedMap?.Title[..Math.Min(Global.SelectedMap?.Title.Length ?? 0, 127)] ?? "",
            Timestamps = DiscordRPC.Timestamps.FromTimeSpan(TimeSpan.FromSeconds(Global.SelectedMap?.Difficulties[0].Notes.Last().Time ?? 0)),
        });
    }

    public override void Update(double dt) {
        var quitEarly = Raylib.IsKeyDown(KeyboardKey.R);
        if(Global.SelectedMap == null || quitEarly || Player.Score.Failed || Music?.Time > Global.SelectedMap?.Difficulties[0].Notes.Last().Time + 1) {
            Ending = true;
            Player.Score.Failed |= quitEarly;
            Player.Score.TimeEnd = Math.Max((int)(Music?.Time ?? 0), 0);
        }

        if(Raylib.IsKeyDown(KeyboardKey.Space) && (NoteManager?.Skippable ?? false)) {
            if(Music is not null) NoteManager.SkipToNote(Music);
        }

        if(Ending) {
            if(Window is not null) UpdateEndSequence(Window, dt);
            return;
        }

        NoteManager ??= new NoteObjectManager(this, Player);
        NoteRenderer ??= new NoteObjectRenderer(this);

        if(!Music?.Playing ?? false ) Music?.Play(-2f);
        else Music?.Update();
        Player.Update(dt, Grid, Music);
        NoteManager?.Update(Player.Cursor);
        HUD.Update(
            dt, 
            Math.Max(0f, Music?.Time ?? 0f), 
            Player.Score, 
            NoteManager?.Skippable ?? false, 
            NoteRenderer.MultiMesh.InstanceCount, 
            NoteManager?.StartProcess ?? 0,
            NoteRenderer.ToRender.Count,
            Player.Score.Health,
            Player.Score.HealthStep,
            Player.ReplayManager.Replay.Frames.Count
        );
    }

    public void GoToMenu(Window window) {
        if(Global.Replay is null) {
            string dir = Player.Score.Serialize();
            if(Global.Settings.Misc.EnableReplays) Player.ReplayManager.Save(dir, Music, Player.Score);
        }
        Global.Replay = null;

        window.SceneHandler.RemoveSceneByType<GameScene>();
        window.SceneHandler.AddScene(new MapInfoScene()); 
        InputManager.ShowCursor();
    }

    public void UpdateEndSequence(Window window, double dt) {
        FadeFrame.Color = Raylib.ColorAlpha(FadeFrame.Color, EndTimer);
        EndTimer += (float)dt;
        if(EndTimer >= 1f) GoToMenu(window);
    }

    public override void Render() {
        Player.StartRender();
        NoteRenderer?.RenderNotes(Player.Cursor);
        Grid.Render();
        Player.EndRender();
        HUD.Render();
    }
}