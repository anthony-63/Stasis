using System.Numerics;
using Microsoft.VisualBasic;
using Raylib_cs;
using Stasis.Content.Beatmaps;
using Stasis.Engine;
using Stasis.Engine.Scene;
using Stasis.Engine.UI;
using Stasis.Engine.UI.Elements;
using Stasis.Game.Scenes.Loading;
using Stasis.Game.Scenes.MapInfo;

namespace Stasis.Game.Scenes.Menu;

public class MenuScene : Scene {
    public UiRoot Root = new();
    public Frame MetaFrame;
    public ScrollContainer MapGrid;
    public GridContainer MapList;
    public TabContainer MainTabContainer;
    public Frame MainFrame;
    public TextBox SearchBox;
    public Button ReloadMaps;
    public Frame SettingsFrame;

    private float settingsTabButtonPlace = 0;

    public string LastSearch = "";

    public void LoadMapButtons(GridContainer mapGrid) {
        foreach(IBeatmapSet map in MapLoader.Maps) {
            var testFrame = new Frame {
                Color = Raylib.ColorFromNormalized(new Vector4(0.1f, 0.1f, 0.1f, 1f)),
                BorderWidth = 3,
                // Roundness = 0.15f,
                BorderColor = Color.White,
            };

            var button = new TestMapButton(map, SelectMap) {
                Size = UDim2.Zero,
                Position = UDim2.Zero,
                Anchor = UiElementAnchor.TopLeft,
                NormalFrame = testFrame,
                HoveringFrame = testFrame,
                PressedFrame = testFrame,
                Label = new Label {
                    Visible = false,
                },
            };

            if(map.Cover.Length > 0) {
                button.AddChild(new ImageFrame {
                    ImageData = map.Cover,
                    Size = UDim2.Fill,
                });
                button.AddChild(new Frame {
                    Size = UDim2.Fill,
                    Color = new Color(12, 12, 12, 200),
                });
                button.AddChild(button.Children.First());
                button.Children.RemoveAt(0);
            }
            button.AddChild(new Label {
                Size = new UDim2(0.9f, 0, 0.9f, 0),
                Position = new UDim2(0.05f, 0, 0.05f, 0),
                AlignmentX = TextAlignX.Left,
                AlignmentY = TextAlignY.Top,
                Text = map.Title,
                FontSize = 18,
                Font = Global.UIFont,
                TextWrapped = true,
            });
            mapGrid.AddChild(button);
        }
    }

    public ScrollContainer MakeMapList() {
        MapGrid = new ScrollContainer() {
            Size = new UDim2(1f, 0, 0.95f, 0),
            Position = new UDim2(0, 0, 0.05f, 0),
            ClipContents = true,
        };

        MapList = new GridContainer() {
            Padding = 17,
            ItemsPerRow = 8,
            Size = UDim2.Fill,
        };

        LoadMapButtons(MapList);

        MapList.UpdateAbsoluteValues(new Vector2(Raylib.GetRenderWidth(), Raylib.GetRenderHeight()), new Vector2(0, 0));
        MapGrid.AddChild(MapList);

        return MapGrid;
    }

    public void ReloadMapsAction() {
        MapGrid.Scroll = 0;
        foreach(TestMapButton button in MapList.Children) {
            button.Unload();
        }
        MapList.Children.Clear();
        LoadMapButtons(MapList);            
    }

    public TabContainerTab MakeSettingsTab(string name) {
        var tab = new TabContainerTab() {
            Size = new UDim2(1, 0, 0.95f, 0),
            Position = new UDim2(0, 0, 0.05f, 0),
            Color = MainFrame.Color,
            SwapTo = new Button() {
                Position = new UDim2(0, settingsTabButtonPlace, 0, 2),
                Size = new UDim2(0, 200, 0.045f, 0),
                NormalFrame = new Frame() {
                    BorderWidth = 0.5f,
                    Color = new Color(22, 22, 22, 255),
                },
                HoveringFrame = new Frame() {
                    BorderWidth = 0.5f,
                    Color = new Color(32, 32, 32, 255),
                },
                Label = new Label() {
                    Size = UDim2.Fill,
                    AlignmentX = TextAlignX.Center,
                    AlignmentY = TextAlignY.Middle,
                    OneLine = true,
                    Text = name,
                    FontSize = 24,
                    Font = Global.UIFont,
                }
            }
        };

        var titleFrame = new Frame() {
            Color = MainFrame.Color,
            BorderWidth = 0.5f,
            Size = new UDim2(1, -2.5f, 0, 50),
            Position = new UDim2(0, 0, 0, 0),
        };
        var titleLabel = new Label() {
            AlignmentX = TextAlignX.Center,
            AlignmentY = TextAlignY.Top,
            FontSize = 48,
            Size = UDim2.Fill,
            Font = Global.UIFont,
            Text = name,
            OneLine = true,
        };
        titleFrame.AddChild(titleLabel);
        tab.AddChild(titleFrame);

        settingsTabButtonPlace += 201;

        return tab;
    }

    public Frame MakeSettingsContainerFrame() {
        var frame = new Frame() {
            Size = new UDim2(1, -2.5f, 1, -55),
            Position = new UDim2(0, 0, 0, 52.5f),
            Color = MainFrame.Color,
            BorderWidth = 0.5f,
        };
        return frame;
    }

    public void MakeSettingSpinbox(Frame parent, float defaultValue, string name, int y, SpinBox.ValueChangedEvent valueChanged) {
        var label = new Label() {
            Size = new UDim2(1, 0, 0, 1),
            Position = new UDim2(0, 10, 0, y),
            AlignmentX = TextAlignX.Left,
            AlignmentY = TextAlignY.Middle,
            Text = name,
            FontSize = 32,
            Font = Global.UIFont,
            OneLine = true,
        };
        label.UpdateAbsoluteValues(parent.AbsoluteSize, parent.AbsolutePosition);

        var spinBox = new SpinBox() {
            Size = new UDim2(0, 100, 0, label.FontSize),
            Position = new UDim2(0, 250, 0, 0),
            Step = 0.01f,
            Anchor = UiElementAnchor.MiddleLeft,
            NormalFrame = new Frame() {
                Color = new Color(9, 9, 9, 255),
                BorderWidth = 0.5f,
                Roundness = 0.5f,
            },
            FocusedFrame = new Frame() {
                Color = new Color(15, 15, 15, 255),
                BorderWidth = 0.5f,
                Roundness = 0.5f,
            },
            Text = new Label() {
                OneLine = true,
                FontSize = 18,
                Size = UDim2.Fill,
                Font = Global.UIFont,
            },
            Placeholder = new Label() {
                OneLine = true,
                FontSize = 18,
                Size = UDim2.Fill,
                Font = Global.UIFont,
                Text = "Enter Value",
            },
            Value = defaultValue,
        };
        spinBox.ValueChanged += valueChanged;

        label.AddChild(spinBox);

        parent.AddChild(label);
    }

    public void MakeSettingsToggle(Frame parent, bool defaultValue, string name, int y, Button.ButtonToggleEvent valueChanged) {
        var label = new Label() {
            Size = new UDim2(1, 0, 0, 1),
            Position = new UDim2(0, 10, 0, y),
            AlignmentX = TextAlignX.Left,
            AlignmentY = TextAlignY.Middle,
            Text = name,
            FontSize = 32,
            Font = Global.UIFont,
            OneLine = true,
        };
        label.UpdateAbsoluteValues(parent.AbsoluteSize, parent.AbsolutePosition);

        var button = new Button() {
            Size = new UDim2(0, 100, 0, label.FontSize),
            Position = new UDim2(0, 250, 0, 0),
            Anchor = UiElementAnchor.MiddleLeft,
            ToggledValue = defaultValue,
            Toggle = true,
            NormalFrame = new Frame() {
                Color = new Color(9, 9, 9, 255),
                BorderWidth = 0.5f,
                Roundness = 0.5f,
            },
            HoveringFrame = new Frame() {
                Color = new Color(16, 16, 16, 255),
                BorderWidth = 0.5f,
                Roundness = 0.5f,
            },
            PressedFrame = new Frame() {
                Color = new Color(22, 22, 22, 255),
                BorderWidth = 0.5f,
                Roundness = 0.5f,
            },
            Label = new Label() {
                OneLine = true,
                Size = UDim2.Fill,
                FontSize = 18,
                Font = Global.UIFont,
                Text = defaultValue ? "Enabled" : "Disabled",
            },
        };
        button.Toggled += (b) => {
            if(button.ToggledValue) button.Label.Text = "Enabled";
            else button.Label.Text = "Disabled";
            valueChanged(b);
        };

        label.AddChild(button);

        parent.AddChild(label);
    }

    public TabContainerTab MakeNoteSettings() {
        var tab = MakeSettingsTab("Note Settings");
        var container = MakeSettingsContainerFrame();
        MakeSettingSpinbox(container, Global.Settings.Note.ApproachTime, "Approach Time", 20, UpdateApproachTime);
        MakeSettingSpinbox(container, Global.Settings.Note.ApproachDistance, "Approach Distance", 60, UpdateApproachDistance);
        MakeSettingSpinbox(container, Global.Settings.Note.FadeIn, "Fade In", 100, UpdateFadeIn);
        MakeSettingsToggle(container, Global.Settings.Note.HalfGhost, "Half Ghost", 140, UpdateHalfGhost);
        MakeSettingsToggle(container, Global.Settings.Note.Pushback, "Pushback", 180, UpdatePushback);
        tab.AddChild(container);
        return tab;
    }

    void SaveSettings() {
        Global.Settings.Save("Assets/settings.toml");
    }

    public void UpdateApproachTime(float value) {
        Global.Settings.Note.ApproachTime = value;
        SaveSettings();
    }

    public void UpdateFadeIn(float value) {
        Global.Settings.Note.FadeIn = value;
        SaveSettings();
    }

    public void UpdateHalfGhost(bool value) {
        Global.Settings.Note.HalfGhost = value;
        SaveSettings();
    }

    public void UpdateApproachDistance(float value) {
        Global.Settings.Note.ApproachDistance = value;
        SaveSettings();
    }

    public void UpdatePushback(bool value) {
        Global.Settings.Note.Pushback = value;
        SaveSettings();
    }


    public TabContainerTab MakeCursorSettings() {
        var tab = MakeSettingsTab("Cursor Settings");
        var container = MakeSettingsContainerFrame();

        MakeSettingSpinbox(container, Global.Settings.Cursor.Sensitivity, "Sensitivity", 20, UpdateSensitivity);
        MakeSettingSpinbox(container, Global.Settings.Cursor.Scale, "Scale", 60, UpdateCursorScale);
        MakeSettingsToggle(container, Global.Settings.Cursor.Clamped, "Clamped", 100, UpdateClamped);

        tab.AddChild(container);
        return tab;
    }

    public void UpdateSensitivity(float value) {
        Global.Settings.Cursor.Sensitivity = value;
        SaveSettings();
    }

    public void UpdateCursorScale(float value) {
        Global.Settings.Cursor.Scale = value;
        SaveSettings();
    }

    public void UpdateClamped(bool value) {
        Global.Settings.Cursor.Clamped = value;
        SaveSettings();
    }

    public TabContainerTab MakeCameraSettings() {
        var tab = MakeSettingsTab("Camera Settings");
        var container = MakeSettingsContainerFrame();

        MakeSettingSpinbox(container, Global.Settings.Camera.FOV, "FOV", 20, UpdateFOV);
        MakeSettingSpinbox(container, Global.Settings.Camera.CameraParallax, "Camera Parallax", 60, UpdateCameraParallax);
        MakeSettingSpinbox(container, Global.Settings.Camera.GridParallax, "Grid Parallax", 100, UpdateGridParallax);

        tab.AddChild(container);
        return tab;
    }

    public void UpdateFOV(float value) {
        Global.Settings.Camera.FOV = value;
        SaveSettings();
    }

    public void UpdateCameraParallax(float value) {
        Global.Settings.Camera.CameraParallax = value;
        SaveSettings();
    }

    public void UpdateGridParallax(float value) {
        Global.Settings.Camera.GridParallax = value;
        SaveSettings();
    }

    public TabContainerTab MakeAudioSettings() {
        var tab = MakeSettingsTab("Audio Settings");
        var container = MakeSettingsContainerFrame();

        MakeSettingSpinbox(container, Global.Settings.Audio.Volume, "Music Volume", 20, UpdateVolume);
        MakeSettingSpinbox(container, Global.Settings.Audio.FXVolume, "Effects Volume", 60, UpdateFXVolume);

        tab.AddChild(container);
        return tab;
    }

    public void UpdateVolume(float value) {
        Global.Settings.Audio.Volume = value;
        SaveSettings();
    }

    public void UpdateFXVolume(float value) {
        Global.Settings.Audio.FXVolume = value;
        SaveSettings();
    }

    public TabContainerTab MakeMiscSettings() {
        var tab = MakeSettingsTab("Miscellaneous");
        var container = MakeSettingsContainerFrame();
        MakeSettingsToggle(container, Global.Settings.Misc.EnableReplays, "Enable Replays", 20, UpdateEnableReplay);
        tab.AddChild(container);
        return tab;
    }

    public void UpdateEnableReplay(bool value) {
        Global.Settings.Cursor.Clamped = value;
        SaveSettings();
    }

    public TabContainerTab MakeAdvancedSettings() {
        var tab = MakeSettingsTab("Advanced(breakable)");
        var container = MakeSettingsContainerFrame();
        tab.AddChild(container);
        return tab;
    }

    public TabContainerTab MakeSettings() {
        var frame = new TabContainerTab() {
            Size = new UDim2(0.95f, -5, 1f, 0),
            Position = new UDim2(0.05f, 5, 0, 0),
            Color = MainFrame.Color,
            SwapTo = new Button() {
                Size = new UDim2(0.05f, 0, 0.082f, 0),
                Position = new UDim2(0, 0, 0.082f, 0),
                NormalFrame = new Frame() {
                    Color = new Color(8, 8, 8, 255),
                },
                HoveringFrame = new Frame() {
                    Color = new Color(12, 12, 12, 255),
                },
                Children = [
                    new ImageFrame() {
                        Size = UDim2.Fill,
                        ImagePath = Global.GetAsset("Assets/Menu/SettingsButton.png"),
                    }
                ]
            },
        };

        frame.AddChild(new TabContainer() {
            Tabs = [
                MakeNoteSettings(),
                MakeCursorSettings(),
                MakeCameraSettings(),
                MakeAudioSettings(),
                MakeMiscSettings(),
                MakeAdvancedSettings(),
            ]
        });

        return frame;
    }


    #pragma warning disable CS8618
    public MenuScene() {
        MainFrame = new Frame() {
            Size = new UDim2(1f, 0, 1, 0),
            Color = new Color(12, 12, 12, 255),
        };

        MetaFrame = new Frame() {
            Color = Raylib.ColorFromNormalized(new Vector4(0.03f, 0.03f, 0.03f, 1f)),
            Size = new UDim2(1, 0, 0.07f, 0),
            Position = new UDim2(0, 0, -0.02f, 0),
            Roundness = 0.5f,
        };

        ReloadMaps = new Button() {
            Position = new UDim2(0.22f, 0, 0.42f, 0),
            Size = new UDim2(0.09f, 0, 0.4f, 0),
            Label = new Label() {
                Text = "Reload Maps",
                Size = UDim2.Fill,
                AlignmentX = TextAlignX.Center,
                FontSize = 18,
                Font = Global.UIFont,
                OneLine = true,
            },

            NormalFrame = new() {
                Color = Color.DarkGray,
                BorderWidth = 1f,
                BorderColor = Color.Gray,
                Roundness = 3f,
            },

            PressedFrame = new() {
                Color = new Color(120, 120, 120, 255),
                BorderWidth = 1f,
                BorderColor = Color.Gray,
                Roundness = 3f,
            },

            HoveringFrame = new() {
                Color = new Color(100, 100, 100, 255),
                BorderWidth = 1f,
                BorderColor = Color.Gray,
                Roundness = 3f,
            },
        };

        SearchBox = new TextBox() {
            Position = new UDim2(0.01f, 0, 0.42f, 0),
            Size = new UDim2(0.2f, 0, 0.4f, 0),
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
                Text = "Search...",
                FontSize = 18,
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
                Font = Global.UIFont,
                TextColor = Color.White,
                OneLine = true,
            },
        };

        ReloadMaps.PressedOnce += ReloadMapsAction;

        MetaFrame.AddChild(ReloadMaps);
        MetaFrame.AddChild(SearchBox);

        MapGrid = MakeMapList();

        var leftFrame = new Frame() {
            Color = new Color(3, 3, 3, 255),
            Size = new UDim2(0.05f, 0, 1, 0),
        };
        MainTabContainer = new TabContainer() {
            Tabs = [
                new TabContainerTab() {
                    Size = new UDim2(0.95f, 0, 1f, 0),
                    Position = new UDim2(0.05f, 0, 0, 0),
                    Color = MainFrame.Color,
                    SwapTo = new Button() {
                        Size = new UDim2(0.05f, 0, 0.082f, 0),
                        NormalFrame = new Frame() {
                            Color = new Color(8, 8, 8, 255),
                        },
                        HoveringFrame = new Frame() {
                            Color = new Color(12, 12, 12, 255),
                        },
                        Children = [
                            new ImageFrame() {
                                Size = UDim2.Fill,
                                ImagePath = Global.GetAsset("Assets/Menu/PlayButton.png"),
                            }
                        ]
                    },
                },
                MakeSettings(),
            ]
        };

        MainTabContainer.Tabs[0].AddChild(MapGrid);
        MainTabContainer.Tabs[0].AddChild(MetaFrame);

        Root.AddChild(MainFrame);
        Root.AddChild(leftFrame);
        Root.AddChild(MainTabContainer);
        Root.AddChild(Global.BasicFPSLabel);

        SetRPC();
    }
    #pragma warning restore CS8618

    public void SetRPC() {
        // Global.Discord.SetPresence(new DiscordRPC.RichPresence() {
        //     Details = "In Menu",
        //     State = "Listening to PLACEHOLDER",
        //     Timestamps = DiscordRPC.Timestamps.Now,
        // });
    }

    public void SelectMap(IBeatmapSet map) {
        Global.SelectedMap = map;
        Global.LoadedMenu = this;
        Window?.SceneHandler.RemoveSceneByType<MenuScene>();
        Window?.SceneHandler.AddScene(new MapInfoScene());
    }

    public override void Render() {
        Global.BasicFPSLabel.Text = Raylib.GetFPS().ToString() + " FPS";
        Root.Render(Raylib.GetRenderWidth(), Raylib.GetRenderHeight());
    }

    public override void Update(double dt) {
        if(Raylib.IsKeyPressed(KeyboardKey.F2)) {
            SelectMap(MapLoader.Maps[Global.Random.Next(MapLoader.Maps.Count)]);
        }
        
        if(Raylib.IsFileDropped()) {
            var files = Raylib.GetDroppedFiles();
            foreach(string file in files) {
                Logger.Info("Loading drag and dropped map: ", file);
                var newPath = "Assets/Maps/" + Path.GetFileName(file);
                File.Copy(file, newPath);
                MapLoader.LoadMap(newPath);
            }
            ReloadMapsAction();
        }

        Root.Update(dt);
        if(LastSearch != SearchBox.Text.Text) MapGrid.Scroll = 0;
        foreach(UiElement button in MapGrid.Children[0].Children) {
            if(button is TestMapButton mapButton) {
                button.IgnoreUpdate = MetaFrame.IsHovering();
                button.Visible =
                    mapButton.Map.Title.Contains(SearchBox.Text.Text, StringComparison.CurrentCultureIgnoreCase) ||
                    mapButton.Map.Artist.Contains(SearchBox.Text.Text, StringComparison.CurrentCultureIgnoreCase) ||
                    mapButton.Map.Mappers.Any(p => p.Contains(SearchBox.Text.Text, StringComparison.CurrentCultureIgnoreCase));
            }
        }
        LastSearch = SearchBox.Text.Text;
    }
}