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

    void GetMaxYAllChildren(UiElement element, ref float max, bool parent = true) {
        maxY = 0;
        foreach(UiElement child in element.Children) {
            if(!child.Visible) continue;
            if(child.Children.Count > 0 && parent) GetMaxYAllChildren(child, ref max, false);
            max = Math.Max(max, child.AbsolutePosition.Y + child.AbsoluteSize.Y - Scroll);
        }
        if(parent) maxY -= AbsoluteSize.Y;
    }

    public override void Update(double dt) {
        if(!Visible) return;
        if(IsHovering(constantPosition ?? AbsolutePosition)) {
            var mdelt = Raylib.GetMouseWheelMove() * Sensitivity;
            GetMaxYAllChildren(this, ref maxY);
            Scroll += mdelt;

            if(Raylib.IsKeyPressed(KeyboardKey.Down)) Scroll -= 500f;
            else if(Raylib.IsKeyPressed(KeyboardKey.Up)) Scroll += 500f;

            Scroll = Math.Clamp(Scroll, -maxY, 0);
            Position.Y.Offset = Scroll;
        }

        base.Update(dt);
    }

    public override void SetClipDim() {
        ScissorRect = new Rectangle() {
            X = AbsolutePosition.X,
            Y = AbsolutePosition.Y - Scroll,
            Width = AbsoluteSize.X,
            Height = AbsoluteSize.Y,
        };
    }

    public override void Render() {
        base.Render();
    }
}