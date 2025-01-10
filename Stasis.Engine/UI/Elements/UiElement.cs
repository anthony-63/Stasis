using System.Numerics;
using Raylib_cs;

namespace Stasis.Engine.UI.Elements;

public class UiElement : IUiElement {
    private Vector2 absoluteSize = Vector2.Zero;
    private Vector2 absolutePosition = Vector2.Zero;

    private bool visible = true;

    public UiElementAnchor Anchor = UiElementAnchor.TopLeft;

    public Vector2 AbsoluteSize => absoluteSize;
    public Vector2 AbsolutePosition => absolutePosition;

    public bool Visible { get => visible; set => visible = value; }

    public UDim2 Size = UDim2.Zero;
    public UDim2 Position = UDim2.Zero;

    public List<IUiElement> Children = new();

    public virtual void Update(double dt) {
        foreach (var element in Children) element.Update(dt);
    }

    public virtual bool IsHovering() {
        return Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), new Rectangle {
            X = AbsolutePosition.X,
            Y = AbsolutePosition.Y,
            Width = AbsoluteSize.X,
            Height = AbsoluteSize.Y,
        });
    }

    public virtual void UpdateAbsoluteValues(Vector2 parentSize, Vector2 parentPosition) {
        absoluteSize = new Vector2(
            (parentSize.X * Size.X.Scale) + Size.X.Offset,
            (parentSize.Y * Size.Y.Scale) + Size.Y.Offset
        );
        absolutePosition = Vector2.Add(new Vector2(
            (parentSize.X * Position.X.Scale) + Position.X.Offset,
            (parentSize.Y * Position.Y.Scale) + Position.Y.Offset
        ), parentPosition);

        switch(Anchor) {
            case UiElementAnchor.TopLeft: break;
            case UiElementAnchor.TopMiddle: absolutePosition = Vector2.Subtract(absolutePosition, new Vector2(absoluteSize.X / 2f, 0f)); break;
            case UiElementAnchor.TopRight: absolutePosition = Vector2.Subtract(absolutePosition, new Vector2(absoluteSize.X, 0)); break;
            case UiElementAnchor.MiddleLeft: absolutePosition = Vector2.Subtract(absolutePosition, new Vector2(0f, absoluteSize.Y / 2f)); break;
            case UiElementAnchor.Center: absolutePosition = Vector2.Subtract(absolutePosition, new Vector2(absoluteSize.X / 2f, absoluteSize.Y / 2f)); break;
            case UiElementAnchor.MiddleRight: absolutePosition = Vector2.Subtract(absolutePosition, new Vector2(absoluteSize.X, absoluteSize.Y / 2f)); break;
            case UiElementAnchor.BottomLeft: absolutePosition = Vector2.Subtract(absolutePosition, new Vector2(0f, absoluteSize.Y)); break;
            case UiElementAnchor.BottomMiddle: absolutePosition = Vector2.Subtract(absolutePosition, new Vector2(absoluteSize.X / 2f, absoluteSize.Y)); break;
            case UiElementAnchor.BottomRight: absolutePosition = Vector2.Subtract(absolutePosition, new Vector2(absoluteSize.X, absoluteSize.Y)); break;
        }

        foreach (var element in Children) {
            element.UpdateAbsoluteValues(absoluteSize, absolutePosition);
        }
    }

    public virtual void Render() {
        if(!visible) return;
        foreach (var element in Children) element.Render();
    }

    public void SetAbsoluteValues(Vector2 position, Vector2 size) {
        absolutePosition = position;
        absoluteSize = size;
    }
}

public enum UiElementAnchor {
    TopLeft,
    TopMiddle,
    TopRight,
    MiddleLeft,
    Center,
    MiddleRight,
    BottomLeft,
    BottomMiddle,
    BottomRight,
}