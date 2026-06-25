using System.Numerics;
using Raylib_cs;

namespace Stasis.Engine.GFX;

public class Camera
{
    Camera3D RlCamera;

    public ref Vector3 Position
    {
        get
        {
            return ref RlCamera.Position;
        }
    }

    public ref Vector3 Target
    {
        get
        {
            return ref RlCamera.Target;
        }
    }

    public Camera(Vector3 position, float fov)
    {
        RlCamera = new Camera3D(
            position,
            new Vector3(0, 0, 0),
            new Vector3(0, 1, 0),
            fov,
            CameraProjection.Perspective
        );
    }

    public void Start()
    {
        Raylib.BeginMode3D(RlCamera);
    }

    public void End()
    {
        Raylib.EndMode3D();
    }
}