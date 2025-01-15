using System.Numerics;
using Raylib_cs;

namespace Stasis.Engine.UI.Elements;

public class GridContainer : UiElement {
    public float Padding;

    public float ItemsPerRow = 16;

    public override void UpdateAbsoluteValues(Vector2 parentSize, Vector2 parentPosition) {
        int x = 0;
        int y = 0;
        var size = AbsoluteSize.X / ItemsPerRow - Padding;
        foreach(UiElement child in Children) {
            var csize = child.Size;
            csize.X.Offset = size;
            csize.Y.Offset = size;
            child.Size = csize;

            if(!child.Visible) continue;

            if(x >= ItemsPerRow) {
                x = 0;
                y++;
            }

            var cpos = child.Position;
            cpos.Y.Offset = y * (size + Padding) + Padding / 2f;
            cpos.X.Offset = x * (size + Padding) + Padding / 2f;
            child.Position = cpos;
            x++;
        }
        base.UpdateAbsoluteValues(parentSize, parentPosition);
    }

    public override void Render() {
        base.Render();
    }
}
