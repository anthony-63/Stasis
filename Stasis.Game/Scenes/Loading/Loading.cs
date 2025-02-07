using System.Diagnostics;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;
using DiscordRPC;
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
        if(!File.Exists("Assets/settings.json")) Global.Settings.Save("Assets/settings.json");
    }

    public override void Update(double dt) {
        CreateDirectories();
        Global.Settings = Settings.Load("Assets/settings.json");

        Global.SelectedMap = null;
        MapLoader.LoadMaps("Assets/Maps");

        Window?.SceneHandler.RemoveSceneByType<LoadingScene>();
        Window?.SceneHandler.AddScene(new MenuScene());
    }
}