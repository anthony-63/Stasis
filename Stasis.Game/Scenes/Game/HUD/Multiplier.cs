using System.Numerics;
using Raylib_cs;
using Stasis.Engine.UI;
using Stasis.Engine.UI.Elements;
using Stasis.Game.Scenes.Game.Player;

namespace Stasis.Game.Scenes.Game.HUD;

public class Multiplier : Label {
    Frame BG;
    Frame FG;

    public Multiplier() {
        OneLine = true;

        Size = new UDim2(0, 0, 1, 0);
        FontSize = 52;
        Font = "Assets/Game/font.ttf";

        AlignmentX = TextAlignX.Left;
        AlignmentY = TextAlignY.Top;
        Text = "1X";

        TextColor = Color.SkyBlue;

        Position = new UDim2(0.005f, 0, 0.495f, 0);

        BG = new() {
            Roundness = 0.4f,
            Color = Color.DarkGray,
            Position = new UDim2(0, 0, 0, 45),
            Size = new UDim2(0, 225, 0, 12),
        };
        FG = new() {
            Roundness = 0.4f,
            Color = Color.SkyBlue,
            Size = new UDim2(1f / 8f, 0, 1, 0),
        };

        BG.AddChild(FG);

        AddChild(BG);
    }

    public override void Render() {
        base.Render();
    }


    public void Update(double dt, Score score) {
        Text = score.Multipier + "X";

        var sz = FG.Size;
        sz.X.Scale = score.Multipier / 8f;
        FG.Size = sz;

        base.Update(dt);
    }
}