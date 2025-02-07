using System.Numerics;
using Raylib_cs;

namespace Stasis.Engine.UI.Elements;

public class TextBox : UiElement {
    public Label Text = new();
    public Label Placeholder = new() {
        Text = "Enter Text...",
        AlignmentX = TextAlignX.Left,
        AlignmentY = TextAlignY.Middle,
        TextColor = Color.Gray,
        Size = UDim2.Fill,
    };

    private bool resetCursor = false;

    public Frame NormalFrame = new();
    public Frame FocusedFrame = new();
    public Frame DisabledFrame = new();

    public float CaretTimer = 0.5f;
    private float timer = 0f;
    private bool caretVisible = true;

    public bool IsNumeric = false;

    public TextBoxState State = TextBoxState.Normal;

    public override void UpdateAbsoluteValues(Vector2 parentPosition, Vector2 parentSize) {
        base.UpdateAbsoluteValues(parentPosition, parentSize);
        NormalFrame.SetAbsoluteValues(AbsolutePosition, AbsoluteSize);
        FocusedFrame.SetAbsoluteValues(AbsolutePosition, AbsoluteSize);
        DisabledFrame.SetAbsoluteValues(AbsolutePosition, AbsoluteSize);
        Text.UpdateAbsoluteValues(NormalFrame.AbsoluteSize, NormalFrame.AbsolutePosition);
        Placeholder.UpdateAbsoluteValues(NormalFrame.AbsoluteSize, NormalFrame.AbsolutePosition);
    }

    public virtual void GetInput() {
        var c = Raylib.GetCharPressed();
        if((Raylib.IsKeyPressed(KeyboardKey.Backspace) || Raylib.IsKeyPressedRepeat(KeyboardKey.Backspace)) && Text.Text.Length > 0) Text.Text = Text.Text[..^1];
        else if(c != 0) Text.Text += (char)c;
    }

    public override void Update(double dt) {
        base.Update(dt);
        Text.OneLine = true;
        timer += (float)dt;
        if(timer > CaretTimer) {
            timer = 0;
            caretVisible = !caretVisible;
        }

        if(State == TextBoxState.Focused) {
            GetInput();
        }
        if(IsHovering()) {
            if(Raylib.IsMouseButtonPressed(MouseButton.Left)) {
                State = TextBoxState.Focused;
            } else if(State != TextBoxState.Focused) {
                State = TextBoxState.Hovering;
            }
            resetCursor = false;
            Raylib.SetMouseCursor(MouseCursor.IBeam);
        } else if(!IsHovering() && Raylib.IsMouseButtonPressed(MouseButton.Left)) {
            State = TextBoxState.Normal;
        } else if(!IsHovering() && !resetCursor) {
            Raylib.SetMouseCursor(MouseCursor.Arrow);
            resetCursor = true;
        }

        Text.Update(dt);
        Placeholder.Update(dt);
        NormalFrame.Update(dt);
        FocusedFrame.Update(dt);
        DisabledFrame.Update(dt);
    }

    public override void Render() {
        switch(State) {
            case TextBoxState.Normal: NormalFrame.Render(); break;
            case TextBoxState.Hovering: NormalFrame.Render(); break;
            case TextBoxState.Focused: FocusedFrame.Render(); break;
            case TextBoxState.Disabled: DisabledFrame.Render(); break;
        }

        if(Text.Text == "" && State != TextBoxState.Focused) Placeholder.Render(); 
        else Text.Render();

        if(caretVisible && State == TextBoxState.Focused) {
            var textSize = Text.lines[0].Item2;
            Raylib.DrawLineEx(
                new Vector2(Text.AbsolutePosition.X + textSize.X + Text.TextOrigin.X + 3, NormalFrame.AbsolutePosition.Y + 3),
                new Vector2(Text.AbsolutePosition.X+textSize.X + Text.TextOrigin.X + 3, NormalFrame.AbsolutePosition.Y + NormalFrame.AbsoluteSize.Y - 3),
                0.5f, Text.TextColor);
        }
        base.Render();
    }
}

public enum TextBoxState {
    Normal,
    Hovering,
    Focused,
    Disabled,
}