using System.Diagnostics;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;
using Stasis.Content.Settings;
using Stasis.Engine;
using Stasis.Engine.Scene;
using Stasis.Game.Scenes.Game;
using Stasis.Game.Scenes.Menu;

namespace Stasis.Game.Scenes.Loading;

public class LoadingScene : Scene {
    public void Render(Window window) {}

    public void CreateDirectories() {
        Directory.CreateDirectory("Assets/Scores");
        Directory.CreateDirectory("Assets/Maps");
        if(!File.Exists("Assets/settings.toml")) Global.Settings.Save("Assets/settings.toml");
        else Global.Settings = Settings.Load("Assets/settings.toml");
    }

    public override void Update(double dt) {
        CreateDirectories();
        Global.Settings = Settings.Load("Assets/settings.toml");

        Global.SelectedMap = null;
        MapLoader.LoadMaps("Assets/Maps");

        Window?.SceneHandler.RemoveSceneByType<LoadingScene>();
        Window?.SceneHandler.AddScene(new MenuScene());
    }
}