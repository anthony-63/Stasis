using Raylib_cs;
using Stasis.Game.Scenes.Game.Player;

namespace Stasis.Game.Scenes.Game.HUD;

public class Accuracy {
    double value = 100.0;

    public void Render() {
        Raylib.DrawText(value.ToString(), Raylib.GetRenderWidth() - 64, 0, 20, Color.White);
    }

    public void Update(Score score) {
        value = score.Accuracy;
    }
}