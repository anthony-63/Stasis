using System.Diagnostics;
using System.Runtime.InteropServices;
using Raylib_cs;

namespace Stasis.Engine.Audio;

public class SyncAudioPlayer {
    public Music AudioStream;

    private byte[] AudioData = {};

    public float Speed = 1f;

    public float Time = 0;
    public bool Playing => Raylib.IsMusicStreamPlaying(AudioStream);

    private float LastTime = 0;

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
        Raylib.PlayMusicStream(AudioStream);
        Raylib.SeekMusicStream(AudioStream, from);
        LastTime = Now();
    }

    private static long Now() {
        long nano = 10000L * Stopwatch.GetTimestamp();
        nano /= TimeSpan.TicksPerMillisecond;
        nano *= 100L;
        return nano;
    }

    public void Update() {
        if(!Raylib.IsMusicStreamPlaying(AudioStream)) return;

        Raylib.UpdateMusicStream(AudioStream);

        Time += Raylib.GetFrameTime();
    }

    public void Sync() {
        if(ShouldSync()) {
            Console.WriteLine($"Resynced audio: {Time}");
            Raylib.SeekMusicStream(AudioStream, Time);
        }
    }

    private bool ShouldSync() {
        return Math.Abs(Time - Raylib.GetMusicTimePlayed(AudioStream)) >= 0.05;
    }
}