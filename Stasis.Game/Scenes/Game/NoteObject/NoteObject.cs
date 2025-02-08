using System.Numerics;
using Raylib_cs;
using Stasis.Content.Beatmaps;
using Stasis.Game.Scenes.Game.Player;

namespace Stasis.Game.Scenes.Game.NoteObject;

public class NoteObject {
    public const float HIT_WINDOW = 0.055f;
    public const float AABB = 1.1375f;

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
        return (time - Note.Time) <= HIT_WINDOW * speed;
    }

    public bool IsVisible(float time, float speed, float approachTime, bool pushback) {
        if(Hit) return false;
        if(time > Note.Time && !pushback && GetZ(time, speed) > 0.1) {
            return false;
        }
        return CalculateTime(time, speed, approachTime) <= 1f && InHitWindow(time, speed);
    }

    public float CalculateTime(float time, float speed, float approachTime) {
        return (Note.Time - time) / speed / approachTime;
    }

    public bool IsHitting(Vector2 cursorPosition) {
        return Math.Abs(-cursorPosition.X - (X + (cursorPosition.X * Global.Settings.Camera.GridParallax / 50f))) <= AABB && Math.Abs(cursorPosition.Y - (Y + -(cursorPosition.Y * Global.Settings.Camera.GridParallax / 50f))) <= AABB;
    }
}