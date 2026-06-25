using System.Formats.Asn1;
using System.Numerics;
using Raylib_cs;
using Stasis.Engine;
using Stasis.Engine.UI;
using Stasis.Engine.UI.Elements;
using Stasis.Game.Scenes.Game.Player;

namespace Stasis.Game.Scenes.Game.HUD;

public class Multiplier : Label
{
    ProgressBar Value;

    Label Encouragement;

    public Multiplier()
    {
        OneLine = true;

        Size = new UDim2(0, 0, 1, 0);
        FontSize = 52;
        Font = Global.UIFont;

        AlignmentX = TextAlignX.Left;
        AlignmentY = TextAlignY.Top;
        Text = "1X";

        TextColor = Color.SkyBlue;

        Position = new UDim2(0.005f, 0, 0.40f, 0);

        Value = new()
        {
            Position = new UDim2(0, 0, 0, 50),
            Size = new UDim2(0, 175, 0, 12),
            Background = new()
            {
                Roundness = 1.5f,
                Size = UDim2.Fill,
                BorderWidth = 4,
                BorderColor = Color.White,
                Color = Color.DarkGray,
                Rotation = 90,
            },
            Foreground = new()
            {
                Roundness = 1.5f,
                Size = UDim2.Fill,
                Color = Color.SkyBlue,
                Rotation = 90,
            },
            Value = 1,
            MaxValue = 8,
        };

        Encouragement = new()
        {
            FontSize = 24,
            Font = Global.UIFont,
            Size = Size,
            AlignmentX = TextAlignX.Left,
            AlignmentY = TextAlignY.Top,
            Position = new UDim2(0, 0, 0, 70),
            Text = EncouragementMessages.Messages[0],
            OneLine = true,
        };

        AddChild(Value);
        AddChild(Encouragement);
    }


    public void Update(Score score)
    {
        Text = score.Multipier + "X";
        Value.Value = score.Multipier;

        Encouragement.Text = EncouragementMessages.Messages[score.Multipier - 1];
    }
}