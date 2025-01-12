using System.Numerics;

using Stasis.Engine.UI.Elements;

namespace Stasis.Engine.UI;

public class UiRoot {
    public List<IUiElement> Children = new();
    
    public virtual void Render(int width, int height) {
        var parentSize = new Vector2(width, height);
        var parentPosition = Vector2.Zero;
        Parallel.ForEach(Children, elem => {
            elem.UpdateAbsoluteValues(parentSize, parentPosition);
        });
        foreach(var element in Children) {
            element.Render();
        }
    }
    public virtual void Update(double dt) {
        foreach (var element in Children) {
            element.Update(dt);
        }
    }
}