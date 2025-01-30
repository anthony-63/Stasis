using System.Data;
using System.Numerics;
using Raylib_cs;

namespace Stasis.Engine.UI.Elements;

public class SpinBox : TextBox {
    public float Value = 0;
    public float Step = 1;

    public string? Format = null;

    public override void UpdateAbsoluteValues(Vector2 parentPosition, Vector2 parentSize) {
        base.UpdateAbsoluteValues(parentPosition, parentSize);
    }

    public override void GetInput() {
        if(IsHovering()) {
            if(Raylib.GetMouseWheelMove() != 0) {
                if(Raylib.GetMouseWheelMove() > 0) Value += Step;
                if(Raylib.GetMouseWheelMove() < 0) Value -= Step;
                Text.Text = Value.ToString(Format ?? "");
            }
        }
        base.GetInput();
    }

    public override void Update(double dt) {
        if(float.TryParse(Text.Text, out float v)) {
            Value = v;
        }
        base.Update(dt);
    }

    public override void Render() {
        base.Render();
    }
}