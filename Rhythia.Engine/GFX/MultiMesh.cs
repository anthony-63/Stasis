using System.Numerics;
using System.Reflection.Metadata;
using Raylib_cs;

namespace Rhythia.Engine.GFX;

struct MeshInstance {
    public Material Material;
    public Matrix4x4 Transform;
}

public class MultiMesh {
    public Mesh Mesh;

    MeshInstance[] Instances = [];
    public int InstanceCount => Instances.Length;
    int Index = 0;

    public MultiMesh(string meshPath) {
        var model = Raylib.LoadModel(meshPath);
        Instances = new MeshInstance[4];
        unsafe {
            Mesh = model.Meshes[0];
        }
    }

    public void AddInstance(Matrix4x4 transform, Material material) {
        if(Index + 1 >= Instances.Length) {
            Array.Resize(ref Instances, Instances.Length * 2);
        }
        Instances[Index++] = new MeshInstance {
            Material = material,
            Transform = transform,
        };
    }

    public void Render() {
        for(int i = 0; i < Index; i++) {
            Raylib.DrawMesh(Mesh, Instances[i].Material, Instances[i].Transform);
        }
        if(Instances.Length - Index > Instances.Length / 2) {
            Array.Resize(ref Instances, Instances.Length / 3);
        }
        Array.Clear(Instances);
        Index = 0;
    }
}