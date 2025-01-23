using System.Numerics;

namespace Stasis.Engine.UI.Elements;

public interface IUiElement {
    public Vector2 AbsoluteSize { get; }
    public Vector2 AbsolutePosition { get; }
    public bool Visible { get; set; }
    public UDim2 Size { get; set; }
    public UDim2 Position { get; set; }
    public void Update(double dt);
    public bool IsHovering();
    public void AddChild(IUiElement child);
    public void UpdateAbsoluteValues(Vector2 parentSize, Vector2 parentPosition);
    public void SetAbsoluteValues(Vector2 position, Vector2 size);
    public void Render();
}