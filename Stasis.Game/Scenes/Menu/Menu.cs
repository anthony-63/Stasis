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

    public ScrollContainer MakeMapList() {
        MapGrid = new ScrollContainer() {
            Size = new UDim2(1f, 0, 1f, 0),
        };

        var mapList = new GridContainer() {
            Padding = 6,
            ItemsPerRow = 8,
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
            Color = Raylib.ColorFromNormalized(new Vector4(0.05f, 0.05f, 0.05f, 1f)),
            Size = new UDim2(1f, 0, 0.05f, 0),
        };
        Root.AddChild(MainFrame);
        Root.AddChild(MapGrid);
        Root.AddChild(MetaFrame);
        Root.AddChild(Global.BasicFPSLabel);
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
    }
}