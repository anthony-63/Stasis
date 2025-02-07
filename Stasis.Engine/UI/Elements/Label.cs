using System.IO.Compression;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using Raylib_cs;

namespace Stasis.Engine.UI.Elements;

public class LabelFont {
    public Font Font;
    public int Spacing = 1;

    Dictionary<char, float> fontSizes;
    float fontHeight;
    float scaleFactor;


    public LabelFont() : this(Raylib.GetFontDefault(), 10) {}

    public LabelFont(Font font, int size) {
        Font = font;

        fontSizes = [];
        fontHeight = font.BaseSize;
        scaleFactor = size/font.BaseSize;

        unsafe {
            for(int i = 0; i < font.GlyphCount; i++) {
                var glyph = font.Glyphs[i];
                fontSizes[(char)glyph.Value] = glyph.AdvanceX == 0 ? font.Recs[i].Width * scaleFactor : glyph.AdvanceX * scaleFactor;
            }
            if(!fontSizes.ContainsKey('?')) fontSizes['?'] = fontSizes['H'];
        }
    }

    private static Font loadFont(string path, int size) {
        return Raylib.LoadFontEx(path, size, [], 0);
    }

    public LabelFont(string path, int size) : this(loadFont(path, size), size) {}

    public Vector2 MeasureText(string text) {
        var size = new Vector2(0, fontHeight);
        int i = 0;
        foreach(var c in text) {
            if(fontSizes.ContainsKey(c)) size.X += fontSizes[c];
            else size.X += fontSizes['?'];
            if(i != 0) size.X += Spacing;
            i++;
        }
        return size;
    }
}

public class Label : UiElement {
    public string Text = "";

    public static List<Tuple<string, int, LabelFont>> LoadedFonts = new();

    public List<Tuple<char, int>> textIndexed = new();

    public List<Tuple<string, Vector2>> lines = new();

    private float lineHeight = 0;
    public float LineHeight => lineHeight;

    public bool OneLine = false;

    public Color TextColor = Color.White;

    private string fontPath = "";

    public string Font {
        set {
            if(LoadedFonts.Any((v) => v.Item1 == value && v.Item2 == FontSize)) {
                font = LoadedFonts.Find((v) => v.Item1 == value && v.Item2 == FontSize)?.Item3 ?? new LabelFont();
            } else {
                font = new LabelFont(value, FontSize);
                LoadedFonts.Add(new(value, FontSize, font));
            }
            fontPath = value;
        }
        get => fontPath;
    }

    private LabelFont font = new LabelFont();
    public int FontSize = 18;

    public TextAlignX AlignmentX = TextAlignX.Center;
    public TextAlignY AlignmentY = TextAlignY.Middle;

    public int LineSpacing = 4;
    public bool TextWrapped = false;

    public Vector2 TextOrigin = Vector2.Zero;

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
                    var nextLineSize = font.MeasureText(line + character);
                    newLine = nextLineSize.X >= AbsoluteSize.X;
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
                lineSize = font.MeasureText(line);
            }
            absoluteTextSize.X = Math.Max(absoluteTextSize.X, lineSize.X);
            lineHeight = Math.Max(lineHeight, lineSize.Y);
            lines.Add(new(line, lineSize));
            absoluteTextSize.Y = lineHeight * lines.Count;
        } else {
            var textSize = font.MeasureText(Text);
            absoluteTextSize = textSize;
            lines.Add(new(Text, textSize));
        }
        base.UpdateAbsoluteValues(parentSize, parentPosition);
    }

    public override void Render() {
        if(!Visible) return;
        TextOrigin = Vector2.Zero;
        if (AlignmentX == TextAlignX.Center) TextOrigin.X = (AbsoluteSize.X - absoluteTextSize.X) / 2;
        if (AlignmentX == TextAlignX.Right) TextOrigin.X = AbsoluteSize.X - absoluteTextSize.X;
        if (AlignmentY == TextAlignY.Middle) TextOrigin.Y = (AbsoluteSize.Y - absoluteTextSize.Y) / 2;
        if (AlignmentY == TextAlignY.Bottom) TextOrigin.Y = AbsoluteSize.Y - absoluteTextSize.Y;
        for (int i = 0; i < lines.Count; i++) {
            var tuple = lines[i];
            var text = tuple.Item1;
            var lineSize = tuple.Item2;
            var lineOffsetY = i * (lineHeight + LineSpacing);
            var textOffset = Vector2.UnitY * lineOffsetY;
            if (AlignmentX == TextAlignX.Center) textOffset.X = (absoluteTextSize.X - lineSize.X) / 2;
            if (AlignmentX == TextAlignX.Right) textOffset.X = absoluteTextSize.X - lineSize.X;
            Raylib.DrawTextEx(font.Font, text, AbsolutePosition + TextOrigin + textOffset, FontSize, font.Spacing, TextColor);
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