using System.Numerics;
using Raylib_cs;
using Stasis.Engine.UI;
using Stasis.Engine.UI.Elements;
using Stasis.Game.Scenes.Game.Player;

namespace Stasis.Game.Scenes.Game.HUD;

public class FPS : Label {
    int MaxFPS = 0;

    public FPS() {
        OneLine = true;

        Size = new UDim2(0, 0, 1, 0);
        FontSize = 48;
        Font = Global.UIFont;

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
        var FPS = Raylib.GetFPS();
        Text = FPS.ToString() + " FPS";
        MaxFPS = Math.Max(MaxFPS, FPS);

        if(FPS > MaxFPS / 2) {
            TextColor = Color.Lime;
        } else if(FPS > MaxFPS / 6) {
            TextColor = Color.Orange;
        } else if(FPS > MaxFPS / 8) {
            TextColor = Color.Red;
        }

        UpdatePos();
        base.Update(dt);
    }
}