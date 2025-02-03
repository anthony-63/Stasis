using System.Globalization;
using System.Numerics;
using Raylib_cs;
using Stasis.Engine;
using Stasis.Engine.UI;
using Stasis.Engine.UI.Elements;
using Stasis.Game.Scenes.Game.Player;

namespace Stasis.Game.Scenes.Game.HUD;

public class DebugStats : Label {

    public DebugStats() {
        Size = new UDim2(1f, 0, 1f, 0);
        Position = new UDim2(0, 15, 0, 15);
        AlignmentX = TextAlignX.Left;
        AlignmentY = TextAlignY.Top;
        TextColor = Color.Pink;
        FontSize = 32;
        Font = Global.UIFont;
    }

    public void Update(int allocatedInstances, int startProcess, float time, int visibleCount, float health, float healthStep) {
        Text =
        $"audio_time: {time}\n" +
        $"visible_note_count: {visibleCount}\n" +
        $"allocated_instances: {allocatedInstances}\n" +
        $"start_process: {startProcess}\n" +
        $"health: {health}\n" + 
        $"health_step: {healthStep}";
    }
}