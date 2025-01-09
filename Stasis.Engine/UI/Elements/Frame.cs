using System.Numerics;
using Raylib_cs;

namespace Stasis.Engine.UI.Elements;

public class Frame : UiElement {
    public Color Color = Color.White;

    public Color BorderColor = Color.Gray;
    public float BorderWidth = 0f;
    public float Roundness = 0f;

    public override void UpdateAbsoluteValues(Vector2 parentSize, Vector2 parentPosition) {
        base.UpdateAbsoluteValues(parentSize, parentPosition);
    }

    public override void Render() {
        if(BorderWidth > 0) {
            Raylib.DrawRectangleRounded(new Rectangle {
                X = AbsolutePosition.X - BorderWidth,
                Y = AbsolutePosition.Y - BorderWidth,
                Width = AbsoluteSize.X + BorderWidth * 2,
                Height = AbsoluteSize.Y + BorderWidth * 2,
            }, Roundness, 0, BorderColor);
        }

        Raylib.DrawRectangleRounded(new Rectangle {
            X = AbsolutePosition.X,
            Y = AbsolutePosition.Y,
            Width = AbsoluteSize.X,
            Height = AbsoluteSize.Y,
        }, Roundness, 0, Color);

        base.Render();
    }
}
