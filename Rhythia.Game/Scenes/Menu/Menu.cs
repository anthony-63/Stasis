using System.Numerics;
using Raylib_cs;
using Rhythia.Content.Beatmaps;
using Rhythia.Content.Settings;
using Rhythia.Engine;
using Rhythia.Engine.Scene;
using Rhythia.Engine.UI;
using Rhythia.Engine.UI.Elements;
using Rhythia.Game.Scenes.Loading;

namespace Rhythia.Game.Scenes.Menu;

public class MenuScene : IScene {
    public UiRoot TestUI = new();
    public Label FPSLabel = new() {
        TextColor = Color.Green,
        FontSpacing = 2,
        FontSize = 24,
        AlignmentX = TextAlignX.Left,
        AlignmentY = TextAlignY.Top,
    };
    public MenuScene() {
        float x = 0;
        float y = 0;

        float padding = 0.5f;

        Font font = Raylib.LoadFontEx("Assets/Game/font.ttf", 18, [], 0);;

        foreach(IBeatmapSet map in MapLoader.Maps) {
            TestUI.Children.Add(new TestMapButton() {
                Map = map,
                Size = new UDim2(0f, Raylib.GetRenderWidth() / 8 - padding * 8, 0f, Raylib.GetRenderWidth() / 8 - padding * 8),
                Position = new UDim2(0f, (Raylib.GetRenderWidth() / 8 * x) + padding, 0, (Raylib.GetRenderWidth() / 8 * y) + padding),
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
            x++;
            if(x % 8 == 0) {
                x = 0;
                y++;
            }
        }

        TestUI.Children.Add(FPSLabel);
    }

    public void Render(Window window) {
        TestUI.Render(Raylib.GetRenderWidth(), Raylib.GetRenderHeight());
    }

    public void Update(Window window, double dt) {
        FPSLabel.Text = $"FPS: {Raylib.GetFPS()}";
        foreach(UiElement element in TestUI.Children) {
            if(element is TestMapButton) {
                TestMapButton button = (TestMapButton)element;
                button.CheckPressed(window);
            }
        }
        TestUI.Update(dt);
    }
}