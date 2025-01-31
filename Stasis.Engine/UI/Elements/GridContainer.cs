using System.Numerics;
using Raylib_cs;

namespace Stasis.Engine.UI.Elements;

public class GridContainer : UiElement {
    public float Padding;

    public float ItemsPerRow = 16;

    public bool SquareItems = true;

    public override void UpdateAbsoluteValues(Vector2 parentSize, Vector2 parentPosition) {
        int x = 0;
        int y = 0;
        var size = AbsoluteSize.X / ItemsPerRow - Padding;
        foreach(UiElement child in Children) {
            child.Size.X.Offset = size;
            if(SquareItems) child.Size.Y.Offset = size;

            if(!child.Visible) continue;

            if(x >= ItemsPerRow) {
                x = 0;
                y++;
            }

            float ySize;
            if(SquareItems) ySize = size;
            else ySize = child.AbsoluteSize.Y;

            child.Position.Y.Offset = y * (ySize + Padding) + Padding / 2f;
            child.Position.X.Offset = x * (size + Padding) + Padding / 2f;
            x++;
        }
        base.UpdateAbsoluteValues(parentSize, parentPosition);
    }

    public override void Render() {
        base.Render();
    }
}
