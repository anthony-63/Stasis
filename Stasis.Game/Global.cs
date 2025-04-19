using Raylib_cs;
using Stasis.Content.Beatmaps;
using Stasis.Content.Settings;
using Stasis.Engine.UI;
using Stasis.Engine.Discord;
using Stasis.Engine.UI.Elements;
using Stasis.Game.Scenes.Menu;
using Stasis.Game.Scenes.Game.Player;
using System.Numerics;
using Stasis.Engine;
using Stasis.Content.Replays;

namespace Stasis.Game;

public class Mods {
    public float Speed = 1f;
    public float StartFrom = 0f;
    public bool NoFail = false;
    public bool VisualMap = false;

}

public static class Global {
    public static IBeatmapSet? SelectedMap;

    public static Settings Settings = new();

    public static string UIFont = GetAsset("Assets/Game/font.ttf");

    public static Mods Mods = new();

    public static MenuScene? LoadedMenu = null;
    public static bool EnableDebugStats = false;

    public static RPCClient Discord = new();

    public static Replay? Replay = null;

    public static string GetMapHash(IBeatmapSet map) {
        return Util.GetSHA256(map.Title + string.Concat(map.Difficulties[0].Notes.Select(x => x.X + x.Y + x.Time) ?? []));
    }
    public static Random Random = new Random();

    public static string GetAsset(string path, bool draw = true) {
        if(!File.Exists(path)) {
            Logger.Info("Downloading ", path);
            if(draw) {
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.Black);
                Raylib.DrawTextEx(Raylib.LoadFontEx(UIFont, 50, [], 0), "Downloading " + path, new Vector2(20, 20), 50, 1, Color.Green);
                Raylib.EndDrawing();
            }
            using var client = new HttpClient();
            using var s = client.GetStreamAsync("https://github.com/anthony-63/StasisDefaultAssets/raw/refs/heads/master/" + path.Replace("Assets/", ""));
            Directory.CreateDirectory(Path.GetDirectoryName(path) ?? "");
            using var fs = new FileStream(path, FileMode.CreateNew);

            s.Result.CopyTo(fs);
        }
        return path;
    }

    public static string GetModText(Mods mods, bool alwaysIncldueSpeed = false) {
        List<string> modList = [];

        if(mods.Speed != 1f || alwaysIncldueSpeed) modList.Add(mods.Speed.ToString("0.00") + "x");
        if(mods.NoFail) modList.Add("No Fail");
        if(mods.VisualMap) modList.Add("Visual Mode");
        if(Replay is not null) modList.Add("Replay");

        return string.Join(", ", modList);
    }

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