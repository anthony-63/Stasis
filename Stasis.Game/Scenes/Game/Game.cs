using System.Numerics;
using Raylib_cs;
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
    public Sprite Grid = Sprite.MakePlane(new Vector3(0, 0, 0), new Vector3(90, 0, 180), new Vector2(6, 6), "Assets/Game/Grid.png");

    public SyncAudioPlayer Music = new(Global.SelectedMap?.AudioData ?? null, Global.Settings.Audio.Volume);

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

        Global.Discord.SetPresence(new DiscordRPC.RichPresence() {
            Details = "Playing map '" + (Global.SelectedMap?.Title ?? "") + "'",
            Timestamps = DiscordRPC.Timestamps.FromTimeSpan(TimeSpan.FromSeconds(Global.SelectedMap?.Difficulties[0].Notes.Last().Time ?? 0)),
        });
    }

    public override void Update(double dt) {
        if(Global.SelectedMap == null || Raylib.IsKeyDown(KeyboardKey.R) || Player.Score.Failed) {
            Ending = true;
        }

        if(Ending) {
            if(Window is not null) UpdateEndSequence(Window, dt);
            return;
        }

        NoteManager ??= new NoteObjectManager(this, Player);
        NoteRenderer ??= new NoteObjectRenderer(this);

        if(!Music.Playing) Music.Play(0f);
        else Music.Update();
        Player.Update(ref Grid);
        NoteManager?.Update(Player.Cursor);
        HUD.Update(dt, Music.Time, Player.Score);
    }

    public static void GoToMenu(Window window) {
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
        // Raylib.DrawText("Allocated Instances: " + NoteRenderer?.MultiMesh.InstanceCount, 0, 24, 24, Color.White);
    }
}