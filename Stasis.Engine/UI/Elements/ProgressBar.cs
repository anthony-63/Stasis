using System.Numerics;
using System.Security.Cryptography;
using Raylib_cs;

namespace Stasis.Engine.UI.Elements;

public class ProgressBar : UiElement {

    public Frame Background = new() {
        Size = new UDim2(1, 0, 1, 0),
        Color = Color.DarkGray,
    };

    public Frame Foreground = new() {
        Size = new UDim2(1, 0, 1, 0),
        Color = Color.White,
    };

    public float Value = 25f;
    public float MaxValue = 100f;

    public override void UpdateAbsoluteValues(Vector2 parentSize, Vector2 parentPosition) {
        base.UpdateAbsoluteValues(parentSize, parentPosition);
        Background.UpdateAbsoluteValues(AbsoluteSize, AbsolutePosition);
        Foreground.UpdateAbsoluteValues(Background.AbsoluteSize, Background.AbsolutePosition);
    }

    void GetMaxYAllChildren(UiElement element, ref float max) {
        foreach(UiElement child in element.Children) {
            max = Math.Max(max, child.AbsolutePosition.Y - child.AbsoluteSize.Y * 3.5f);
            if(child.Children.Count > 0) GetMaxYAllChildren(child, ref max);
        }
    }

    public override void Update(double dt) {
        if(!Visible) return;
        
        var val = Value / MaxValue;
        Foreground.Size.X.Scale = val;

        Foreground.Update(dt);
        Background.Update(dt);

        base.Update(dt);
    }

    public override void Render() {
        Background.Render();
        Foreground.Render();
        base.Render();
    }
}