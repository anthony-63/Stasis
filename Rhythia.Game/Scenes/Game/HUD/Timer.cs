using System.Globalization;
using System.Numerics;
using Raylib_cs;

namespace Rhythia.Game.Scenes.Game.HUD;

public class Timer {
    public float EndTime = 1f;

    static readonly float Height = 8;
    static readonly float BottomPadding = 5;
    static readonly float SidePadding = 5;

    float PerecentDone = 0f;

    public void Render() {
        Raylib.DrawRectanglePro(new Rectangle {
            X = SidePadding, Y = Raylib.GetRenderHeight() - Height - BottomPadding,
            Height = Height,
            Width = Raylib.GetRenderWidth() - SidePadding * 2f,
        }, Vector2.Zero, 0, Raylib.ColorAlpha(Color.DarkGray, 0.5f));
        Raylib.DrawRectanglePro(new Rectangle {
            X = SidePadding, Y = Raylib.GetRenderHeight() - Height - BottomPadding,
            Height = Height,
            Width = PerecentDone * (Raylib.GetRenderWidth() - SidePadding * 2f),
        }, Vector2.Zero, 0, Color.SkyBlue);
    }

    public void Update(float currentTime) {
        PerecentDone = (currentTime == 0 ? 1f : currentTime) / EndTime;
    }
}