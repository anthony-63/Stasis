using System.Numerics;
using System.Security.Cryptography;
using Raylib_cs;

namespace Stasis.Engine.UI.Elements;

public class ScrollContainer : UiElement {
    public float Scroll = 0f;
    public float Sensitivity = 50f;

    private Vector2? constantPosition;
    private float maxY = 0;

    public override void UpdateAbsoluteValues(Vector2 parentSize, Vector2 parentPosition) {
        base.UpdateAbsoluteValues(parentSize, parentPosition);
        constantPosition ??= AbsolutePosition;
    }

    void GetMaxYAllChildren(UiElement element, ref float max) {
        foreach(UiElement child in element.Children) {
            if(child.Children.Count > 0) GetMaxYAllChildren(child, ref max);
            max = Math.Max(max, child.AbsolutePosition.Y - child.AbsoluteSize.Y * 3.5f);
        }
    }

    public override void Update(double dt) {
        if(!Visible) return;
        if(Scroll >= 0) {
            maxY = 0;
            GetMaxYAllChildren(this, ref maxY);
        }
        if(IsHovering(constantPosition ?? AbsolutePosition)) {
            var mdelt = Raylib.GetMouseWheelMove() * Sensitivity;
            Scroll += mdelt;
            Scroll = Math.Clamp(Scroll, -maxY, 0);
            Position.Y.Offset = Scroll;
        }
        
        base.Update(dt);
    }

    public override void Render() {
        base.Render();
    }
}