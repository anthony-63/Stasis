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

    Font Font = Raylib.LoadFontEx("Assets/Game/font.ttf", 18, [], 0);

    public ScrollContainer MakeMapList() {
        MapGrid = new ScrollContainer() {
            Size = new UDim2(1f, 0, 1f, 0),
        };

        var mapList = new GridContainer() {
            Padding = 6,
            ItemsPerRow = 8,
            Size = new UDim2(0.97f, 0, 0.95f, 0),
            Position = new UDim2(0.03f, 0, 0.05f, 0),
        };

        foreach(IBeatmapSet map in MapLoader.Maps) {
            var testFrame = new Frame {
                Color = Raylib.ColorFromNormalized(new Vector4(0.1f, 0.1f, 0.1f, 1f)),
                BorderWidth = 1,
                BorderColor = Color.LightGray,
            };

            var button = new TestMapButton() {
                Map = map,
                Size = UDim2.Zero,
                Position = UDim2.Zero,
                Anchor = UiElementAnchor.TopLeft,
                NormalFrame = testFrame,
                HoveringFrame = testFrame,
                PressedFrame = testFrame,
                Label = new Label {
                    Visible = false,
                }
            };
            if(map.Cover.Length > 0) {
                button.Children.Add(new ImageFrame {
                    ImageData = map.Cover,
                    Size = new UDim2(1, 0, 1, 0),
                });
                button.Children.Add(new Frame {
                    Size = new UDim2(1, 0, 1, 0),
                    Color = new Color(12, 12, 12, 150),
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
        MapGrid = MakeMapList();
        MetaFrame = new Frame() {
            Color = Raylib.ColorFromNormalized(new Vector4(0.05f, 0.05f, 0.05f, 1f)),
            Size = new UDim2(0.97f, 0, 0.05f, 0),
            Position = new UDim2(0.03f, 0, 0, 0),
        };
        
        TestUI.Children.Add(MapGrid);
        TestUI.Children.Add(MetaFrame);
    }

    public void Render(Window window) {
        TestUI.Render(Raylib.GetRenderWidth(), Raylib.GetRenderHeight());
        Raylib.DrawFPS(10, 10);
    }

    public void Update(Window window, double dt) {
        foreach(UiElement element in ((UiElement)MapGrid.Children.Where(t => t is GridContainer).First()).Children) {
            if(element is TestMapButton t) {
                t.CheckPressed(window, this);
            }
        }
        TestUI.Update(dt);
    }
}