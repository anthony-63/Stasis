using System.Numerics;
using Raylib_cs;

namespace Stasis.Engine.GFX;

public class Sprite
{
    public Vector3 Position;
    public Vector3 Rotation;
    public Vector2 Size;
    public Mesh Mesh;
    public Material Material;

    public static Sprite MakePlane(Vector3 position, Vector3 rotation, Vector2 size, string texturePath)
    {
        Sprite sprite = new Sprite
        {
            Position = position,
            Rotation = rotation,
            Size = size,
        };

        Image img = Raylib.LoadImage(texturePath);
        Texture2D texture = Raylib.LoadTextureFromImage(img);

        Raylib.UnloadImage(img);

        Material material = Raylib.LoadMaterialDefault();

        Raylib.SetMaterialTexture(ref material, 0, texture);

        sprite.Mesh = Raylib.GenMeshPlane(size.X, size.Y, 1, 1);
        sprite.Material = material;

        return sprite;
    }

    public static Sprite MakePlane(Vector3 position, Vector3 rotation, Vector2 size, Texture2D texture)
    {
        Sprite sprite = new Sprite
        {
            Position = position,
            Rotation = rotation,
            Size = size,
        };
        Material material = Raylib.LoadMaterialDefault();

        Raylib.SetMaterialTexture(ref material, 0, texture);

        sprite.Mesh = Raylib.GenMeshPlane(size.X, size.Y, 1, 1);
        sprite.Material = material;

        return sprite;
    }

    public void SetTexture(Texture2D texture)
    {
        Raylib.SetMaterialTexture(ref Material, 0, texture);
    }

    public void Render()
    {
        var transform = Matrix4x4.CreateTranslation(Position);
        transform = Matrix4x4.Multiply(transform, Matrix4x4.CreateRotationX(Raylib.DEG2RAD * Rotation.X));
        transform = Matrix4x4.Multiply(transform, Matrix4x4.CreateRotationY(Raylib.DEG2RAD * Rotation.Y));
        transform = Matrix4x4.Multiply(transform, Matrix4x4.CreateRotationZ(Raylib.DEG2RAD * Rotation.Z));
        Raylib.DrawMesh(Mesh, Material, Matrix4x4.Transpose(transform));
    }
}