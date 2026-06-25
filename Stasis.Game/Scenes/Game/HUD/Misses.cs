using System.Numerics;
using Raylib_cs;
using Stasis.Engine.UI;
using Stasis.Engine.UI.Elements;
using Stasis.Game.Scenes.Game.Player;

namespace Stasis.Game.Scenes.Game.HUD;

public class Misses : Label
{
    Label MissesValue;

    public Misses()
    {
        OneLine = true;

        Size = new UDim2(0, 0, 1, 0);
        FontSize = 48;
        Font = Global.UIFont;

        AlignmentX = TextAlignX.Right;
        AlignmentY = TextAlignY.Top;
        Text = "Misses";

        TextColor = Color.Maroon;

        Position = new UDim2(0.99f, 0, 0.485f, 0);
        MissesValue = new Label
        {
            OneLine = true,
            Text = "0x",
            AlignmentX = AlignmentX,
            AlignmentY = AlignmentY,
            Position = new UDim2(Position.X.Scale, 0, 0, 35),
            Size = UDim2.Fill,
            FontSize = FontSize + 8,
            Font = Font,
            TextColor = Color.Red,
        };

        AddChild(MissesValue);
    }

    public override void Render()
    {
        base.Render();
    }

    public void Update(Score score)
    {
        MissesValue.Text = score.Misses.ToString() + "x";
    }
}