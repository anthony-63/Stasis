using System.Numerics;
using System.Runtime.Intrinsics;
using Raylib_cs;
using Stasis.Content.Beatmaps;
using Stasis.Engine;
using Stasis.Engine.Scene;
using Stasis.Engine.UI;
using Stasis.Engine.UI.Elements;
using Stasis.Game.Scenes.Loading;
using Stasis.Game.Scenes.MapInfo;
using TinyTween;

namespace Stasis.Game.Scenes.Menu;

public class MenuScene : Scene {
    public UiRoot Root = new();
    public Frame MetaFrame;
    public ScrollContainer MapGrid;
    public Frame MainFrame;

    Font Font = Raylib.LoadFontEx("Assets/Game/font.ttf", 18, [], 0);

    public ScrollContainer MakeMapList() {
        MapGrid = new ScrollContainer() {
            Size = new UDim2(1f, 0, 1f, 0),
        };
        var mapList = new GridContainer() {
            Padding = 6,
            ItemsPerRow = 10,
            Size = new UDim2(1f, 0, 0.95f, 0),
            Position = new UDim2(0f, 0, 0.05f, 0),
        };

        foreach(IBeatmapSet map in MapLoader.Maps) {
            var testFrame = new Frame {
                Color = Raylib.ColorFromNormalized(new Vector4(0.1f, 0.1f, 0.1f, 1f)),
                BorderWidth = 1,
                BorderColor = Color.LightGray,
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
                button.Children.Add(new ImageFrame {
                    ImageData = map.Cover,
                    Size = new UDim2(1, 0, 1, 0),
                });
                button.Children.Add(new Frame {
                    Size = new UDim2(1, 0, 1, 0),
                    Color = new Color(12, 12, 12, 200),
                });
                button.Children.Add(button.Children.First());
                button.Children.RemoveAt(0);
            }
            button.Children.Add(new Label {
                Size = new UDim2(0.9f, 0, 0.9f, 0),
                Position = new UDim2(0.05f, 0, 0.05f, 0),
                AlignmentX = TextAlignX.Center,
                AlignmentY = TextAlignY.Middle,
                Text = map.Title,
                Font = Font,
                FontSize = 18,
                TextWrapped = true,
            });
            mapList.Children.Add(button);
        }
        MapGrid.Children.Add(mapList);

        return MapGrid;
    }

    public MenuScene() {
        MainFrame = new Frame() {
            Size = new UDim2(1f, 0, 1, 0),
            Color = new Color(12, 12, 12, 255),
        };

        MapGrid = MakeMapList();
        MetaFrame = new Frame() {
            Color = Raylib.ColorFromNormalized(new Vector4(0.05f, 0.05f, 0.05f, 1f)),
            Size = new UDim2(1f, 0, 0.05f, 0),
        };
        Root.Children.Add(MainFrame);
        Root.Children.Add(MapGrid);
        Root.Children.Add(MetaFrame);
    }

    private void SelectMap(IBeatmapSet map) {
        Global.SelectedMap = map;
        Global.LoadedMenu = this;
        Window?.SceneHandler.RemoveSceneByType<MenuScene>();
        Window?.SceneHandler.AddScene(new MapInfoScene());
    }

    public override void Render() {
        Root.Render(Raylib.GetRenderWidth(), Raylib.GetRenderHeight());
        Raylib.DrawFPS(10, 10);
    }

    public override void Update(double dt) {
        Root.Update(dt);
    }
}