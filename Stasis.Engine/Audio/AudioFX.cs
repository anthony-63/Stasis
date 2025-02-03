using System.Runtime.InteropServices;
using Raylib_cs;

namespace Stasis.Engine.Audio;

public class AudioFX {
    public const int ChannelCount = 32; 

    public Sound FX;

    public Sound[] channels = new Sound[ChannelCount];
    private int currentChannel = 0;

    public AudioFX(string path, float volume) {
        FX = Raylib.LoadSound(path);
        for(int i = 0; i < channels.Length; i++) {
            channels[i] = Raylib.LoadSoundAlias(FX);
            Raylib.SetSoundVolume(channels[i], volume);
        }
    }

    public void Play() {
        Raylib.PlaySound(channels[currentChannel % ChannelCount]);
        currentChannel++;
    }
}