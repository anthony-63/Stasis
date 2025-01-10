using System.Numerics;
using System.Runtime.Intrinsics;
using Raylib_cs;
using Stasis.Content.Beatmaps;
using Stasis.Engine;
using Stasis.Engine.Scene;
using Stasis.Engine.UI;
using Stasis.Engine.UI.Elements;
using Stasis.Game.Scenes.Loading;

namespace Stasis.Game.Scenes.Menu;

public class MenuScene : IScene {
    public UiRoot TestUI = new();
    public Frame MetaFrame = new();
    public ScrollContainer MapGrid = new();

    public Label FPSLabel = new() {
        TextColor = Color.Green,
        FontSpacing = 2,
        FontSize = 24,
        AlignmentX = TextAlignX.Left,
        AlignmentY = TextAlignY.Top,
    };
    public MenuScene() {
        Font font = Raylib.LoadFontEx("Assets/Game/font.ttf", 18, [], 0);
        MapGrid = new ScrollContainer() {
            Size = new UDim2(1f, 0, 1f, 0),
        };

        MetaFrame = new Frame() {
            Color = Raylib.ColorFromNormalized(new Vector4(0.05f, 0.05f, 0.05f, 1f)),
            Size = new UDim2(0.97f, 0, 0.05f, 0),
            Position = new UDim2(0.03f, 0, 0, 0),
        };

        var mapList = new GridContainer() {
            Padding = 6,
            ItemsPerRow = 8,
            Size = new UDim2(0.97f, 0, 0.95f, 0),
            Position = new UDim2(0.03f, 0, 0.05f, 0),
        };

        foreach(IBeatmapSet map in MapLoader.Maps) {
            mapList.Children.Add(new TestMapButton() {
                Map = map,
                Size = UDim2.Zero,
                Position = UDim2.Zero,
                Anchor = UiElementAnchor.TopLeft,
                NormalFrame = new Frame {
                    Color = Raylib.ColorFromNormalized(new Vector4(0.1f, 0.1f, 0.1f, 1f)),
                    BorderWidth = 1,
                    BorderColor = Color.LightGray,
                },
                HoveringFrame = new Frame {
                    Color = Raylib.ColorFromNormalized(new Vector4(0.3f, 0.3f, 0.3f, 1f)),
                    BorderWidth = 1,
                    BorderColor = Color.LightGray,
                },
                PressedFrame = new Frame {
                    Color = Raylib.ColorFromNormalized(new Vector4(0.5f, 0.5f, 0.5f, 5f)),
                    BorderWidth = 1,
                    BorderColor = Color.LightGray,
                },
                Label = new Label {
                    Size = new UDim2(1f, 0, 1f, 0),
                    Position = UDim2.Zero,
                    AlignmentX = TextAlignX.Center,
                    AlignmentY = TextAlignY.Middle,
                    Text = map.Title,
                    Font = font,
                    FontSize = 18,
                    TextWrapped = true,
                }
            });
        }
        MapGrid.Children.Add(mapList);
        TestUI.Children.Add(MapGrid);
        MapGrid.Scroll = Global.LastScroll;
        
        TestUI.Children.Add(MetaFrame);
        TestUI.Children.Add(FPSLabel);
    }

    public void Render(Window window) {
        TestUI.Render(Raylib.GetRenderWidth(), Raylib.GetRenderHeight());
    }

    public void Update(Window window, double dt) {
        FPSLabel.Text = $"FPS: {Raylib.GetFPS()}";
        foreach(UiElement element in ((UiElement)MapGrid.Children.Where(t => t is GridContainer).First()).Children) {
            if(element is TestMapButton t) {
                t.CheckPressed(window);
            }
        }
        TestUI.Update(dt);
        Global.LastScroll = MapGrid.Scroll;

    }
}