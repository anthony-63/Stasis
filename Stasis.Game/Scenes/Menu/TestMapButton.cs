using Stasis.Content.Beatmaps;
using Stasis.Engine;
using Stasis.Engine.UI.Elements;
using Stasis.Game.Scenes.Game;

namespace Stasis.Game.Scenes.Menu;

public class TestMapButton : Button {
    public required IBeatmapSet Map;
    public void CheckPressed(Window window) {
        if(State == ButtonState.Pressed) {
            Global.SelectedMap = Map;
            window.SceneHandler.RemoveSceneByType<MenuScene>();
            window.SceneHandler.AddScene(new GameScene());
        }
    }
}