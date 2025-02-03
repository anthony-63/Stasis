using System.Numerics;
using Raylib_cs;
using Stasis.Content.Beatmaps;
using Stasis.Game.Scenes.Game.Player;

namespace Stasis.Game.Scenes.Game.NoteObject;

public class NoteObject {
    public static float HitWindow = 0.055f;
    public static float AABB = (1.75f + 0.525f) / 2.0f;

    public Note Note;

    public bool Hit = false;
    public bool Processed = false;
    public int Index = -1;
    public Color Color;

    public float Time => Note.Time;
    public float X => Note.X * 2f;
    public float Y => Note.Y * 2f;

    public NoteObject(Note note, int index, Color color) {
        Note = note;
        Index = index;
        Color = color;
    }

    public Matrix4x4 GetTransform(float time, float speed, Cursor cursor) {
        return Matrix4x4.Transpose(Matrix4x4.CreateTranslation(
            new Vector3(X + (cursor.ClampedPosition.X * Global.Settings.Camera.GridParallax / 50f), Y + -(cursor.ClampedPosition.Y * Global.Settings.Camera.GridParallax / 50f), GetZ(time, speed))
        ));
    }

    public float GetZ(float time, float speed) {
        return -(CalculateTime(time, speed, Global.Settings.Note.ApproachTime) * Global.Settings.Note.ApproachDistance);
    }

    public bool InHitWindow(float time, float speed) {
        return (time - Note.Time) <= HitWindow * speed;
    }

    public bool IsVisible(float time, float speed, float approachTime, bool pushback) {
        if(Hit) return false;
        if(Time > Note.Time && !pushback && GetZ(time, speed) > 0.2) return false;
        return CalculateTime(time, speed, approachTime) <= 1f && InHitWindow(time, speed);
    }

    public float CalculateTime(float time, float speed, float approachTime) {
        return (Note.Time - time) / speed / approachTime;
    }

    public bool IsHitting(Vector2 cursorPosition) {
        return Math.Abs(-cursorPosition.X - (X + (cursorPosition.X * Global.Settings.Camera.GridParallax / 50f))) <= AABB && Math.Abs(cursorPosition.Y - (Y + -(cursorPosition.Y * Global.Settings.Camera.GridParallax / 50f))) <= AABB;
    }
}