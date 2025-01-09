using System.Numerics;
using Raylib_cs;

namespace Stasis.Engine.UI.Elements;

public class Label : UiElement {
    public string Text = "Lorem ipsum";

    private List<Tuple<string, Vector2>> lines = new();
    private float lineHeight = 0;
    public float LineHeight => lineHeight;

    public Color TextColor = Color.White;

    public Font Font = Raylib.GetFontDefault();
    public int FontSize = 18;
    public int FontSpacing = 1;

    public TextAlignX AlignmentX = TextAlignX.Center;
    public TextAlignY AlignmentY = TextAlignY.Middle;

    public int LineSpacing = 4;
    public bool TextWrapped = false;

    private Vector2 absoluteTextSize = Vector2.Zero;
    public Vector2 AbsoluteTextSize => absoluteTextSize;

    public override void UpdateAbsoluteValues(Vector2 parentSize, Vector2 parentPosition) {
        absoluteTextSize = Vector2.Zero;
        lineHeight = FontSize;
        lines.Clear();
        var line = "";
        var lineSize = Vector2.Zero;
        foreach (char character in Text) {
            var newLine = character == '\n';
            var keepCharacter = TextWrapped && !newLine;
            if (keepCharacter) {
                var nextLineSize = Raylib.MeasureTextEx(Font, line + character, FontSize, FontSpacing);
                newLine = nextLineSize.X + FontSpacing >= AbsoluteSize.X;
            }
            if (newLine) {
                absoluteTextSize.X = Math.Max(absoluteTextSize.X, lineSize.X);
                lineHeight = Math.Max(lineHeight, lineSize.Y);
                lines.Add(new(line, lineSize));
                line = keepCharacter ? $"{character}" : "";
                lineSize = Vector2.Zero;
                continue;
            }
            line += character;
            lineSize = Raylib.MeasureTextEx(Font, line, FontSize, FontSpacing);
        }
        absoluteTextSize.X = Math.Max(absoluteTextSize.X, lineSize.X);
        lineHeight = Math.Max(lineHeight, lineSize.Y);
        lines.Add(new(line, lineSize));
        absoluteTextSize.Y = (lineHeight * lines.Count) + (LineSpacing * (lines.Count - 1));
        base.UpdateAbsoluteValues(parentSize, parentPosition);
    }

    public override void Render() {
        var textOrigin = Vector2.Zero;
        if (AlignmentX == TextAlignX.Center) textOrigin.X = (AbsoluteSize.X - absoluteTextSize.X) / 2;
        if (AlignmentX == TextAlignX.Right) textOrigin.X = AbsoluteSize.X - absoluteTextSize.X;
        if (AlignmentY == TextAlignY.Middle) textOrigin.Y = (AbsoluteSize.Y - absoluteTextSize.Y) / 2;
        if (AlignmentY == TextAlignY.Bottom) textOrigin.Y = AbsoluteSize.Y - absoluteTextSize.Y;
        for (int i = 0; i < lines.Count; i++) {
            var tuple = lines[i];
            var text = tuple.Item1;
            var lineSize = tuple.Item2;
            var lineOffsetY = i * (lineHeight + LineSpacing);
            var textOffset = Vector2.UnitY * lineOffsetY;
            if (AlignmentX == TextAlignX.Center) textOffset.X = (absoluteTextSize.X - lineSize.X) / 2;
            if (AlignmentX == TextAlignX.Right) textOffset.X = absoluteTextSize.X - lineSize.X;
            Raylib.DrawTextEx(Font, text, AbsolutePosition + textOrigin + textOffset, FontSize, FontSpacing, TextColor);
        }
        base.Render();
    }
}

public enum TextAlignX {
    Left,
    Center,
    Right
}
public enum TextAlignY {
    Top,
    Middle,
    Bottom
}