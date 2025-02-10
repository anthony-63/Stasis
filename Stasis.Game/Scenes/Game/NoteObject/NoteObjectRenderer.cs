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
        MultiMesh = new MultiMesh(Global.GetAsset("Assets/Game/Mesh.obj"), CaluclateMaxInstances());
    }

    public static int CaluclateMaxInstances() {
        var maxVisibleNotes = 0;
        var visibleNotes = 1;
        var earliestNote = 0;

        var notes = Global.SelectedMap?.Difficulties[0].Notes ?? [];

        for(int i = 1; i < notes.Length; i++) {
            var noteTime = notes[i].Time;
            for(int j = earliestNote; j < i; j++) {
                var timeDiff = noteTime - notes[j].Time;
                if(timeDiff > Global.Settings.Note.ApproachTime + 0.75) {
                    earliestNote++;
                    visibleNotes--;
                } else break;
            }
            visibleNotes++;
            maxVisibleNotes = Math.Max(visibleNotes, maxVisibleNotes);
        }

        return maxVisibleNotes;
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