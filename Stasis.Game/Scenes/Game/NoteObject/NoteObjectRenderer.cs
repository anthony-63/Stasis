using System.Collections.Specialized;
using System.Numerics;
using Raylib_cs;
using Stasis.Content.Beatmaps;
using Stasis.Engine;
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

    public static float Linstep(float a, float b, float x) {
        if(a == b) return (x >= a) ? 1f : 0f;
        return Math.Clamp((x - a) / (b - a), 0, 1);
    }

    public void RenderNotes(Cursor cursor) {
        if(ToRender.Count < 1) return;


        foreach(var note in ToRender) {
            var color = note.Color;
            var alpha = 1f;

            var ar = Global.Settings.Note.ApproachDistance / Global.Settings.Note.ApproachTime;
            var aprSpd = ar / (Game.Music?.Speed ?? 0);
            var dist = aprSpd * (note.Time - (Game.Music?.Time ?? 0f));

            var fadeInStart = Global.Settings.Note.ApproachDistance;
            var fadeInEnd = Global.Settings.Note.ApproachDistance*(1f - Global.Settings.Note.FadeIn);
            
            var fadeIn = Math.Pow(Linstep(fadeInStart, fadeInEnd, dist), 1.3) * 1f;

            if(Global.Settings.Note.FadeIn > 0)
                alpha = Math.Min((float)fadeIn, alpha);
            if(Global.Settings.Note.HalfGhost) {
                var fadeOutStart = 12f / 50f * ar;
                var fadeOutEnd = 3f / 50f * ar;
                var fadeOutBase = 0.8f;
                var fadeOut = (1 - fadeOutBase + (Math.Pow(Linstep(fadeOutEnd, fadeOutStart, dist), 1.3f) * fadeOutBase)) * 1f;
                alpha = Math.Min((float)fadeOut, alpha);
            }

            color = Raylib.ColorAlpha(color, alpha);

            MultiMesh.AddInstance(
                note.GetTransform(Game.Music?.Time ?? 0f, Game.Music?.Speed ?? 1f, cursor),
                color
            );
        }

        MultiMesh.Render();
    }
}