using Raylib_cs;
using Stasis.Content.Beatmaps;
using Stasis.Content.Settings;
using Stasis.Game.Scenes.Menu;

namespace Stasis.Game;

public static class Global {
    public static IBeatmapSet? SelectedMap;

    public static Settings Settings = new();

    public static Color[] Colors = [Color.White, Color.Pink];

    public static MenuScene? LoadedMenu = null;

    public static string UIFont = "Assets/Game/font.ttf";
}