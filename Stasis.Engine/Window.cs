using Stasis.Engine.Scene;

using Raylib_cs;

namespace Stasis.Engine;

public class Window {
    public SceneHandler SceneHandler = new SceneHandler();

    public Window(int width, int height, string title, Image icon) {
        Raylib.SetConfigFlags(ConfigFlags.Msaa4xHint);
        Raylib.InitAudioDevice();
        Logger.Info("Initialized Audio");
        Raylib.InitWindow(width, height, title);
        Raylib.SetWindowIcon(icon);
        Logger.Info("Initialized Window");
        InputManager.BindKey([KeyboardKey.LeftAlt, KeyboardKey.Enter], InputType.PressedOnce, Raylib.ToggleBorderlessWindowed);
    }

    public void Run() {
        while(!Raylib.WindowShouldClose()) {
            InputManager.Update();
            SceneHandler.UpdateAllScenes(this, Raylib.GetFrameTime());
            
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);

            SceneHandler.RenderAllScenes(this);

            Raylib.EndDrawing();
        }
    }
}