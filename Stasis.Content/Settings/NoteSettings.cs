namespace Stasis.Content.Settings;

public class NoteSettings {
    public float ApproachTime { get; set; } = 0.38f;
    public float ApproachDistance { get; set; } = 14f;

    public bool Pushback { get; set; } = false;

    public float FadeIn { get; set; } = 0f;
    public bool HalfGhost { get; set; } = false;

    public uint[] Colors { get; set; } = [15294303, 15240543, 15252063, 15263839, 12249183, 9300063, 6285407, 6285453, 6285498, 6285544, 6273768, 6262248, 6250472, 9265128, 12214248, 15228904, 15228836, 15228813];
}