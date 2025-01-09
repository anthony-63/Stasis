using Raylib_cs;
using Stasis.Engine.UI;
using Stasis.Engine.UI.Elements;
using Stasis.Game.Scenes.Game.Player;

namespace Stasis.Game.Scenes.Game.HUD;

public class HUDRoot {
    Timer Timer = new Timer();
    Accuracy Accuracy = new Accuracy();

    public HUDRoot() {
        Timer.EndTime = Global.SelectedMap?.Difficulties[0].Notes.Last().Time ?? 0f;
    }

    public void Render() {
        Timer.Render();
        Accuracy.Render();
    }

    public void Update(float currentTime, Score score) {
        Timer.Update(currentTime);
        Accuracy.Update(score);
    }
}