using System.Numerics;

using Stasis.Engine.UI.Elements;

namespace Stasis.Engine.UI;

public class UiRoot {
    public List<IUiElement> Children = new();

    public virtual void AddChild(IUiElement child) {
        Children.Add(child);
    }

    public virtual void Render(int width, int height) {
        var parentSize = new Vector2(width, height);
        var parentPosition = Vector2.Zero;
        foreach (var element in Children) {
            element.UpdateAbsoluteValues(parentSize, parentPosition);
            element.Render();
        }
    }
    
    public virtual void Update(double dt) {
        foreach (var element in Children) {
            element.Update(dt);
        }
    }
}