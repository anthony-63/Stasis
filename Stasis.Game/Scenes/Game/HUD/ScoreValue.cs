using System.Numerics;
using System.Runtime.CompilerServices;
using Raylib_cs;
using Stasis.Engine.UI;
using Stasis.Engine.UI.Elements;
using Stasis.Game.Scenes.Game.Player;

namespace Stasis.Game.Scenes.Game.HUD;

public class ScoreValue : Label
{
    public ScoreValue()
    {
        OneLine = true;

        Size = new UDim2(0, 0, 1, 0);
        FontSize = 48;
        Font = Global.UIFont;

        AlignmentX = TextAlignX.Left;
        AlignmentY = TextAlignY.Top;
        Text = "0";

        TextColor = Color.LightGray;

        Position = new UDim2(0.005f, 0, 0, 0);
    }

    public override void Render()
    {
        base.Render();
    }

    public void UpdatePos()
    {
        var pos = Position;
        pos.Y.Offset = Raylib.GetRenderHeight() - 60;
        Position = pos;
    }

    public void Update(Score score)
    {
        Text = score.ScoreValue.ToString();
        UpdatePos();
    }
}