using System.Numerics;
using Raylib_cs;

namespace Stasis.Engine.UI.Elements;

public class GridContainer : UiElement {
    public float Padding;

    public override void UpdateAbsoluteValues(Vector2 parentSize, Vector2 parentPosition) {
        base.UpdateAbsoluteValues(parentSize, parentPosition);
    }

    public override void Render() {
        base.Render();
    }
}
