using System.Numerics;
using Raylib_cs;

namespace Stasis.Engine.UI.Elements;

public class Button : UiElement {
    public Frame NormalFrame = new();
    public Frame PressedFrame = new();
    public Frame HoveringFrame = new();
    public Frame DisabledFrame = new();
    
    public Label Label = new();

    public ButtonState State = ButtonState.Normal;
    public delegate void ButtonEvent();

    public event ButtonEvent? PressedOnce;
    public event ButtonEvent? Holding;
    public event ButtonEvent? Released;
    public event ButtonEvent? Hovering;

    public override void Update(double dt) {
        if(!Visible || IgnoreUpdate) return;
        
        if(IsHovering() && State != ButtonState.Pressed && State != ButtonState.Holding && Raylib.IsMouseButtonPressed(MouseButton.Left)) {
            State = ButtonState.Pressed;
            Raylib.SetMouseCursor(MouseCursor.Arrow);
            if(PressedOnce is not null) PressedOnce();
        } else if(IsHovering() && Raylib.IsMouseButtonDown(MouseButton.Left) && State == ButtonState.Pressed | State == ButtonState.Holding) {
            State = ButtonState.Holding;
            if(Holding is not null) Holding();
        } else if(IsHovering()) {
            State = ButtonState.Hovering;
            Raylib.SetMouseCursor(MouseCursor.PointingHand);
            if(Hovering is not null) Hovering();
        } else if(!IsHovering() && State == ButtonState.Pressed || State == ButtonState.Holding || !IsHovering() && State == ButtonState.Hovering) {
            State = ButtonState.Normal;
            Raylib.SetMouseCursor(MouseCursor.Arrow);
            if(Released is not null) Released();
        } else State = ButtonState.Normal;
        
        Label.Update(dt);
        base.Update(dt);
    }

    public override void Render() {
        if(!Visible) return;
        switch(State) {
            case ButtonState.Normal: NormalFrame.Render(); break;
            case ButtonState.Pressed: PressedFrame.Render(); break;
            case ButtonState.Hovering: HoveringFrame.Render(); break;
            case ButtonState.Disabled: DisabledFrame.Render(); break;
        }

        Label.Render();

        base.Render();
    }

    public override void UpdateAbsoluteValues(Vector2 parentSize, Vector2 parentPosition) {
        base.UpdateAbsoluteValues(parentSize, parentPosition);
        if(!Visible) return;
        
        NormalFrame.SetAbsoluteValues(AbsolutePosition, AbsoluteSize);
        PressedFrame.SetAbsoluteValues(AbsolutePosition, AbsoluteSize);
        HoveringFrame.SetAbsoluteValues(AbsolutePosition, AbsoluteSize);
        DisabledFrame.SetAbsoluteValues(AbsolutePosition, AbsoluteSize);
        Label.UpdateAbsoluteValues(NormalFrame.AbsoluteSize, NormalFrame.AbsolutePosition);
    }
}

public enum ButtonState {
    Normal,
    Pressed,
    Holding,
    Hovering,
    Disabled,
}