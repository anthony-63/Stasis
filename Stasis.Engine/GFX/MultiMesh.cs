using System.ComponentModel;
using System.Numerics;
using System.Reflection.Metadata;
using Raylib_cs;

namespace Stasis.Engine.GFX;

public class MultiMesh {
    const string _vs_shader = @"
#version 430

in vec3 vertexPosition;
in mat4 instanceTransform;

uniform mat4 mvp;
out vec4 fragColor;

void main() {
    mat4 new_transform = instanceTransform;
    float color = new_transform[0].x;
    float alpha = new_transform[1].y;
    float r = float((int(color) & 0xff0000) >> 16) / 255.0;
    float g = float((int(color) & 0x00ff00) >> 8) / 255.0;
    float b = float((int(color) & 0x0000ff)) / 255.0;
    fragColor = vec4(r, g, b, alpha);
    new_transform[0].x = 1.0;
    new_transform[1].y = 1.0;
    new_transform[2].z = 1.0;
    new_transform[3].w = 1.0;

    gl_Position = mvp*new_transform*vec4(vertexPosition, 1.0);
}
";
    const string _fs_shader = @"
#version 430

in vec4 fragColor;
out vec4 finalColor;

void main() {
    finalColor = fragColor;
}
";

    public Mesh Mesh;

    Matrix4x4[] Instances = [];
    Material Material;

    public int InstanceCount => Instances.Length;
    int Index = 0;

    public MultiMesh(string meshPath) {
        var model = Raylib.LoadModel(meshPath);
        Instances = new Matrix4x4[4];
        Material = Raylib.LoadMaterialDefault();

        Shader shader = Raylib.LoadShaderFromMemory(_vs_shader, _fs_shader);
        unsafe {
            shader.Locs[6] = Raylib.GetShaderLocation(shader, "mvp");
            shader.Locs[9] = Raylib.GetShaderLocationAttrib(shader, "instanceTransform");
        }

        Material.Shader = shader;

        unsafe {
            Material.Maps[0].Color = Color.White;
            Mesh = model.Meshes[0];
        }
    }

    public void AddInstance(Matrix4x4 transform, Color color) {
        if(Index + 1 >= Instances.Length) {
            Array.Resize(ref Instances, Instances.Length * 2);
        }

        float final = (color.R << 16) | (color.G << 8) | color.B;
        Instances[Index++] = transform * Matrix4x4.CreateScale(final, color.A / 255f, 1f);
    }

    public void Render() {
        Raylib.DrawMeshInstanced(Mesh, Material, Instances, Index);
        if(Instances.Length - Index > Instances.Length / 2) {
            Array.Resize(ref Instances, Instances.Length / 3);
        }
        
        Array.Clear(Instances);
        Index = 0;
    }
}