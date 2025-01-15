using System.Numerics;
using Raylib_cs;

namespace Stasis.Engine.UI.Elements;



public class Button : UiElement {
    public Frame NormalFrame = new Frame();
    public Frame PressedFrame = new Frame();
    public Frame HoveringFrame = new Frame();
    public Frame DisabledFrame = new Frame();
    
    public Label Label = new Label();

    public ButtonState State = ButtonState.Normal;
    public delegate void ButtonEvent();

    public event ButtonEvent? PressedOnce;
    public event ButtonEvent? Holding;
    public event ButtonEvent? Released;
    public event ButtonEvent? Hovering;

    public override void Update(double dt) {
        if(IsHovering() && State != ButtonState.Pressed && State != ButtonState.Holding && Raylib.IsMouseButtonDown(MouseButton.Left)) {
            State = ButtonState.Pressed;
            if(PressedOnce is not null) PressedOnce();
        } else if(IsHovering() && Raylib.IsMouseButtonDown(MouseButton.Left) && State == ButtonState.Pressed | State == ButtonState.Holding) {
            State = ButtonState.Holding;
            if(Holding is not null) Holding();
        } else if(IsHovering()) {
            State = ButtonState.Hovering;
            if(Hovering is not null) Hovering();
        } else if(!IsHovering() && State == ButtonState.Pressed || State == ButtonState.Holding) {
            State = ButtonState.Normal;
            if(Released is not null) Released();
        }
        else State = ButtonState.Normal;
        
        Label.Update(dt);
        base.Update(dt);
    }

    public override void Render() {
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
        NormalFrame.Position = Position;
        NormalFrame.Size = Size;

        base.UpdateAbsoluteValues(parentSize, parentPosition);
        
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