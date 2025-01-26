using System.Numerics;
using System.Runtime.InteropServices;
using Raylib_cs;
using Stasis.Content.Beatmaps;
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
    public Frame MainFrame;
    public TextBox SearchBox;

    public string LastSearch = "";

    public ScrollContainer MakeMapList() {
        MapGrid = new ScrollContainer() {
            Size = new UDim2(1f, 0, 1f, 0),
        };

        var mapList = new GridContainer() {
            Padding = 17,
            ItemsPerRow = 8,
            Size = new UDim2(0.97f, 0, 0.95f, 0),
            Position = new UDim2(0.03f, 0, 0.05f, 0),
        };

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
            mapList.AddChild(button);
        }

        mapList.UpdateAbsoluteValues(new Vector2(Raylib.GetRenderWidth(), Raylib.GetRenderHeight()), new Vector2(0, 0));
        MapGrid.AddChild(mapList);

        return MapGrid;
    }

    public MenuScene() {
        MainFrame = new Frame() {
            Size = new UDim2(1f, 0, 1, 0),
            Color = new Color(12, 12, 12, 255),
        };

        MapGrid = MakeMapList();
        MetaFrame = new Frame() {
            Color = Raylib.ColorFromNormalized(new Vector4(0.03f, 0.03f, 0.03f, 1f)),
            Size = new UDim2(0.97f, 0, 0.07f, 0),
            Position = new UDim2(0.03f, 0, -0.02f, 0),
            Roundness = 0.5f,
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
                AlignmentY = TextAlignY.Top,
                Position = new UDim2(0.04f, 0, 0.05f, 0),
                Size = new UDim2(0.96f, 0, 0.95f, 0),
                Text = "Search...",
                FontSize = 18,
                Font = Global.UIFont,
                TextColor = Color.Gray,
                OneLine = true,
            },
            Text = new() {
                AlignmentX = TextAlignX.Left,
                AlignmentY = TextAlignY.Top,
                Position = new UDim2(0.04f, 0, 0.05f, 0),
                Size = new UDim2(0.96f, 0, 0.95f, 0),
                FontSize = 18,
                Font = Global.UIFont,
                TextColor = Color.White,
                OneLine = true,
            },
        };

        MetaFrame.AddChild(SearchBox);

        Root.AddChild(MainFrame);
        Root.AddChild(MapGrid);
        Root.AddChild(MetaFrame);
        Root.AddChild(Global.BasicFPSLabel);

        SetRPC();
    }

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
        Root.Update(dt);
        if(LastSearch != SearchBox.Text.Text) MapGrid.Scroll = 0;
        foreach(UiElement button in ((UiElement)MapGrid.Children[0]).Children) {
            if(button is TestMapButton mapButton) {
                button.IgnoreUpdate = SearchBox.IsHovering();
                button.Visible =
                    mapButton.Map.Title.Contains(SearchBox.Text.Text, StringComparison.CurrentCultureIgnoreCase) ||
                    mapButton.Map.Artist.Contains(SearchBox.Text.Text, StringComparison.CurrentCultureIgnoreCase) ||
                    mapButton.Map.Mappers.Any(p => p.Contains(SearchBox.Text.Text, StringComparison.CurrentCultureIgnoreCase));
            }
        }
        LastSearch = SearchBox.Text.Text;
    }
}