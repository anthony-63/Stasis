using Raylib_cs;
using Stasis.Engine;
using Stasis.Game.Scenes.Loading;

namespace Stasis.Game;

public class Program
{
    static void Main()
    {
        Logger.Init("Stasis.log");
        var window = new Window(1280, 720, "Stasis", Raylib.LoadImage(Global.GetAsset("Assets/Game/Icon.ico", false)));
        window.SceneHandler.AddScene(new LoadingScene());
        window.Run();
        Global.Settings.Save("Assets/settings.toml");
    }
}