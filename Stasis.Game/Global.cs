using Raylib_cs;
using Stasis.Content.Beatmaps;
using Stasis.Content.Settings;
using Stasis.Engine.UI;
using Stasis.Engine.Discord;
using Stasis.Engine.UI.Elements;
using Stasis.Game.Scenes.Menu;

namespace Stasis.Game;

public static class Global {
    public static IBeatmapSet? SelectedMap;

    public static Settings Settings = new();

    public static Color[] Colors = [Color.White, Color.Pink];
    public static string UIFont = "Assets/Game/font.ttf";

    public static MenuScene? LoadedMenu = null;

    public static RPCClient Discord = new();

    public static Label BasicFPSLabel = new() {
        TextColor = Color.Lime,
        FontSize = 32,
        Text = "0 FPS",
        Position = new UDim2(0.98f, 0, 0, 5),
        AlignmentX = TextAlignX.Right,
        AlignmentY = TextAlignY.Top,
        Font = UIFont,
        OneLine = true,
    };
}