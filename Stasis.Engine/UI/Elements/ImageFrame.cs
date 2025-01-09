using System.Data;
using System.Numerics;
using Raylib_cs;

namespace Stasis.Engine.UI.Elements;

public class ImageFrame : UiElement {
    public string ImagePath {
        set {
            Image img = Raylib.LoadImage(value);
            texture = Raylib.LoadTextureFromImage(img);            
            Raylib.UnloadImage(img);
        }
    }

    private Texture2D texture;
    
    public Color BorderColor = Color.Gray;
    public float BorderWidth = 0f;

    public override void UpdateAbsoluteValues(Vector2 parentSize, Vector2 parentPosition) {
        base.UpdateAbsoluteValues(parentSize, parentPosition);
    }

    public override void Render() {
        if(BorderWidth > 0) {
            Raylib.DrawRectanglePro(new Rectangle {
                X = AbsolutePosition.X - BorderWidth,
                Y = AbsolutePosition.Y - BorderWidth,
                Width = AbsoluteSize.X + BorderWidth * 2,
                Height = AbsoluteSize.Y + BorderWidth * 2,
            }, Vector2.Zero, 0, BorderColor);
        }

        Raylib.DrawTexturePro(texture, new Rectangle {
            X = 0, Y = 0,
            Width = texture.Width,
            Height = texture.Height
        }, new Rectangle {
            X = AbsolutePosition.X,
            Y = AbsolutePosition.Y,
            Width = AbsoluteSize.X,
            Height = AbsoluteSize.Y,
        }, Vector2.Zero, 0, Color.White);

        base.Render();

    }
}
