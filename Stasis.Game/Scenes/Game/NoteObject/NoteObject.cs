using System.Numerics;
using Raylib_cs;
using Stasis.Content.Beatmaps;
using Stasis.Game.Scenes.Game.Player;

namespace Stasis.Game.Scenes.Game.NoteObject;

public class NoteObject {
    public const float HitWindow = 0.055f;
    public const float AABB = (1.75f + 0.525f) / 2.0f;

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

    public Matrix4x4 GetTransform(float Time, Cursor cursor) {
        return Matrix4x4.Transpose(Matrix4x4.CreateTranslation(
            new Vector3(X + (cursor.ClampedPosition.X * Global.Settings.Camera.GridParallax / 50f), Y + -(cursor.ClampedPosition.Y * Global.Settings.Camera.GridParallax / 50f), GetZ(Time))
        ));
    }

    public float GetZ(float Time) {
        return -(CalculateTime(Time, Global.Settings.Note.ApproachTime) * Global.Settings.Note.ApproachDistance);
    }

    public bool InHitWindow(float Time, float Speed) {
        return (Time - Note.Time) <= HitWindow * Speed;
    }

    public bool IsVisible(float Time, float Speed, float ApproachTime, bool Pushback) {
        if(Hit) return false;
        if(Time > Note.Time && !Pushback && GetZ(Time) > 0.2) return false;
        return CalculateTime(Time, ApproachTime) <= 1f && InHitWindow(Time, Speed);
    }

    public float CalculateTime(float Time, float ApproachTime) {
        return (Note.Time - Time) / ApproachTime;
    }

    public bool IsHitting(Vector2 cursorPosition) {
        return Math.Abs(-cursorPosition.X - X) <= AABB && Math.Abs(cursorPosition.Y - Y) <= AABB;
    }
}