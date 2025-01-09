namespace Stasis.Content.Settings;

public class NoteSettings {
    public float ApproachTime { get; set; } = 0.38f;
    public float ApproachDistance { get; set; } = 14f;

    public bool Pushback { get; set; } = false;

    // public float GetPushbackMS(float speed) {
    //     return ApproachTime * speed / ApproachDistance * Math.Min(8f, Pushback * 2);
    // }
}