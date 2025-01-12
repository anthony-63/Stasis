using System.Numerics;
using Raylib_cs;
using Stasis.Content.Beatmaps;
using Stasis.Engine;
using Stasis.Engine.UI;
using Stasis.Engine.UI.Elements;
using Stasis.Game.Scenes.Game;
using TinyTween;

namespace Stasis.Game.Scenes.Menu;

public class TestMapButton : Button {
    public Frame HoverFrame = new Frame() {
        Size = new UDim2(1, 0, 1, 0),
        Color = new Color(255, 255, 255, 150),
    };
    
    public required IBeatmapSet Map;
    public TestMapButton() {
        Children.Add(HoverFrame);
    }

    public override void Update(double dt) {
        HoverFrame.Visible = State == ButtonState.Hovering || State == ButtonState.Pressed;
        base.Update(dt);
    }

    public override void UpdateAbsoluteValues(Vector2 parentSize, Vector2 parentPosition) {
        base.UpdateAbsoluteValues(parentSize, parentPosition);
    }

    public override void Render() {
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