using System.IO.Compression;
using System.Numerics;
using Raylib_cs;

namespace Stasis.Engine.UI.Elements;

public class Label : UiElement {
    public string Text = "";

    public static List<Tuple<string, int, Font>> LoadedFonts = new();

    public List<Tuple<char, int>> textIndexed = new();

    private List<Tuple<string, Vector2>> lines = new();

    private float lineHeight = 0;
    public float LineHeight => lineHeight;

    public bool OneLine = false;

    public Color TextColor = Color.White;

    private string fontPath = "";

    public string Font {
        set {
            if(LoadedFonts.Any((v) => v.Item1 == value && v.Item2 == FontSize)) {
                font = LoadedFonts.Find((v) => v.Item1 == value && v.Item2 == FontSize)?.Item3 ?? Raylib.GetFontDefault();
            } else {
                font = Raylib.LoadFontEx(value, FontSize, [], 0);
                LoadedFonts.Add(new(value, FontSize, font));
            }
            fontPath = value;
        }
        get => fontPath;
    }

    private Font font = Raylib.GetFontDefault();
    public int FontSize = 18;
    public int FontSpacing = 1;

    public TextAlignX AlignmentX = TextAlignX.Center;
    public TextAlignY AlignmentY = TextAlignY.Middle;

    public int LineSpacing = 4;
    public bool TextWrapped = false;

    private Vector2 absoluteTextSize = Vector2.Zero;
    public Vector2 AbsoluteTextSize => absoluteTextSize;

    public override void UpdateAbsoluteValues(Vector2 parentSize, Vector2 parentPosition) {
        if(!Visible) return;
        absoluteTextSize = Vector2.Zero;
        lineHeight = FontSize;
        lines.Clear();
        if(!OneLine) {
            var line = "";
            var lineSize = Vector2.Zero;
            foreach (char character in Text) {
                var newLine = character == '\n';
                var keepCharacter = TextWrapped && !newLine;
                if (keepCharacter) {
                    var nextLineSize = Raylib.MeasureTextEx(font, line + character, FontSize, FontSpacing);
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
                lineSize = Raylib.MeasureTextEx(font, line, FontSize, FontSpacing);
            }
            absoluteTextSize.X = Math.Max(absoluteTextSize.X, lineSize.X);
            lineHeight = Math.Max(lineHeight, lineSize.Y);
            lines.Add(new(line, lineSize));
            absoluteTextSize.Y = (lineHeight * lines.Count) + (LineSpacing * (lines.Count - 1));
        } else {
            lines.Add(new(Text, Raylib.MeasureTextEx(font, Text, FontSize, FontSpacing)));
        }
        base.UpdateAbsoluteValues(parentSize, parentPosition);
    }

    public override void Render() {
        if(!Visible) return;
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
            Raylib.DrawTextEx(font, text, AbsolutePosition + textOrigin + textOffset, FontSize, FontSpacing, TextColor);
        }
        base.Render();
    }

    public override void SetAbsoluteValues(Vector2 position, Vector2 size) {
        base.SetAbsoluteValues(position, size);
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