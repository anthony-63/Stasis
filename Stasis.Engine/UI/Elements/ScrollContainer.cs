using System.Numerics;
using Raylib_cs;

namespace Stasis.Engine.UI.Elements;

public class ScrollContainer : UiElement {
    public float Scroll = 0f;
    public float Sensitivity = 25f;

    public override void UpdateAbsoluteValues(Vector2 parentSize, Vector2 parentPosition) {
        base.UpdateAbsoluteValues(parentSize, parentPosition);
    }

    void GetMaxYAllChildren(UiElement element, ref float max) {
        foreach(UiElement child in element.Children) {
            max = Math.Max(max, child.AbsolutePosition.Y + child.AbsoluteSize.Y);
            if(child.Children.Count > 0) GetMaxYAllChildren(child, ref max);
        }
    }

    public override void Update(double dt) {
        if(!Visible) return;
        if(IsHovering()) {
            var mdelt = Raylib.GetMouseWheelMove() * Sensitivity;
            Scroll += mdelt;

            var maxY = 0f; 
            GetMaxYAllChildren(this, ref maxY);
            if(maxY <= Raylib.GetRenderHeight() && mdelt < 0) {
                Scroll -= mdelt;
            } 
            Scroll = -Math.Max(-Scroll, 0);
            foreach(UiElement child in Children) {
                child.Position.Y.Offset = Scroll;
                if(child.AbsolutePosition.Y > Raylib.GetRenderHeight()) {
                    child.Visible = false;
                }
            }
        }
        
        base.Update(dt);
    }

    public override void Render() {
        base.Render();
    }
}