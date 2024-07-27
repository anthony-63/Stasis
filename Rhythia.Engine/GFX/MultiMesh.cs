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
    int MaxInstanceCount;
    int Index = 0;

    public MultiMesh(string meshPath, int maxInstanceCount) {
        var model = Raylib.LoadModel(meshPath);
        MaxInstanceCount = maxInstanceCount;
        Instances = new MeshInstance[MaxInstanceCount];
        unsafe {
            Mesh = model.Meshes[0];
        }
    }

    public void AddInstance(Matrix4x4 transform, Material material) {
        Instances[Index++] = new MeshInstance {
            Material = material,
            Transform = transform,
        };
    }

    public void Render() {
        for(int i = 0; i < Index; i++) {
            Raylib.DrawMesh(Mesh, Instances[i].Material, Instances[i].Transform);
        }
        Array.Clear(Instances);
        Index = 0;
    }
}