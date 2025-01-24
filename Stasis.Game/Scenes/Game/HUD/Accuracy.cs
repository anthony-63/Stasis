using System.Numerics;
using Raylib_cs;
using Stasis.Engine.UI;
using Stasis.Engine.UI.Elements;
using Stasis.Game.Scenes.Game.Player;

namespace Stasis.Game.Scenes.Game.HUD;

public class Accuracy : Label {
    Label AccValue;

    public Accuracy() {
        OneLine = true;

        Size = new UDim2(0, 0, 1, 0);
        FontSize = 48;
        Font = Global.UIFont;

        AlignmentX = TextAlignX.Right;
        AlignmentY = TextAlignY.Top;
        Text = "Accuracy";

        TextColor = Color.Lime;

        Position = new UDim2(0.99f, 0, 0.375f, 0);
        AccValue = new Label {
            OneLine = true,
            Text = 100f.ToString("0.00") + "%",
            AlignmentX = AlignmentX,
            AlignmentY = AlignmentY,
            Position = new UDim2(Position.X.Scale, 0, 0, 35),
            Size = UDim2.Fill,
            FontSize = FontSize + 8,
            Font = Font,
            TextColor = Color.Green,
        };

        AddChild(AccValue);
    }

    public override void Render() {
        base.Render();
    }

    public void Update(double dt, Score score) {
        AccValue.Text = score.Accuracy.ToString("0.00") + "%";
        base.Update(dt);
    }
}