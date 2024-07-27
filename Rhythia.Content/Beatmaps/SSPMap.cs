using System.Reflection.Metadata.Ecma335;
using System.Text;
using Rhythia.Engine;

namespace Rhythia.Content.Beatmaps;

struct DataOffsets {
    public ulong CustomDataOffset;
    public ulong MarkerOffset;
    public ulong AudioOffset;
    public ulong AudioLength;
}

enum BlockOffsets {
    Magic = 0x0,
    Version = 0x4,
    NoteCount = 0x22,
    Difficulty = 0x2a,
    DataOffsets = 0x30,
    MarkerOffset = 0x70,
    IdOffset = 0x80,
};

class SSPMapParser {
    int Index;
    byte[] Buffer;

    DataOffsets DataOffsets;

    public SSPMapParser(byte[] buffer) {
        Buffer = buffer;
        Index = 0;
        DataOffsets = GetDataOffsets();
    }

    public DataOffsets GetDataOffsets() {
        Index = (int)BlockOffsets.DataOffsets;

        var offsets = new DataOffsets();
        
        offsets.CustomDataOffset = Read64();
        Index += 0x8; // skip custom data byte length

        offsets.AudioOffset = Read64();
        offsets.AudioLength = Read64();

        Index = (int)BlockOffsets.MarkerOffset;
        offsets.MarkerOffset = Read64();

        return offsets;
    }

    public bool VerifySignature() {
        Index = (int)BlockOffsets.Magic;
        return "SS+m"u8.ToArray().SequenceEqual([Read8(), Read8(), Read8(), Read8()]);
    }

    public ushort GetVersion() {
        Index = (int)BlockOffsets.Version;
        return Read8();
    }

    public string GetTitle() {
        Index = (int)BlockOffsets.IdOffset;
        var titleOffset = (int)BlockOffsets.IdOffset + Read16() + 0x2;
        Index = titleOffset;
        return ReadString();
    }

    public string[] GetMappers() {
        Index = (int)BlockOffsets.IdOffset;
        var titleOffset = (int)BlockOffsets.IdOffset + Read16() + 0x2;
        Index = titleOffset;
        var mappersOffset = titleOffset + Read16() + 0x2;
        Index = mappersOffset;

        ReadString(); // skip over song name

        var mapperCount = Read16();
        var mappers = new string[mapperCount];
        
        for(int i = 0; i < mapperCount; i++)
            mappers[i] = ReadString();

        return mappers;
    }

    public string GetDifficultyName() {
        Index = (int)DataOffsets.CustomDataOffset;

        var customDataCount = Read16();
        if(customDataCount < 1) return "";

        ReadString();

        var dataType = Read8();
        if(dataType == 0x09)
            return ReadString();
        else if(dataType == 0x0b)
            return ReadStringLong();
        else
            throw new FileLoadException("Invalid data type for difficulty name");
    }

    Note[] GetNotes() {
        Index = (int)BlockOffsets.NoteCount;
        var noteCount = Read32();

        Index = (int)DataOffsets.MarkerOffset;

        var notes = new Note[noteCount];

        for(int i = 0; i < noteCount; i++) {
            var time = Read32() / 1000.0f;

            Read8(); // always 1. why??? so dumb!

            var hasQuantum = Read8() == 1;
            if(hasQuantum) {
                notes[i] = new Note {
                    Time = time,
                    X = -(ReadFloat() - 1),
                    Y = -(ReadFloat() - 1),
                };
            } else {
                notes[i] = new Note {
                    Time = time,
                    X = -(Read8() - 1),
                    Y = -(Read8() - 1),
                };
            }
        }

        return notes;
    }

    public Beatmap GetBeatmapFromData() {
        return new Beatmap {
            Name = GetDifficultyName(),
            Notes = [.. GetNotes().ToList().OrderBy(n => n.Time)],
        };
    }

    public byte[] GetAudioBytes(DataOffsets offsets) {
        Index = (int)offsets.AudioOffset;
        byte[] audio = new byte[offsets.AudioLength];
        ReadExact(ref audio);
        return audio;
    }

    byte Read8() {
        return Buffer[Index++];
    }

    ushort Read16() {
        Index += 2;
        return BitConverter.ToUInt16(Buffer, Index - 2);
    }

    uint Read32() {
        Index += 4;
        return BitConverter.ToUInt32(Buffer, Index - 4);
    }

    ulong Read64() {
        Index += 8;
        return BitConverter.ToUInt64(Buffer, Index - 8);
    }

    float ReadFloat() {
        Index += 4;
        return BitConverter.ToSingle(Buffer, Index - 4);
    }

    void ReadExact(ref byte[] bytes) {
        bytes = new ArraySegment<byte>(Buffer, Index, bytes.Length).ToArray();
        Index += bytes.Length;
    }

    string ReadString() {
        var len = Read16();
        var stringBuffer = new byte[len];
        ReadExact(ref stringBuffer);
        return Encoding.UTF8.GetString(stringBuffer);
    }

    string ReadStringLong() {
        var len = Read32();
        var stringBuffer = new byte[len];
        ReadExact(ref stringBuffer);
        return Encoding.UTF8.GetString(stringBuffer);
    }
}

public class SSPMap : IBeatmapSet {
    public ushort Version { get; set; }
    public string Title { get; set; }
    public string Artist { get; set; } = "";
    public string[] Mappers { get; set; }
    public Beatmap[] Difficulties { get; set; } 
    public string Path { get; set; }
    public byte[] AudioData { get; set; } = [];

    public SSPMap(string path) {
        Logger.Info("Loading SSPM Map: ", path);

        var mapBuffer = File.ReadAllBytes(path);
        var parser = new SSPMapParser(mapBuffer);

        if(!parser.VerifySignature())
            throw new FileLoadException("Failed to verify file signature");

        Version = parser.GetVersion();
        if(Version != 2)
            throw new FileLoadException("Invalid map version, only supports sspmv2 for now.");

        Title = parser.GetTitle();
        Mappers = parser.GetMappers();
        Difficulties = [parser.GetBeatmapFromData()];
        Path = path;
        AudioData = parser.GetAudioBytes(parser.GetDataOffsets());
    }
}