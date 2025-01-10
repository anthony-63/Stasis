using Raylib_cs;
using Stasis.Content.Beatmaps;
using Stasis.Content.Settings;

namespace Stasis.Game;

public static class Global {
    public static IBeatmapSet? SelectedMap;

    public static Settings Settings = new();

    public static Color[] Colors = [Color.White, Color.Pink];

    public static float LastScroll = 0f;
}