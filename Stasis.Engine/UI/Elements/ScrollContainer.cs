using System.Numerics;
using System.Security.Cryptography;
using Raylib_cs;

namespace Stasis.Engine.UI.Elements;

public class ScrollContainer : UiElement {
    public float Scroll = 0f;
    public float Sensitivity = 50f;

    public override void UpdateAbsoluteValues(Vector2 parentSize, Vector2 parentPosition) {
        base.UpdateAbsoluteValues(parentSize, parentPosition);
    }

    void GetMaxYAllChildren(UiElement element, ref float max) {
        foreach(UiElement child in element.Children) {
            var extra = 0f;
            if(child is GridContainer grid) extra = grid.Padding;  
            max = Math.Max(max, child.AbsolutePosition.Y + child.AbsoluteSize.Y + extra);
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
            Scroll = Math.Clamp(Scroll, -maxY, 0);

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