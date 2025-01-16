using System.Numerics;
using System.Runtime.Intrinsics;
using Raylib_cs;
using Stasis.Content.Beatmaps;
using Stasis.Engine;
using Stasis.Engine.Scene;
using Stasis.Engine.UI;
using Stasis.Engine.UI.Elements;
using Stasis.Game.Scenes.Game;
using Stasis.Game.Scenes.Loading;
using Stasis.Game.Scenes.Menu;
using TinyTween;

namespace Stasis.Game.Scenes.MapInfo;

public class MapInfoScene : Scene {
    public UiRoot Root = new();
    Frame MainFrame;
    Button BackButton;
    Button PlayButton;

    Frame Cover;

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

    Label MakeInfoLabel(string name, string? value, float x, float y) {
        var Title = new Label() {
            Text = name + ": ",
            TextColor = Color.SkyBlue,
            Position = new UDim2(0.14f, 0, y, 0),
            AlignmentX = TextAlignX.Left,
            Size = new UDim2(1, 0, 0, 0),
            FontSize = 24,
            Font = Global.UIFont,
        };
        Title.Children.Add(new Label() {
            Position = new UDim2(0, x, 0, 0),
            Text = value ?? "None",
            AlignmentX = TextAlignX.Left,
            Size = new UDim2(1, 0, 0, 0),
            FontSize = 24,
            Font = Global.UIFont,
        });

        return Title;
    }

    public MapInfoScene() {
        MainFrame = new Frame() {
            Size = new UDim2(1f, 0, 1, 0),
            Color = new Color(12, 12, 12, 255),
        };

        Frame buttonFrame = new Frame() {
            Size = new UDim2(1, 0, 1, 0),
            Color = new Color(60, 60, 60, 255),
            BorderWidth = 2,
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
                FontSize = 24,
                Font = Global.UIFont,
            },
            NormalFrame = buttonFrame,
            HoveringFrame = buttonFrame,
            DisabledFrame = buttonFrame,
            PressedFrame = buttonFrame,
        };

        PlayButton = new Button() {
            Size = UDim2.ScaleByPixels(100, 50),
            Position = new UDim2(0.95f, 0, 0.95f, 0),
            Anchor = UiElementAnchor.Center,
            Label = new Label() {
                Text = "Play",
                Size = new UDim2(1, 0, 1, 0),
                AlignmentX = TextAlignX.Center,
                // AlignmentY = TextAlignY.Middle,
                FontSize = 24,
                Font = Global.UIFont,
            },
            NormalFrame = buttonFrame,
            HoveringFrame = buttonFrame,
            DisabledFrame = buttonFrame,
            PressedFrame = buttonFrame,
        };
        
        BackButton.PressedOnce += GoToMenu;
        PlayButton.PressedOnce += PlayMap;

        Cover = MakeCoverFrame();
        MainFrame.Children.Add(Cover);
        
        var toIncrease = 0.035f;
        var initial = 0.027f;
        var mapLength = TimeSpan.FromSeconds(Global.SelectedMap?.Difficulties[0].Notes.Last().Time ?? 0);
        var mapLengthString = String.Format("{0:D1}:{1:D2}", mapLength.Minutes, mapLength.Seconds);

        MainFrame.Children.Add(MakeInfoLabel("Title", Global.SelectedMap?.Title, 50, initial + toIncrease * 0));
        MainFrame.Children.Add(MakeInfoLabel("Mapper", String.Join(" & ", Global.SelectedMap?.Mappers.ToArray() ?? ["None"]), 75, initial + toIncrease * 1));
        MainFrame.Children.Add(MakeInfoLabel("Length", mapLengthString, 75, initial + toIncrease * 2));
        MainFrame.Children.Add(MakeInfoLabel("Note Count", Global.SelectedMap?.Difficulties[0].Notes.Length.ToString(), 110, initial + toIncrease * 3));

        Root.Children.Add(MainFrame);
        Root.Children.Add(BackButton);
        Root.Children.Add(PlayButton);
    }

    private void GoToMenu() {
        Window?.SceneHandler.RemoveSceneByType<MapInfoScene>();
        Window?.SceneHandler.AddScene(Global.LoadedMenu ?? new MenuScene());
    }

    private void PlayMap() {
        Window?.SceneHandler.RemoveSceneByType<MapInfoScene>();
        Window?.SceneHandler.AddScene(new GameScene());
    }

    public override void Render() {
        Root.Render(Raylib.GetRenderWidth(), Raylib.GetRenderHeight());
        Raylib.DrawFPS(10, 10);
    }

    public override void Update(double dt) {
        Root.Update(dt);
    }
}