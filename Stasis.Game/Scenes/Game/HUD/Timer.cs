using Raylib_cs;
using Stasis.Engine;
using Stasis.Engine.UI;
using Stasis.Engine.UI.Elements;

namespace Stasis.Game.Scenes.Game.HUD;

public class Timer : ProgressBar
{
    static readonly float Height = 16;

    public Label Skip;

    public Timer()
    {
        Anchor = UiElementAnchor.BottomLeft;
        Size = new UDim2(1, 0, 0, Height);
        Position = new UDim2(0, 0, 1, 0);
        Foreground.Color = Global.Replay is null ? Color.SkyBlue : Raylib.GetColor(0xF5A976ff);
        Background.Color = Color.DarkGray;

        Skip = new()
        {
            AlignmentX = TextAlignX.Center,
            AlignmentY = TextAlignY.Top,
            Position = new UDim2(0.5f, 0, 0, -48),
            Text = "Press Space To Skip...",
            OneLine = true,
            FontSize = 48,
            Font = Global.UIFont,
            Visible = false,
            TextColor = Color.Gray,
        };

        AddChild(Skip);
    }

    public void Update(float currentTime, bool skippable)
    {
        Value = currentTime;
        Skip.Visible = skippable;
    }
}