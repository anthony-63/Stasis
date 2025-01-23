using System.Numerics;
using Raylib_cs;
using Stasis.Engine.UI;
using Stasis.Engine.UI.Elements;
using Stasis.Game.Scenes.Game.Player;

namespace Stasis.Game.Scenes.Game.HUD;

public class FPS : Label {
    public FPS() {
        OneLine = true;

        Size = new UDim2(0, 0, 1, 0);
        FontSize = 48;
        Font = "Assets/Game/font.ttf";

        AlignmentX = TextAlignX.Right;
        AlignmentY = TextAlignY.Top;
        Text = "0 FPS";

        TextColor = Color.Lime;

        Position = new UDim2(0.995f, 0, 0, 0);
    }

    public override void Render() {
        base.Render();
    }

    public void UpdatePos() {
        var pos = Position;
        pos.Y.Offset = Raylib.GetRenderHeight() - 60;
        Position = pos;
    }

    public override void Update(double dt) {
        Text = Raylib.GetFPS().ToString() + " FPS";
        UpdatePos();
        base.Update(dt);
    }
}