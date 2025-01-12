using System.Numerics;
using Raylib_cs;
using Stasis.Engine;
using Stasis.Engine.Audio;
using Stasis.Engine.GFX;
using Stasis.Engine.Scene;
using Stasis.Game.Scenes.Game.HUD;
using Stasis.Game.Scenes.Game.NoteObject;
using Stasis.Game.Scenes.Game.Player;
using Stasis.Game.Scenes.Menu;

namespace Stasis.Game.Scenes.Game;

public class GameScene : IScene {
    public Player.Player Player = new();
    public Sprite Grid = Sprite.MakePlane(new Vector3(0, 0, 0), new Vector3(90, 0, 180), new Vector2(6, 6), "Assets/Game/Grid.png");

    public SyncAudioPlayer Music = new(Global.SelectedMap?.AudioData ?? null, Global.Settings.Audio.Volume);

    public NoteObjectManager? NoteManager = null;
    public NoteObjectRenderer? NoteRenderer = null;

    HUDRoot HUD = new();

    public GameScene() {
        InputManager.HideCursor();
    }

    public void Update(Window window, double dt) {
        if(Global.SelectedMap == null || Raylib.IsKeyDown(KeyboardKey.R)) {
            GoToMenu(window);
            return;
        }

        NoteManager ??= new NoteObjectManager(this, Player);
        NoteRenderer ??= new NoteObjectRenderer(this);

        if(!Music.Playing) Music.Play(0f);
        else Music.Update();
        Player.Update(ref Grid);
        NoteManager?.Update(Player.Cursor);
        HUD.Update(Music.Time, Player.Score);
    }

    public void GoToMenu(Window window) {
        window.SceneHandler.RemoveSceneByType<GameScene>();
        window.SceneHandler.AddScene(Global.LoadedMenu ?? new MenuScene());
        InputManager.ShowCursor();
    }

    public void Render(Window window) {
        HUD.Render();

        Player.StartRender();
        NoteRenderer?.RenderNotes(Player.Cursor);
        Grid.Render();
        Player.EndRender();
        Raylib.DrawText("Allocated Instances: " + NoteRenderer?.MultiMesh.InstanceCount, 0, 24, 24, Color.White);
        Raylib.DrawFPS(0, 0);
    }
}