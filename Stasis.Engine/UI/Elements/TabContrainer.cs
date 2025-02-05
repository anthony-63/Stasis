using System.Numerics;
using System.Reflection;
using Raylib_cs;

namespace Stasis.Engine.UI.Elements;

public class TabContainerTab : Frame {
    public Button SwapTo = new();
}

public class TabContainer : UiElement {
    public List<TabContainerTab> Tabs = new();

    public int TabIndex = 0;
    
    public override void UpdateAbsoluteValues(Vector2 parentSize, Vector2 parentPosition) {
        base.UpdateAbsoluteValues(parentSize, parentPosition);

        for(int i = 0; i < Tabs.Count; i++) {
            Tabs[i].UpdateAbsoluteValues(parentSize, parentPosition);
            Tabs[i].SwapTo.UpdateAbsoluteValues(parentSize, parentPosition);
        }
    }

    public override void Update(double dt) {
        if(!Visible) return;
        for(int i = 0; i < Tabs.Count; i++) {
            var index = i;
            Tabs[i].SwapTo.PressedOnce ??= () => TabIndex = index;
            Tabs[i].Update(dt);
            Tabs[i].SwapTo.Update(dt);
            Tabs[i].Visible = i == TabIndex;
            Tabs[i].IgnoreUpdate = i != TabIndex;
        }

        base.Update(dt);
    }

    public override void Render() {
        base.Render();
        for(int i = 0; i < Tabs.Count; i++) {
            Tabs[i].Render();
            Tabs[i].SwapTo.Render();
        }
    }
}