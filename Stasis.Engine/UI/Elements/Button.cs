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
    public delegate void ButtonToggleEvent(bool value);

    public ButtonEvent? PressedOnce;
    public ButtonEvent? Holding;
    public ButtonEvent? Released;
    public ButtonEvent? Hovering;

    public ButtonToggleEvent? Toggled;

    public bool Toggle = false;
    public bool ToggledValue = false;

    private void UpdateNoToggle() {
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
    }

    private void UpdateToggle() {
        if(IsHovering() && !ToggledValue && Raylib.IsMouseButtonPressed(MouseButton.Left)) {
            State = ButtonState.Pressed;
            ToggledValue = true;
            if(Toggled is not null) Toggled(true);
            Raylib.SetMouseCursor(MouseCursor.Arrow);
            if(PressedOnce is not null) PressedOnce();
        } else if(IsHovering() && ToggledValue && Raylib.IsMouseButtonPressed(MouseButton.Left)) {
            State = ButtonState.Hovering;
            ToggledValue = false;
            if(Toggled is not null) Toggled(false);
            Raylib.SetMouseCursor(MouseCursor.Arrow);
            if(PressedOnce is not null) PressedOnce();
        } else if(IsHovering() && State != ButtonState.Pressed) {
            State = ButtonState.Hovering;
            Raylib.SetMouseCursor(MouseCursor.PointingHand);
            if(Hovering is not null) Hovering();
        } else if(!IsHovering() && State == ButtonState.Pressed || State == ButtonState.Holding || !IsHovering() && State == ButtonState.Hovering) {
            State = ButtonState.Normal;
            Raylib.SetMouseCursor(MouseCursor.Arrow);
            if(Released is not null) Released();
        } else State = ButtonState.Normal;
    }

    public override void Update(double dt) {
        if(!Visible || IgnoreUpdate) return;
        if(Toggle) UpdateToggle();
        else UpdateNoToggle();
        
        Label.Update(dt);
        base.Update(dt);
    }

    public override void Render() {
        if(!Visible) return;

        if(Toggle) {
            if(ToggledValue) PressedFrame.Render();
            else if(State == ButtonState.Hovering) HoveringFrame.Render();
            else NormalFrame.Render();
        } else{
            switch(State) {
                case ButtonState.Normal: NormalFrame.Render(); break;
                case ButtonState.Holding:
                case ButtonState.Pressed: PressedFrame.Render(); break;
                case ButtonState.Hovering: HoveringFrame.Render(); break;
                case ButtonState.Disabled: DisabledFrame.Render(); break;
            }
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