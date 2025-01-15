using Stasis.Content.Settings;
using Stasis.Engine;
using Stasis.Engine.Scene;
using Stasis.Game.Scenes.Game;
using Stasis.Game.Scenes.Menu;

namespace Stasis.Game.Scenes.Loading;

public class LoadingScene : Scene {
    public void Render(Window window) {}

    public override void Update(double dt) {
        Global.Settings = Settings.Load("Assets/settings.json");

        Global.SelectedMap = null;
        MapLoader.LoadMaps("Assets/Maps");

        Window?.SceneHandler.RemoveSceneByType<LoadingScene>();
        Window?.SceneHandler.AddScene(new MenuScene());
    }
}