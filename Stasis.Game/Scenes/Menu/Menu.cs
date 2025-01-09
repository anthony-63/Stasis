using System.Numerics;
using Raylib_cs;
using Stasis.Content.Beatmaps;
using Stasis.Content.Settings;
using Stasis.Engine;
using Stasis.Engine.Scene;
using Stasis.Engine.UI;
using Stasis.Engine.UI.Elements;
using Stasis.Game.Scenes.Loading;

namespace Stasis.Game.Scenes.Menu;

public class MenuScene : IScene {
    public UiRoot TestUI = new();
    public GridContainer MapGrid = new();

    public Label FPSLabel = new() {
        TextColor = Color.Green,
        FontSpacing = 2,
        FontSize = 24,
        AlignmentX = TextAlignX.Left,
        AlignmentY = TextAlignY.Top,
    };
    public MenuScene() {
        Font font = Raylib.LoadFontEx("Assets/Game/font.ttf", 18, [], 0);
        MapGrid = new GridContainer() {
            Padding = 0.5f,
            Size = new UDim2(1f, 0, 1f, 0),
        };

        foreach(IBeatmapSet map in MapLoader.Maps) {
            MapGrid.Children.Add(new TestMapButton() {
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

        TestUI.Children.Add(MapGrid);
        TestUI.Children.Add(FPSLabel);
    }

    public void Render(Window window) {
        TestUI.Render(Raylib.GetRenderWidth(), Raylib.GetRenderHeight());
    }

    public void Update(Window window, double dt) {
        FPSLabel.Text = $"FPS: {Raylib.GetFPS()}";
        TestUI.Update(dt);
    }
}