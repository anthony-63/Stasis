namespace Stasis.Engine.Scene;

public interface IScene {
    public Window? Window { get; set; }

    public void Update(double dt);
    public void Render();
}