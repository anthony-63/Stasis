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
    
    public void CloneDefaultAssets() {
        {
            Logger.Info("Downloading assets...");
            using var client = new HttpClient();
            using var s = client.GetStreamAsync("https://github.com/anthony-63/StasisDefaultAssets/archive/refs/heads/master.zip");
            using var fs = new FileStream("Assets.zip", FileMode.CreateNew);
            
            s.Result.CopyTo(fs);
            Logger.Info("Done.");
        }
        Logger.Info("Unzipping assets...");
        ZipFile.ExtractToDirectory("Assets.zip", "AssetsCloned");
        Logger.Info("Deleting archive...");
        File.Delete("Assets.zip");
        Logger.Info("Moving Game folder");
        Directory.Move("AssetsCloned/StasisDefaultAssets-master", "Assets");
        Logger.Info("Deleting old one...");
        Directory.Delete("AssetsCloned");
        Logger.Info("Your all set!");
    }

    public void CreateDirectories() {
        if(!Directory.Exists("Assets/")) CloneDefaultAssets();
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