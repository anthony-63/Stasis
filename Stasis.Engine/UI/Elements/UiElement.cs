using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using Raylib_cs;

namespace Stasis.Engine.UI.Elements;

public class UiElement
{
    private Vector2 absoluteSize = Vector2.Zero;
    private Vector2 absolutePosition = Vector2.Zero;

    private bool visible = true;
    private bool culled = false;
    public bool IgnoreUpdate = false;

    public UiElementAnchor Anchor = UiElementAnchor.TopLeft;

    public Vector2 AbsoluteSize => absoluteSize;
    public Vector2 AbsolutePosition => absolutePosition;

    public bool Visible { get => visible; set => visible = value; }

    public UDim2 Size = UDim2.Zero;
    public UDim2 Position = UDim2.Zero;

    public List<UiElement> Children = new();

    public bool ClipContents = false;
    public Rectangle ScissorRect = new();

    public UiRoot Root = new();

    public virtual void Update(double dt)
    {
        if (culled || IgnoreUpdate) return;
        foreach (var element in Children) element.Update(dt);
    }

    public virtual bool IsHovering()
    {
        if (!visible || culled) return false;
        return Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), new Rectangle
        {
            X = AbsolutePosition.X,
            Y = AbsolutePosition.Y,
            Width = AbsoluteSize.X,
            Height = AbsoluteSize.Y,
        });
    }

    public virtual bool IsHovering(Vector2 position)
    {
        if (!visible || culled) return false;
        return Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), new Rectangle
        {
            X = position.X,
            Y = position.Y,
            Width = AbsoluteSize.X,
            Height = AbsoluteSize.Y,
        });
    }

    public virtual void UpdateAbsoluteValues(Vector2 parentSize, Vector2 parentPosition)
    {
        if (!visible) return;

        absoluteSize = new Vector2(
            (parentSize.X * Size.X.Scale) + Size.X.Offset,
            (parentSize.Y * Size.Y.Scale) + Size.Y.Offset
        );
        absolutePosition = Vector2.Add(new Vector2(
            (parentSize.X * Position.X.Scale) + Position.X.Offset,
            (parentSize.Y * Position.Y.Scale) + Position.Y.Offset
        ), parentPosition);

        switch (Anchor)
        {
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

        culled = (Raylib.GetRenderHeight() < absolutePosition.Y || absolutePosition.Y + absoluteSize.Y < 0) && this is not GridContainer && this is not ScrollContainer;

        if (culled) return;

        foreach (var element in Children)
        {
            element.UpdateAbsoluteValues(absoluteSize, absolutePosition);
        }
    }

    public virtual void Render()
    {
        if (!visible || culled) return;
        SetClipDim();
        if (ClipContents) Raylib.BeginScissorMode((int)ScissorRect.X, (int)ScissorRect.Y, (int)ScissorRect.Width, (int)ScissorRect.Height);
        foreach (var element in Children) element.Render();
        if (ClipContents) Raylib.EndScissorMode();
    }

    public virtual void SetAbsoluteValues(Vector2 position, Vector2 size)
    {
        absolutePosition = position;
        absoluteSize = size;
        foreach (var element in Children)
        {
            element.UpdateAbsoluteValues(absoluteSize, absolutePosition);
        }
    }

    public virtual void SetClipDim()
    {
        ScissorRect = new Rectangle()
        {
            X = AbsolutePosition.X,
            Y = AbsolutePosition.Y,
            Width = AbsoluteSize.X,
            Height = AbsoluteSize.Y
        };
    }

    public virtual void AddChild(UiElement child)
    {
        Children.Add(child);
    }
}

public enum UiElementAnchor
{
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