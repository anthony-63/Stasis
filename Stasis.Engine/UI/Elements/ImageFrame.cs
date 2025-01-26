using System.Data;
using System.Numerics;
using Raylib_cs;

namespace Stasis.Engine.UI.Elements;

public class ImageFrame : UiElement {
    public string ImagePath {
        set {
            EnqueueLoadFromFile(value);
        }
    }

    public byte[] ImageData {
        set {
            EnqueueLoadFromMemory(value);
        }
    }

    private Texture2D? texture = null;
    private Image? ImageQueue = null;
    
    public Color BorderColor = Color.Gray;
    public float BorderWidth = 0f;
    public float Roundness = 0f;

    private void EnqueueLoadFromMemory(byte[] bytes) {
        new Thread(() => {
            ImageQueue = Raylib.LoadImageFromMemory(".png", bytes);
        }).Start();
    }

    private void EnqueueLoadFromFile(string name) {
        new Thread(() => {
            ImageQueue = Raylib.LoadImage(name);
        }).Start();
    }

    public override void UpdateAbsoluteValues(Vector2 parentSize, Vector2 parentPosition) {
        if(!Visible) return;
        base.UpdateAbsoluteValues(parentSize, parentPosition);
    }

    public override void Render() {
        if(!Visible) return;
        if(ImageQueue is Image img) {
            texture = Raylib.LoadTextureFromImage(img);
            Raylib.UnloadImage(img);
            ImageQueue = null;
        }
        if(texture is Texture2D tex) {
            if(BorderWidth > 0) {
                if(Roundness > 0) {
                    Raylib.DrawRectangleRounded(new Rectangle {
                        X = AbsolutePosition.X - BorderWidth,
                        Y = AbsolutePosition.Y - BorderWidth,
                        Width = AbsoluteSize.X + BorderWidth * 2,
                        Height = AbsoluteSize.Y + BorderWidth * 2,
                    }, Roundness, 16, BorderColor);
                } else {
                    Raylib.DrawRectangleRec(new Rectangle {
                        X = AbsolutePosition.X - BorderWidth,
                        Y = AbsolutePosition.Y - BorderWidth,
                        Width = AbsoluteSize.X + BorderWidth * 2,
                        Height = AbsoluteSize.Y + BorderWidth * 2,
                    }, BorderColor);
                }
                Raylib.DrawRectangleRounded(new Rectangle {
                    X = AbsolutePosition.X - BorderWidth,
                    Y = AbsolutePosition.Y - BorderWidth,
                    Width = AbsoluteSize.X + BorderWidth * 2,
                    Height = AbsoluteSize.Y + BorderWidth * 2,
                }, Roundness, 8, BorderColor);
            }
            Raylib.DrawTexturePro(tex, new Rectangle {
                X = 0, Y = 0,
                Width = tex.Width,
                Height = tex.Height
            }, new Rectangle {
                X = AbsolutePosition.X,
                Y = AbsolutePosition.Y,
                Width = AbsoluteSize.X,
                Height = AbsoluteSize.Y,
            }, Vector2.Zero, 0, Color.White);
        }

        base.Render();

    }
}
