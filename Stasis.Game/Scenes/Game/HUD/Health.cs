using System.Globalization;
using System.Numerics;
using Raylib_cs;
using Stasis.Engine;
using Stasis.Engine.UI;
using Stasis.Engine.UI.Elements;
using Stasis.Game.Scenes.Game.Player;

namespace Stasis.Game.Scenes.Game.HUD;

public class Health : ProgressBar {

    public Health() {
        Anchor = UiElementAnchor.BottomLeft;
        Size = new UDim2(0.3f, 0, 0, 14);
        Position = new UDim2(0.5f, 0, 0, 70);
        Anchor = UiElementAnchor.TopMiddle;
        Background = new() {
            Roundness = 1.5f,
            Size = UDim2.Fill,
            Color = Color.Red,
        };
        Foreground = new() {
            Roundness = 1.5f,
            Size = UDim2.Fill,
            Color = Color.Green,
        };
    }

    public void Update(Score score) {
        Value = score.Health;
    }
}