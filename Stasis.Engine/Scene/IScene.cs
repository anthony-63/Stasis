namespace Stasis.Engine.Scene;

public interface IScene {
    public void Update(Window window, double dt);
    public void Render(Window window);
}