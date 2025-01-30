using System.Numerics;
using Raylib_cs;
using Stasis.Content.Beatmaps;
using Stasis.Engine.GFX;
using Stasis.Game.Scenes.Game.Player;

namespace Stasis.Game.Scenes.Game.NoteObject;

public class NoteObjectRenderer {
    GameScene Game;

    public List<NoteObject> ToRender = [];

    public MultiMesh MultiMesh;

    public NoteObjectRenderer(GameScene game) {
        Game = game;
        MultiMesh = new MultiMesh("Assets/Game/Mesh.obj");
    }

    public void RenderNotes(Cursor cursor) {
        if(ToRender.Count < 1) return;

        foreach(var note in ToRender) {
            MultiMesh.AddInstance(
                note.GetTransform(Game.Music?.Time ?? 0f, Game.Music?.Speed ?? 1f, cursor), 
                note.Color
            );
        }

        MultiMesh.Render();
    }
}