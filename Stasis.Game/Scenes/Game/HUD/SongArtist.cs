using System.Numerics;
using Raylib_cs;
using Stasis.Engine.UI;
using Stasis.Engine.UI.Elements;
using Stasis.Game.Scenes.Game.Player;

namespace Stasis.Game.Scenes.Game.HUD;

public class SongArtist : Label {
    public SongArtist() {
        OneLine = true;

        Size = UDim2.Fill;
        FontSize = 48;
        Font = Global.UIFont;

        AlignmentX = TextAlignX.Center;
        AlignmentY = TextAlignY.Top;
        Anchor = UiElementAnchor.TopMiddle;
        Text = Global.SelectedMap?.Title ?? "Umeboshi Chazuke - AAAAA";

        TextColor = Color.White;

        Position = new UDim2(0.5f, 0, 0.01f, 0);
    }

    public override void Render() {
        base.Render();
    }

    public override void Update(double dt) {
        base.Update(dt);
    }
}