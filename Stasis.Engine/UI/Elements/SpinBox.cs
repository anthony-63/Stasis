using System.Data;
using System.Numerics;
using Raylib_cs;

namespace Stasis.Engine.UI.Elements;

public class SpinBox : TextBox {
    public float _value = 0;
    public float Value {
        get => _value;
        set {
            _value = value;
            Text.Text = value.ToString();
        }
    }
    
    public float Step = 1;

    public string? Format = null;

    public delegate void ValueChangedEvent(float value);
    public ValueChangedEvent? ValueChanged;

    public override void UpdateAbsoluteValues(Vector2 parentPosition, Vector2 parentSize) {
        base.UpdateAbsoluteValues(parentPosition, parentSize);
    }

    public override void GetInput() {
        if(IsHovering()) {
            if(Raylib.GetMouseWheelMove() != 0) {
                if(Raylib.GetMouseWheelMove() > 0) _value += Step;
                if(Raylib.GetMouseWheelMove() < 0) _value -= Step;
                _value = (float)Math.Round(_value, 4);

                if(ValueChanged is not null) ValueChanged(Value);
                Text.Text = Value.ToString(Format ?? "");
            }
        }
        base.GetInput();
    }

    public override void Update(double dt) {
        var lastValue = Value;
        if(float.TryParse(Text.Text, out float v)) {
            _value = v;
            if(ValueChanged is not null && lastValue != v) ValueChanged(v);
        }
        base.Update(dt);
    }

    public override void Render() {
        base.Render();
    }
}