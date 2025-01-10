using System.Numerics;
using Raylib_cs;

namespace Stasis.Engine.UI.Elements;

public class GridContainer : UiElement {
    public float Padding;

    public float ItemsPerRow = 16;

    public override void UpdateAbsoluteValues(Vector2 parentSize, Vector2 parentPosition) {
        base.UpdateAbsoluteValues(parentSize, parentPosition);
        int x = 0;
        int y = 0;
        var size = AbsoluteSize.X / ItemsPerRow - Padding;
        foreach(UiElement child in Children) {
            child.Size.X.Offset = size;
            child.Size.Y.Offset = size;
            if(!child.Visible) continue;

            if(x >= ItemsPerRow) {
                x = 0;
                y++;
            }
            child.Position.Y.Offset = y * (size + Padding) + Padding / 2f;
            child.Position.X.Offset = x * (size + Padding) + Padding / 2f;
            x++;
        }
    }

    public override void Render() {
        base.Render();
    }
}
