using Stasis.Engine;

namespace Stasis.Engine.Scene;
public class Scene : IScene
{
    private Window? _window;

    public Window? Window { get => _window; set => _window = value; }

    public virtual void Render() {}

    public virtual void Update(double dt) {}
}
