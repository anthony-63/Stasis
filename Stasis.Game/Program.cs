using Stasis.Engine;
using Stasis.Game.Scenes.Loading;

namespace Stasis.Game;

public class Program {
    static void Main(string[] args) {
        Logger.Init("Stasis.log");
        var window = new Window(1280, 720, "Stasis");
        window.SceneHandler.AddScene(new LoadingScene());
        window.Run();
        Global.Settings.Save("Assets/settings.json");
    }
}