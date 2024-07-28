using System.Numerics;
using Raylib_cs;
using Rhythia.Engine;
using Rhythia.Engine.Audio;
using Rhythia.Engine.GFX;
using Rhythia.Engine.Scene;
using Rhythia.Game.Scenes.Game.HUD;
using Rhythia.Game.Scenes.Game.NoteObject;
using Rhythia.Game.Scenes.Game.Player;
using Rhythia.Game.Scenes.Menu;

namespace Rhythia.Game.Scenes.Game;

public class GameScene : IScene {
    public Player.Player Player = new();
    public Sprite Grid = Sprite.MakePlane(new Vector3(0, 0, 0), new Vector3(90, 0, 180), new Vector2(6, 6), "Assets/Game/Grid.png");

    public SyncAudioPlayer Music = new(Global.SelectedMap?.AudioData ?? null, 0.1f);

    public NoteObjectSpawner? Spawner = null;
    public NoteObjectRenderer? Renderer = null;

    HUDRoot HUD = new();

    public GameScene() {
        InputManager.HideCursor();
    }

    public void Update(Window window, double dt) {
        if(Global.SelectedMap == null || Raylib.IsKeyDown(KeyboardKey.R)) {
            GoToMenu(window);
            return;
        }

        Spawner ??= new NoteObjectSpawner(this, Player);
        Renderer ??= new NoteObjectRenderer(this);

        if(!Music.Playing) Music.Play(0f);
        else Music.Update();
        Player.Update();
        Spawner.Update(Player.Cursor);
        HUD.Update(Music.Time);
    }

    public void GoToMenu(Window window) {
        window.SceneHandler.RemoveSceneByType<GameScene>();
        window.SceneHandler.AddScene(new MenuScene());
        InputManager.ShowCursor();
    }

    public void Render(Window window) {
        HUD.Render();

        Player.StartRender();
        Renderer?.RenderNotesSingle();
        Grid.Render();
        Player.EndRender();
        Raylib.DrawFPS(0, 0);
    }
}