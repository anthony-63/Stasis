using Raylib_cs;
using Stasis.Content.Beatmaps;
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

    Frame Leaderboard;

    SpinBox SpeedMod;
    Button NoFailMod;
    Button VisualMapMod;
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

    Frame MakeLeaderboard() {
        var lbFrame = new Frame() {
            Position = new UDim2(1, -20, 0, 30),
            Anchor = UiElementAnchor.TopRight,
            Size = new UDim2(0.3f, 0, 1, -185),
            Color = new Color(20, 20, 20, 255),
            BorderWidth = 2,
            Roundness = 0.1f,
        };

        var title = new Label() {
            Position = new UDim2(0, 0, 0, 20),
            Size = new UDim2(1, 0, 0, 0),
            FontSize = 32,
            Font = Global.UIFont,
            Text = "Local Leaderboard",
        };

        var scrollContainer = new ScrollContainer() {
            Position = new UDim2(0, 0, 0.08f, 0),
            Size = new UDim2(1, 0, 0.92f, 0),
        };

        var gridContainer = new GridContainer() {
            ItemsPerRow = 1,
            Padding = 8,
            Size = new UDim2(1, -20, 1, 0),
            Position = new UDim2(0, 10, 0, 0),
            SquareItems = false,
        };

        var lbEntries = LeaderboardLoader.LoadLeaderboardFromMap(Global.SelectedMap ?? new BeatmapSet());

        foreach(var e in lbEntries) {
            var entry = new Frame() {
                Size = new UDim2(0, 0, 0, 50),
                Color = new Color(30, 30, 30, 255),
                Roundness = 0.5f,
                BorderWidth = 2,
            };

            var failFrame = new Frame() {
                Size = UDim2.Fill,
                Color = new Color(155, 0, 0, 20),
                Visible = e.Score.Failed,
            };

            var acc = new Label() {
                Size = UDim2.Fill,
                Position = new UDim2(0, -8, 0, 4),
                FontSize = 18,
                Font = Global.UIFont,
                TextColor = Color.Gray,
                AlignmentX = TextAlignX.Right,
                AlignmentY = TextAlignY.Middle,
                OneLine = true,
                Text = e.Score.AccPlaceholder.ToString("0.00") + "%",
            };

            var scoreOrProgress = new Label() {
                Size = UDim2.Fill,
                Position = new UDim2(0, -8, 0, 0),
                FontSize = 24,
                Font = Global.UIFont,
                TextColor = Color.White,
                AlignmentX = TextAlignX.Right,
                AlignmentY = TextAlignY.Top,
                OneLine = true,
                Text = e.Score.ScoreValue.ToString(),
            };

            if(e.Score.Failed) {
                scoreOrProgress.TextColor = Color.Gray;
                var startTime = TimeSpan.FromSeconds(e.timeStart);
                var endTime = TimeSpan.FromSeconds(e.timeEnd);
                scoreOrProgress.Text = string.Format("{0:D1}:{1:D2}", startTime.Minutes, startTime.Seconds) + " / " + string.Format("{0:D1}:{1:D2}", endTime.Minutes, endTime.Seconds);
            }

            var playerName = new Label() {
                Size = new UDim2(0.98f, 0, 1, 0),
                Position = new UDim2(0.02f, 0, 0, 4),
                FontSize = 24,
                Font = Global.UIFont,
                AlignmentX = TextAlignX.Left,
                AlignmentY = TextAlignY.Top,
                OneLine = true,
                Text = "You",
            };

            var mods = new Label() {
                Size = new UDim2(0.98f, 0, 1, 0),
                Position = new UDim2(0.02f, 0, 0, -4),
                FontSize = 18,
                Font = Global.UIFont,
                TextColor = Color.DarkGray,
                AlignmentX = TextAlignX.Left,
                AlignmentY = TextAlignY.Bottom,
                OneLine = true,
                Text = Global.GetModText(e.Mods),
            };

            string from = (e.time - DateTime.Now) switch
            {
                { TotalHours: < 1 } ts => $"{ts.Minutes} minutes ago",
                { TotalDays: < 1 } ts => $"{ts.Hours} hours ago",
                { TotalDays: < 2 } => $"yesterday",
                { TotalDays: < 5 } => $"on {e.time.DayOfWeek}",
                var ts => $"{ts.Days} days ago",
            };

            var timeSet = new Label() {
                Size = UDim2.Fill,
                Position = new UDim2(0, -8, 0, 0),
                FontSize = 16,
                Font = Global.UIFont,
                TextColor = Color.DarkGray,
                AlignmentX = TextAlignX.Right,
                AlignmentY = TextAlignY.Bottom,
                OneLine = true,
                Text = from,
            };

            entry.AddChild(acc);
            entry.AddChild(scoreOrProgress);
            entry.AddChild(timeSet);
            entry.AddChild(playerName);
            entry.AddChild(mods);
            entry.AddChild(failFrame);
            gridContainer.AddChild(entry);
        }

        scrollContainer.AddChild(gridContainer);

        lbFrame.AddChild(title);
        lbFrame.AddChild(scrollContainer);

        return lbFrame;
    }

    SpinBox MakeSpeedMod() {
        var speedMod = new SpinBox() {
            Value = Global.Mods.Speed,
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
                Text = Global.Mods.Speed.ToString(),
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

        speedMod.AddChild(speedModLabel);

        return speedMod;
    }

    Button MakeNoFailMod() {
        Frame buttonFrame = new Frame() {
            Size = UDim2.Fill,
            Color = new Color(60, 60, 60, 255),
            BorderWidth = 2,
            Roundness = 0.3f,
        };

        Frame toggleFrame = new Frame() {
            Size = UDim2.Fill,
            Color = new Color(100, 100, 100, 255),
            BorderWidth = 2,
            Roundness = 0.3f,
        };

        Frame hoveringFrame = new Frame() {
            Size = UDim2.Fill,
            Color = new Color(140, 140, 140, 255),
            BorderWidth = 2,
            Roundness = 0.3f,
        };

        var speedModPos = SpeedMod.Position;

        var noFailButton = new Button() {
            Size = new UDim2(0, 188, 0, 25),
            Position = new UDim2(0.01f, 0, speedModPos.Y.Scale, speedModPos.Y.Offset + 50),
            Toggle = true,
            ToggledValue = Global.Mods.NoFail,
            Anchor = UiElementAnchor.MiddleLeft,
            Label = new Label() {
                Text = "No Fail",
                Size = UDim2.Fill,
                AlignmentX = TextAlignX.Center,
                // AlignmentY = TextAlignY.Middle,
                FontSize = 24,
                Font = Global.UIFont,
            },
            NormalFrame = buttonFrame,
            HoveringFrame = hoveringFrame,
            DisabledFrame = buttonFrame,
            PressedFrame = toggleFrame,
        };

        noFailButton.Toggled += (v) => Global.Mods.NoFail = v;

        return noFailButton;
    }

    Button MakeVisualMapModeButton() {
        Frame buttonFrame = new Frame() {
            Size = UDim2.Fill,
            Color = new Color(60, 60, 60, 255),
            BorderWidth = 2,
            Roundness = 0.3f,
        };

        Frame toggleFrame = new Frame() {
            Size = UDim2.Fill,
            Color = new Color(100, 100, 100, 255),
            BorderWidth = 2,
            Roundness = 0.3f,
        };

        Frame hoveringFrame = new Frame() {
            Size = UDim2.Fill,
            Color = new Color(140, 140, 140, 255),
            BorderWidth = 2,
            Roundness = 0.3f,
        };

        var noFailModPos = NoFailMod.Position;

        var visualMapButton = new Button() {
            Size = new UDim2(0, 188, 0, 25),
            Position = new UDim2(0.01f, 0, noFailModPos.Y.Scale, noFailModPos.Y.Offset + 37),
            Toggle = true,
            ToggledValue = Global.Mods.NoFail,
            Anchor = UiElementAnchor.MiddleLeft,
            Label = new Label() {
                Text = "Visual Mode",
                Size = UDim2.Fill,
                AlignmentX = TextAlignX.Center,
                // AlignmentY = TextAlignY.Middle,
                FontSize = 24,
                Font = Global.UIFont,
            },
            NormalFrame = buttonFrame,
            HoveringFrame = hoveringFrame,
            DisabledFrame = buttonFrame,
            PressedFrame = toggleFrame,
        };

        visualMapButton.Toggled += (v) => Global.Mods.VisualMap = v;

        return visualMapButton;
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

        SpeedMod = MakeSpeedMod();
        NoFailMod = MakeNoFailMod();
        VisualMapMod = MakeVisualMapModeButton();

        Leaderboard = MakeLeaderboard();
        
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
        Root.AddChild(Leaderboard);
        Root.AddChild(BackButton);
        Root.AddChild(PlayButton);
        Root.AddChild(SpeedMod);
        Root.AddChild(VisualMapMod);
        Root.AddChild(NoFailMod);
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
        Global.Mods.Speed = SpeedMod.Value;
        Root.Update(dt);
    }
}