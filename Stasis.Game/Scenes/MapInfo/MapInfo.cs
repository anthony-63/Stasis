using Raylib_cs;
using Stasis.Engine;
using Stasis.Engine.Scene;
using Stasis.Engine.UI;
using Stasis.Engine.UI.Elements;
using Stasis.Game.Scenes.Game;
using Stasis.Game.Scenes.Loading;
using Stasis.Game.Scenes.Menu;

namespace Stasis.Game.Scenes.MapInfo;

public class MapInfoScene : Scene {
    public UiRoot Root = new();
    Frame MainFrame;
    Button BackButton;
    Button PlayButton;

    SpinBox SpeedMod;

    Frame Cover;

    Frame MakeCoverFrame() {
        Frame cover = new Frame() {
            Size = new UDim2(0.1171875f, 0, 0.20833333f, 0),
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

        cover.AddChild(image);

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
        Title.AddChild(new Label() {
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
            Size = UDim2.Fill,
            Color = new Color(60, 60, 60, 255),
            BorderWidth = 2,
            Roundness = 0.1f,
        };

        BackButton = new Button() {
            Size = new UDim2(0.078125f, 0, 0.06944445f, 0),
            Position = new UDim2(0.01f, 0, 0.95f, 0),
            Anchor = UiElementAnchor.MiddleLeft,
            Label = new Label() {
                Text = "Back",
                Size = UDim2.Fill,
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
            Size = new UDim2(0.078125f, 0, 0.06944445f, 0),
            Position = new UDim2(0.99f, 0, 0.95f, 0),
            Anchor = UiElementAnchor.MiddleRight,
            Label = new Label() {
                Text = "Play",
                Size = UDim2.Fill,
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

        SpeedMod = new SpinBox() {
            Value = Mods.Speed,
            Step = 0.01f,
            Position = new UDim2(0.01f, 88, 0.23f, 0),
            Size = new UDim2(0, 100, 0, 25),
            Format = "0.00",
            NormalFrame = new() {
                Color = Color.DarkGray,
                BorderWidth = 1f,
                BorderColor = Color.Gray,
                Roundness = 3f,
            },
            FocusedFrame = new() {
                Color = new Color(100, 100, 100, 255),
                BorderWidth = 1f,
                BorderColor = Color.Gray,
                Roundness = 3f,
            },
            Placeholder = new() {
                AlignmentX = TextAlignX.Left,
                AlignmentY = TextAlignY.Middle,
                Position = new UDim2(0.04f, 0, 0, 0),
                Size = new UDim2(0.96f, 0, 1, 0),
                FontSize = 24,
                Font = Global.UIFont,
                TextColor = Color.Gray,
                OneLine = true,
            },
            Text = new() {
                AlignmentX = TextAlignX.Left,
                AlignmentY = TextAlignY.Middle,
                Position = new UDim2(0.04f, 0, 0, 0),
                Size = new UDim2(0.96f, 0, 1, 0),
                FontSize = 18,
                Text = Mods.Speed.ToString(),
                Font = Global.UIFont,
                TextColor = Color.White,
                OneLine = true,
            },
        };

        var speedModLabel = new Label() {
            AlignmentX = TextAlignX.Right,
            AlignmentY = TextAlignY.Middle,
            Position = new UDim2(0f, -100f, 0, 0),
            Size = new UDim2(0.96f, 0, 1, 0),
            FontSize = 32,
            Text = "Speed: ",
            Font = Global.UIFont,
            TextColor = Color.White,
            OneLine = true,
        };

        SpeedMod.AddChild(speedModLabel);
        
        BackButton.PressedOnce += GoToMenu;
        PlayButton.PressedOnce += PlayMap;

        Cover = MakeCoverFrame();
        MainFrame.AddChild(Cover);
        
        var toIncrease = 0.035f;
        var initial = 0.027f;
        var mapLength = TimeSpan.FromSeconds(Global.SelectedMap?.Difficulties[0].Notes.Last().Time ?? 0);
        var mapLengthString = String.Format("{0:D1}:{1:D2}", mapLength.Minutes, mapLength.Seconds);

        MainFrame.AddChild(MakeInfoLabel("Title", Global.SelectedMap?.Title, 50, initial + toIncrease * 0));
        MainFrame.AddChild(MakeInfoLabel("Mapper", String.Join(" & ", Global.SelectedMap?.Mappers.ToArray() ?? ["None"]), 75, initial + toIncrease * 1));
        MainFrame.AddChild(MakeInfoLabel("Length", mapLengthString, 75, initial + toIncrease * 2));
        MainFrame.AddChild(MakeInfoLabel("Note Count", Global.SelectedMap?.Difficulties[0].Notes.Length.ToString(), 110, initial + toIncrease * 3));

        Root.AddChild(MainFrame);
        Root.AddChild(BackButton);
        Root.AddChild(PlayButton);
        Root.AddChild(SpeedMod);
        Root.AddChild(Global.BasicFPSLabel);

        Global.Discord.SetPresence(new DiscordRPC.RichPresence() {
            Details = "Viewing map '" + (Global.SelectedMap?.Title ?? "") + "'",
            Timestamps = DiscordRPC.Timestamps.Now,
        });
    }

    private void GoToMenu() {
        Window?.SceneHandler.RemoveSceneByType<MapInfoScene>();
        Global.LoadedMenu?.SetRPC();
        if(Global.LoadedMenu is not null) {
            if(((UiElement)Global.LoadedMenu.MapGrid.Children[0]).Children.Count != MapLoader.Maps.Count) {
                Global.LoadedMenu.MapGrid = new();
                Global.LoadedMenu.MakeMapList();
            }
            Window?.SceneHandler.AddScene(Global.LoadedMenu);
        }
    }

    private void PlayMap() {
        Window?.SceneHandler.RemoveSceneByType<MapInfoScene>();
        Window?.SceneHandler.AddScene(new GameScene());
    }

    public override void Render() {
        Global.BasicFPSLabel.Text = Raylib.GetFPS().ToString() + " FPS";
        Root.Render(Raylib.GetRenderWidth(), Raylib.GetRenderHeight());
    }

    public override void Update(double dt) {
        Mods.Speed = SpeedMod.Value;
        Root.Update(dt);
    }
}