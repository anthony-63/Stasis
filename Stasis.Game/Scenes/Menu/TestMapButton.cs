using Raylib_cs;
using Stasis.Content.Beatmaps;
using Stasis.Engine;
using Stasis.Engine.UI;
using Stasis.Engine.UI.Elements;
using Stasis.Game.Scenes.Game;

namespace Stasis.Game.Scenes.Menu;

public class TestMapButton : Button {
    public Frame HoverFrame = new Frame() {
        Size = new UDim2(1, 0, 1, 0),
        Color = new Color(60, 60, 60, 150),
    };
    
    public required IBeatmapSet Map;

    public TestMapButton() {
        Children.Add(HoverFrame);
    }

    public override void Render() {
        HoverFrame.Visible = State == ButtonState.Hovering || State == ButtonState.Pressed;
        base.Render();
    }

    public void CheckPressed(Window window, MenuScene menu) {
        if(State == ButtonState.Pressed) {
            Global.SelectedMap = Map;
            Global.LoadedMenu = menu;
            window.SceneHandler.RemoveSceneByType<MenuScene>();
            window.SceneHandler.AddScene(new GameScene());
        }
    }
}