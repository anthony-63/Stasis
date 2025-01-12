using Stasis.Content.Settings;
using Stasis.Engine;
using Stasis.Engine.Scene;
using Stasis.Game.Scenes.Game;
using Stasis.Game.Scenes.Menu;

namespace Stasis.Game.Scenes.Loading;

public class LoadingScene : IScene {
    public void Render(Window window) {}

    public void Update(Window window, double dt) {
        Global.Settings = Settings.Load("Assets/settings.json");

        Global.SelectedMap = null;
        MapLoader.LoadMaps("Assets/Maps");

        window.SceneHandler.RemoveSceneByType<LoadingScene>();
        window.SceneHandler.AddScene(new MenuScene());
    }
}