using System.Numerics;
using Raylib_cs;
using Stasis.Engine.UI;
using Stasis.Engine.UI.Elements;
using Stasis.Game.Scenes.Game.Player;

namespace Stasis.Game.Scenes.Game.HUD;

public class Combo : Label {
    public Combo() {
        OneLine = true;

        Size = new UDim2(0, 0, 1, 0);
        FontSize = 48;
        Font = Global.UIFont;

        AlignmentX = TextAlignX.Left;
        AlignmentY = TextAlignY.Top;
        Text = "0x";

        TextColor = Color.White;

        Position = new UDim2(0.005f, 0, 0, 0);
    }

    public override void Render() {
        base.Render();
    }

    public void UpdatePos() {
        var pos = Position;
        pos.Y.Offset = Raylib.GetRenderHeight() - 100;
        Position = pos;
    }

    public void Update(double dt, Score score) {
        Text = score.Combo.ToString() + "x";
        UpdatePos();
        base.Update(dt);
    }
}