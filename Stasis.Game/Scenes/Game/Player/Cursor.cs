using System.Numerics;
using Microsoft.VisualBasic;
using Raylib_cs;
using Stasis.Content.Settings;
using Stasis.Engine;
using Stasis.Engine.GFX;

namespace Stasis.Game.Scenes.Game.Player;

public class Cursor {

    private static float CLAMP_SINGLE = (6.0f - 0.525f) / 2.0f;
    private Vector2 CLAMP = new Vector2(CLAMP_SINGLE, CLAMP_SINGLE);

    public Sprite Sprite;

    public Vector2 Position = Vector2.Zero;

    public Vector2 ClampedPosition = Vector2.Zero;

    public Cursor(Vector3 initialPosition, Vector3 rotation, Vector2 scale, string texPath) {
        Sprite = Sprite.MakePlane(initialPosition, rotation, scale, texPath);
    }

    public void Render() {
        Sprite.Render();
    }

    public void ApplyParallax(ref Camera camera, ref Sprite grid) {
        grid.Position.X = -ClampedPosition.X * (Global.Settings.Camera.GridParallax / 50f);
        grid.Position.Z = -ClampedPosition.Y * (Global.Settings.Camera.GridParallax / 50f);

        camera.Position.X = -ClampedPosition.X * (Global.Settings.Camera.CameraParallax / 50f);
        camera.Position.Y = ClampedPosition.Y * (Global.Settings.Camera.CameraParallax / 50f);
        camera.Target.X = -ClampedPosition.X * (Global.Settings.Camera.CameraParallax / 50f);
        camera.Target.Y = ClampedPosition.Y * (Global.Settings.Camera.CameraParallax / 50f);
    }

    public void ProcessInput() {
        var sensFactor = Global.Settings.Cursor.Sensitivity / 50f;
        var delta = Vector2.One * (InputManager.MouseDelta * sensFactor);
        Position -= delta;

        var gridParallax = Vector2.One * (Global.Settings.Camera.GridParallax / 50f);

        var posClamp = CLAMP - (Position * gridParallax);
        var negClamp = -(CLAMP + (Position * gridParallax));

        ClampedPosition = Vector2.Clamp(Position, negClamp, posClamp);
        if(Global.Settings.Cursor.Clamped) Position = ClampedPosition;
    
        Sprite.Position = new Vector3(Position.X, 0f, Position.Y);
    }
}