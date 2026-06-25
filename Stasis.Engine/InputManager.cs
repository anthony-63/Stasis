using System.Numerics;
using Raylib_cs;

namespace Stasis.Engine;

public enum InputType
{
    PressedOnce,
    HoldingDown,
}
public delegate void InputHandler();

class Keybind
{
    public bool ShouldHandle = true;
    public required KeyboardKey[] BoundKeys;
    public required InputHandler Callback;
    public InputType Type;

    public void Update()
    {

        switch (Type)
        {
            case InputType.PressedOnce:
                {
                    foreach (KeyboardKey key in BoundKeys) ShouldHandle |= Raylib.IsKeyUp(key);

                    bool pressed = true;
                    foreach (KeyboardKey key in BoundKeys) pressed = pressed && Raylib.IsKeyDown(key);

                    if (!pressed) return;

                    if (!ShouldHandle) return;
                    ShouldHandle = false;

                    Callback();
                }
                break;
            case InputType.HoldingDown:
                {
                    bool pressed = false;
                    foreach (KeyboardKey key in BoundKeys)
                    {
                        pressed = pressed && Raylib.IsKeyDown(key);
                    }
                    if (!pressed) return;
                    Callback();
                }
                break;
        }
    }
}

public unsafe static class InputManager
{
    private static Vector2 LastMousePos = Vector2.Zero;
    public static Vector2 MouseDelta = Vector2.Zero;
    public static Vector2 MousePosition = Vector2.Zero;

    private static List<Keybind> BoundKeys = new();

    public static void HideCursor()
    {
        Raylib.DisableCursor();
    }

    public static void ShowCursor()
    {
        Raylib.EnableCursor();
    }

    public static void BindKey(KeyboardKey[] keys, InputType type, InputHandler callback)
    {
        BoundKeys.Add(new Keybind
        {
            BoundKeys = keys,
            Callback = callback,
            ShouldHandle = true,
            Type = type,
        });
    }

    public static void Update()
    {
        Vector2 mpos = Raylib.GetMousePosition();
        MousePosition = new Vector2(mpos.X, mpos.Y);
        MouseDelta = Vector2.Subtract(MousePosition, LastMousePos);
        LastMousePos = MousePosition;

        foreach (var bind in BoundKeys)
        {
            bind.Update();
        }
    }
}