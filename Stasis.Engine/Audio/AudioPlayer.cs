using Raylib_cs;

namespace Stasis.Engine.Audio;

public class AudioPlayer {
    public Music AudioStream;

    private byte[] AudioData = {};

    public float Time => Raylib.GetMusicTimePlayed(AudioStream);
    public bool Playing => Raylib.IsMusicStreamPlaying(AudioStream);

    public float Speed = 1f;

    public AudioPlayer(string path, float volume, float speed = 1f) {
        AudioStream = Raylib.LoadMusicStream(path);
        Raylib.SetMusicVolume(AudioStream, volume);
        Raylib.SetMusicPitch(AudioStream, speed);
        Speed = speed;
    }

    public AudioPlayer(byte[] data, float volume, float speed = 1f) {
        AudioData = data;
        AudioStream = Raylib.LoadMusicStreamFromMemory(AudioUtil.GetFileFormat(AudioData), AudioData);
        Raylib.SetMusicVolume(AudioStream, volume);
        Raylib.SetMusicPitch(AudioStream, speed);
        Speed = speed;
    }

    public void Play(float from) {
        Raylib.PlayMusicStream(AudioStream);
        Raylib.SeekMusicStream(AudioStream, from);
    }

    public void Seek(float from) {
        Raylib.SeekMusicStream(AudioStream, from);
    }

    public void Update() {
        Raylib.UpdateMusicStream(AudioStream);
    }
}