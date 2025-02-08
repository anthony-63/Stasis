using System.Numerics;
using Stasis.Engine;
using Stasis.Engine.UI.Elements;

namespace Stasis.Content.Replays;

public enum ReplayFrameMeta {
    NORMAL,
    FAILED,
}

public class ReplayFrame {
    public Vector2 CursorPosition;
    public float Time;
    public ReplayFrameMeta Meta;
}

public class Replay {
    public List<ReplayFrame> Frames = new();

    public void Export(string path) {
        if(!Directory.Exists(Path.GetDirectoryName(path))) Directory.CreateDirectory(Path.GetDirectoryName(path) ?? "");
        var stream = new FileStream(path, FileMode.CreateNew);

        stream.Write(BitConverter.GetBytes((long)Frames.Count));
        foreach(ReplayFrame frame in Frames) {
            stream.Write(BitConverter.GetBytes(frame.CursorPosition.X));
            stream.Write(BitConverter.GetBytes(frame.CursorPosition.Y));
            stream.Write(BitConverter.GetBytes(frame.Time));
            stream.Write(BitConverter.GetBytes((int)frame.Meta));
        }
    }

    public ReplayFrame GetFrame(FileStream stream) {
        var dataBuffer4 = new byte[4];

        stream.Read(dataBuffer4);
        var cursorX = BitConverter.ToSingle(dataBuffer4);
        stream.Read(dataBuffer4);
        var cursorY = BitConverter.ToSingle(dataBuffer4);

        stream.Read(dataBuffer4);
        var time = BitConverter.ToSingle(dataBuffer4);

        stream.Read(dataBuffer4);
        var meta = BitConverter.ToInt32(dataBuffer4);
        
        return new ReplayFrame() {
            CursorPosition = new Vector2(cursorX, cursorY),
            Time = time,
            Meta = (ReplayFrameMeta)meta,
        };
    }

    public void SaveFrame(Vector2 cursorPosition, float time, bool failed) {
        Frames.Add(new ReplayFrame() {
            CursorPosition = cursorPosition,
            Time = time,
            Meta = failed ? ReplayFrameMeta.FAILED : ReplayFrameMeta.NORMAL,
        });
    }

    public Replay() {}

    public Replay(string path) {
        using var stream = new FileStream(path, FileMode.Open);
        var dataBuffer8 = new byte[8];
        stream.Read(dataBuffer8);
        var frameCount = BitConverter.ToInt64(dataBuffer8);
        Frames = [];
        for(int i = 0; i < frameCount; i++) {
            Frames.Add(GetFrame(stream));
        }
    }
}