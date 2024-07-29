using System.Numerics;
using Raylib_cs;
using Rhythia.Engine.GFX;

namespace Rhythia.Game.Scenes.Game.NoteObject;

public class NoteObjectRenderer {
    GameScene Game;

    public List<NoteObject> ToRender = [];

    public MultiMesh MultiMesh;

    public Material[] ColorMaterials;

    public NoteObjectRenderer(GameScene game) {
        Game = game;
        MultiMesh = new MultiMesh("Assets/Game/Mesh.obj");
        ColorMaterials = new Material[Global.Colors.Length];
        for(int i = 0; i < ColorMaterials.Length; i++)
            ColorMaterials[i] = ColoredMaterialGenerator.GetColoredMaterial(Global.Colors[i]);
    }

    public void RenderNotesSingle() {
        if(ToRender.Count < 1) return;

        foreach(var note in ToRender) {
            var noteTime = note.CalculateTime(Game.Music?.Time ?? 0, Global.Settings.Note.ApproachTime);
            var noteDist = noteTime * Global.Settings.Note.ApproachDistance;

            var transform = Matrix4x4.Transpose(Matrix4x4.CreateTranslation(
                new Vector3(note.X, note.Y, -noteDist)
            ));

            var mat = ColorMaterials[note.Index % ColorMaterials.Length];

            MultiMesh.AddInstance(transform, mat);
        }

        MultiMesh.Render();
    }
}