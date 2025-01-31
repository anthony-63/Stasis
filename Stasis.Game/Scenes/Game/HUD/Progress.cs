using System.Numerics;
using Microsoft.VisualBasic;
using Raylib_cs;
using Stasis.Engine.UI;
using Stasis.Engine.UI.Elements;
using Stasis.Game.Scenes.Game.Player;

namespace Stasis.Game.Scenes.Game.HUD;

public class Progress : Label {
    string MapLengthString;

    public Progress() {
        OneLine = true;

        var mapLengthTimespan = TimeSpan.FromSeconds(Global.SelectedMap?.Difficulties[0].Notes.Last().Time ?? 0);
        MapLengthString = string.Format("{0:D1}:{1:D2}", mapLengthTimespan.Minutes, mapLengthTimespan.Seconds);

        Size = new UDim2(0, 0, 1, 0);
        FontSize = 38;
        Font = Global.UIFont;
        TextColor = Color.DarkGray;

        AlignmentX = TextAlignX.Right;
        AlignmentY = TextAlignY.Top;
        Text = "0:00/0:00";


        Position = new UDim2(0.995f, 0, 0, 0);
    }

    public override void Render() {
        base.Render();
    }

    public void UpdatePos() {
        var pos = Position;
        pos.Y.Offset = Raylib.GetRenderHeight() - 90;
        Position = pos;
    }

    public void Update(float currentTime) {
        var progressTimespan = TimeSpan.FromSeconds(currentTime);
        var progressString = String.Format("{0:D1}:{1:D2}", progressTimespan.Minutes, progressTimespan.Seconds);

        Text = progressString + "/" + MapLengthString;
        UpdatePos();
    }
}