using System.Globalization;
using System.Numerics;
using Raylib_cs;
using Stasis.Engine;
using Stasis.Engine.UI;
using Stasis.Engine.UI.Elements;
using Stasis.Game.Scenes.Game.Player;

namespace Stasis.Game.Scenes.Game.HUD;

public class Timer : ProgressBar {
    static readonly float Height = 16;

    public Timer() {
        Anchor = UiElementAnchor.BottomLeft;
        Size = new UDim2(1, 0, 0, Height);
        Position = new UDim2(0, 0, 1, 0);
        Foreground.Color = Color.SkyBlue;
        Background.Color = Color.DarkGray;
    }

    public void Update(float currentTime) {
        Value = currentTime;
    }
}