using System.Numerics;
using System.Runtime.Intrinsics;
using Raylib_cs;
using Stasis.Content.Beatmaps;
using Stasis.Engine;
using Stasis.Engine.Scene;
using Stasis.Engine.UI;
using Stasis.Engine.UI.Elements;
using Stasis.Game.Scenes.Loading;
using Stasis.Game.Scenes.Menu;
using TinyTween;

namespace Stasis.Game.Scenes.MapInfo;

public class MapInfoScene : Scene {
    public UiRoot Root = new();
    Frame MainFrame;
    Button BackButton;

    Frame Cover;

    Font Font = Raylib.LoadFontEx("Assets/Game/font.ttf", 18, [], 0);

    Frame MakeCoverFrame() {
        Frame cover = new Frame() {
            Size = UDim2.ScaleByPixels(150, 150),
            Position = new UDim2(0.01f, 0, 0.01f, 0),
            Color = new Color(100, 100, 100, 100),
        };

        ImageFrame image = new ImageFrame() {
            Size = new UDim2(0.9f, 0, 0.9f, 0),
            Position = new UDim2(0.05f, 0, 0.05f, 0),
        };

        if(Global.SelectedMap?.Cover is null || Global.SelectedMap?.Cover.Length <= 0) {
            image.ImagePath = "Assets/Game/cat.png";
        } else {
            image.ImageData = Global.SelectedMap?.Cover ?? [];
        }

        cover.Children.Add(image);

        return cover;
    }

    public MapInfoScene() {
        MainFrame = new Frame() {
            Size = new UDim2(1f, 0, 1, 0),
            Color = new Color(12, 12, 12, 255),
        };

        Frame buttonFrame = new Frame() {
            Size = new UDim2(1, 0, 1, 0),
            Color = new Color(60, 60, 60, 255),
            BorderWidth = 1,
            Roundness = 0.1f,
        };

        BackButton = new Button() {
            Size = UDim2.ScaleByPixels(100, 50),
            Position = new UDim2(0.05f, 0, 0.95f, 0),
            Anchor = UiElementAnchor.Center,
            Label = new Label() {
                Text = "Back",
                Size = new UDim2(1, 0, 1, 0),
                AlignmentX = TextAlignX.Center,
                // AlignmentY = TextAlignY.Middle,
                Font = Font,
            },
            NormalFrame = buttonFrame,
            HoveringFrame = buttonFrame,
            DisabledFrame = buttonFrame,
            PressedFrame = buttonFrame,
        };
        
        BackButton.PressedOnce += GoToMenu;

        Cover = MakeCoverFrame();
        MainFrame.Children.Add(Cover);

        Root.Children.Add(MainFrame);
        Root.Children.Add(BackButton);
    }

    private void GoToMenu() {
        Window?.SceneHandler.RemoveSceneByType<MapInfoScene>();
        Window?.SceneHandler.AddScene(Global.LoadedMenu ?? new MenuScene());
    }

    public override void Render() {
        Root.Render(Raylib.GetRenderWidth(), Raylib.GetRenderHeight());
        Raylib.DrawFPS(10, 10);
    }

    public override void Update(double dt) {
        Root.Update(dt);
    }
}