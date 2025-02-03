using System.ComponentModel.DataAnnotations;
using System.Numerics;
using System.Runtime.InteropServices;
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
    public Frame MainFrame;
    public TextBox SearchBox;
    public Button ReloadMaps;

    public string LastSearch = "";

    public void LoadMapButtons(ref GridContainer mapGrid) {
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
            Size = new UDim2(1f, 0, 1f, 0),
            ClipContents = true,
        };

        MapList = new GridContainer() {
            Padding = 17,
            ItemsPerRow = 8,
            Size = new UDim2(0.97f, 0, 0.95f, 0),
            Position = new UDim2(0.03f, 0, 0.05f, 0),
        };

        LoadMapButtons(ref MapList);

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
        LoadMapButtons(ref MapList);            
    }


    #pragma warning disable CS8618
    public MenuScene() {
        MainFrame = new Frame() {
            Size = new UDim2(1f, 0, 1, 0),
            Color = new Color(12, 12, 12, 255),
        };

        MetaFrame = new Frame() {
            Color = Raylib.ColorFromNormalized(new Vector4(0.03f, 0.03f, 0.03f, 1f)),
            Size = new UDim2(0.97f, 0, 0.07f, 0),
            Position = new UDim2(0.03f, 0, -0.02f, 0),
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

        Root.AddChild(MainFrame);
        Root.AddChild(MapGrid);
        Root.AddChild(MetaFrame);
        Root.AddChild(Global.BasicFPSLabel);

        SetRPC();
    }
    #pragma warning restore CS8618

    public void SetRPC() {
        Global.Discord.SetPresence(new DiscordRPC.RichPresence() {
            Details = "Viewing Map List",
            Timestamps = DiscordRPC.Timestamps.Now,
        });
    }

    private void SelectMap(IBeatmapSet map) {
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