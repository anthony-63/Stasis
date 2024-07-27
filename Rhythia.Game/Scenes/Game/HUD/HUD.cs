using Raylib_cs;
using Rhythia.Engine.UI;
using Rhythia.Engine.UI.Elements;
using Rhythia.Game.Scenes.Game.Player;

namespace Rhythia.Game.Scenes.Game.HUD;

public class HUDRoot {
    Timer Timer = new Timer();

    public HUDRoot() {
        Timer.EndTime = Global.SelectedMap?.Difficulties[0].Notes.Last().Time ?? 0f;
    }

    public void Render() {
        Timer.Render();
    }

    public void Update(float currentTime) {
        Timer.Update(currentTime);
    }
}