using System.Diagnostics;
using System.Runtime.InteropServices;
using Raylib_cs;

namespace Stasis.Engine.Audio;

public class SyncAudioPlayer {
    public Music AudioStream;

    private byte[] AudioData = {};

    public float Speed = 1f;

    public float Time = 0f;
    public bool Playing = false;

    public SyncAudioPlayer(string path, float volume, float speed = 1f) {
        AudioStream = Raylib.LoadMusicStream(path);
        Raylib.SetMusicVolume(AudioStream, volume);
        Raylib.SetMusicPitch(AudioStream, speed);
        Speed = speed;
    }

    public SyncAudioPlayer(byte[]? data, float volume, float speed = 1f) {
        if(data == null) return;
        AudioData = data ?? [];
        AudioStream = Raylib.LoadMusicStreamFromMemory(AudioUtil.GetFileFormat(AudioData), AudioData);
        Raylib.SetMusicVolume(AudioStream, volume);
        Raylib.SetMusicPitch(AudioStream, speed);
        AudioStream.Looping = false;
        Speed = speed;
    }

    public void Play(float from) {
        if(from < 0) {
            Playing = true;
            Time = from;
            return;
        }

        Raylib.PlayMusicStream(AudioStream);
        Raylib.SeekMusicStream(AudioStream, from);
        Playing = true;
        Time = Raylib.GetMusicTimePlayed(AudioStream);
    }

    public void Seek(float from) {
        Raylib.SeekMusicStream(AudioStream, from);
        Time = from;
    }

    public void Update() {
        if(!Playing) return;
        if(Time < 0) {
            Time += Raylib.GetFrameTime();
            return;
        }

        if(Time >= 0 && Playing && !Raylib.IsMusicStreamPlaying(AudioStream)) {
            Raylib.PlayMusicStream(AudioStream);
        }

        Raylib.UpdateMusicStream(AudioStream);
        Time += Raylib.GetFrameTime() * Speed;
        if(Math.Abs(Time - Raylib.GetMusicTimePlayed(AudioStream)) > 0.5f) {
            Time = Raylib.GetMusicTimePlayed(AudioStream);
        }
    }
}