using Raylib_cs;

namespace Stasis.Engine.Audio;

public class AudioFX {
    public Sound FX;

    private byte[] AudioData = {};


    public AudioFX(string path, float volume) {
        FX = Raylib.LoadSound(path);
        Raylib.SetSoundVolume(FX, volume);
    }

    public void Play() {
        Sound newSound = Raylib.LoadSoundAlias(FX);
        Raylib.PlaySound(newSound);
    }
}