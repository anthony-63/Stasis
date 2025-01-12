using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Raylib_cs;

namespace Stasis.Engine.UI.Elements;

public class LabelTextWrapper {
    public static void DrawTextBoxed(
        Font font,
        string text,
        Rectangle rec,
        float fontSize,
        float spacing,
        bool wordWrap,
        Color tint
    ) {
        DrawTextBoxedSelectable(font, text, rec, fontSize, spacing, wordWrap, tint, 0, 0, Color.White, Color.White);
    }

    // Draw text using font inside rectangle limits with support for text selection
    public static unsafe void DrawTextBoxedSelectable(
        Font font,
        string text,
        Rectangle rec,
        float fontSize,
        float spacing,
        bool wordWrap,
        Color tint,
        int selectStart,
        int selectLength,
        Color selectTint,
        Color selectBackTint
    ) {
        int length = text.Length;
        float textOffsetY = 0;
        float textOffsetX = 0.0f;
        float scaleFactor = fontSize / (float)font.BaseSize;
        bool shouldMeasure = wordWrap;
        int startLine = -1;
        int endLine = -1;
        int lastk = -1;

        using var textNative = new Utf8Buffer(text);

        for (int i = 0, k = 0; i < length; i++, k++) {
            int codepointByteCount = 0;
            int codepoint = Raylib.GetCodepoint(&textNative.AsPointer()[i], &codepointByteCount);
            int index = Raylib.GetGlyphIndex(font, codepoint);

            if (codepoint == 0x3f) {
                codepointByteCount = 1;
            }

            i += codepointByteCount - 1;

            float glyphWidth = 0;
            if (codepoint != '\n') {
                glyphWidth = (font.Glyphs[index].AdvanceX == 0) ?
                    font.Recs[index].Width * scaleFactor :
                    font.Glyphs[index].AdvanceX * scaleFactor;

                if (i + 1 < length) {
                    glyphWidth = glyphWidth + spacing;
                }
            }

            if (shouldMeasure) {
                if ((codepoint == ' ') || (codepoint == '\t') || (codepoint == '\n')) {
                    endLine = i;
                }

                if ((textOffsetX + glyphWidth) > rec.Width) {
                    endLine = (endLine < 1) ? i : endLine;
                    if (i == endLine)
                        endLine -= codepointByteCount;
                    if ((startLine + codepointByteCount) == endLine)
                        endLine = (i - codepointByteCount);

                    shouldMeasure = !shouldMeasure;
                }
                else if ((i + 1) == length) {
                    endLine = i;
                    shouldMeasure = !shouldMeasure;
                }
                else if (codepoint == '\n') {
                    shouldMeasure = !shouldMeasure;
                }

                if (!shouldMeasure) {
                    textOffsetX = 0;
                    i = startLine;
                    glyphWidth = 0;

                    int tmp = lastk;
                    lastk = k - 1;
                    k = tmp;
                }
            } else {
                if (codepoint == '\n') {
                    if (!wordWrap) {
                        textOffsetY += (font.BaseSize + font.BaseSize / 2) * scaleFactor;
                        textOffsetX = 0;
                    }
                } else {
                    if (!wordWrap && ((textOffsetX + glyphWidth) > rec.Width)) {
                        textOffsetY += (font.BaseSize + font.BaseSize / 2) * scaleFactor;
                        textOffsetX = 0;
                    }

                    // When text overflows rectangle height limit, just stop drawing
                    if ((textOffsetY + font.BaseSize * scaleFactor) > rec.Height) break;

                    bool isGlyphSelected = false;
                    if ((selectStart >= 0) && (k >= selectStart) && (k < (selectStart + selectLength))) {
                        Raylib.DrawRectangleRec(
                            new Rectangle(
                                rec.X + textOffsetX - 1,
                                rec.Y + textOffsetY,
                                glyphWidth,
                                (float)font.BaseSize * scaleFactor
                            ),
                            selectBackTint
                        );
                        isGlyphSelected = true;
                    }

                    if ((codepoint != ' ') && (codepoint != '\t')) {
                        Raylib.DrawTextCodepoint(
                            font,
                            codepoint,
                            new Vector2(rec.X + textOffsetX, rec.Y + textOffsetY),
                            fontSize,
                            isGlyphSelected ? selectTint : tint
                        );
                    }
                }

                if (wordWrap && (i == endLine)) {
                    textOffsetY += (font.BaseSize + font.BaseSize / 2) * scaleFactor;
                    textOffsetX = 0;
                    startLine = endLine;
                    endLine = -1;
                    glyphWidth = 0;
                    selectStart += lastk - k;
                    k = lastk;

                    shouldMeasure = !shouldMeasure;
                }
            }

            if ((textOffsetX != 0) || (codepoint != ' '))
                textOffsetX += glyphWidth;
        }
    }
}

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
        // if(!Visible) return;
        // absoluteTextSize = Vector2.Zero;
        // lineHeight = FontSize;
        // lines.Clear();
        // var line = "";
        // var lineSize = Vector2.Zero;
        // foreach (char character in Text) {
        //     var newLine = character == '\n';
        //     var keepCharacter = TextWrapped && !newLine;
        //     if (keepCharacter) {
        //         var nextLineSize = Raylib.MeasureTextEx(Font, line + character, FontSize, FontSpacing);
        //         newLine = nextLineSize.X + FontSpacing >= AbsoluteSize.X;
        //     }
        //     if (newLine) {
        //         absoluteTextSize.X = Math.Max(absoluteTextSize.X, lineSize.X);
        //         lineHeight = Math.Max(lineHeight, lineSize.Y);
        //         lines.Add(new(line, lineSize));
        //         line = keepCharacter ? $"{character}" : "";
        //         lineSize = Vector2.Zero;
        //         continue;
        //     }
        //     line += character;
        //     lineSize = Raylib.MeasureTextEx(Font, line, FontSize, FontSpacing);
        // }
        // absoluteTextSize.X = Math.Max(absoluteTextSize.X, lineSize.X);
        // lineHeight = Math.Max(lineHeight, lineSize.Y);
        // lines.Add(new(line, lineSize));
        // absoluteTextSize.Y = (lineHeight * lines.Count) + (LineSpacing * (lines.Count - 1));
        base.UpdateAbsoluteValues(parentSize, parentPosition);
    }

    public override void Render() {
        if(!Visible) return;
        // var textOrigin = Vector2.Zero;
        // if (AlignmentX == TextAlignX.Center) textOrigin.X = (AbsoluteSize.X - absoluteTextSize.X) / 2;
        // if (AlignmentX == TextAlignX.Right) textOrigin.X = AbsoluteSize.X - absoluteTextSize.X;
        // if (AlignmentY == TextAlignY.Middle) textOrigin.Y = (AbsoluteSize.Y - absoluteTextSize.Y) / 2;
        // if (AlignmentY == TextAlignY.Bottom) textOrigin.Y = AbsoluteSize.Y - absoluteTextSize.Y;
        // for (int i = 0; i < lines.Count; i++) {
        //     var tuple = lines[i];
        //     var text = tuple.Item1;
        //     var lineSize = tuple.Item2;
        //     var lineOffsetY = i * (lineHeight + LineSpacing);
        //     var textOffset = Vector2.UnitY * lineOffsetY;
        //     if (AlignmentX == TextAlignX.Center) textOffset.X = (absoluteTextSize.X - lineSize.X) / 2;
        //     if (AlignmentX == TextAlignX.Right) textOffset.X = absoluteTextSize.X - lineSize.X;
        //     Raylib.DrawTextEx(Font, text, AbsolutePosition + textOrigin + textOffset, FontSize, FontSpacing, TextColor);
        // }
        LabelTextWrapper.DrawTextBoxed(Font, Text, new Rectangle {
            X = AbsolutePosition.X,
            Y = AbsolutePosition.Y,
            Width = AbsoluteSize.X,
            Height = AbsoluteSize.Y,
        }, FontSize, FontSpacing, TextWrapped, Color.White);
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