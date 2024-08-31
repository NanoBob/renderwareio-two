using System.Numerics;
using RenderWareIoTwo.Formats.BinaryStreamFile;

namespace RenderWareIoTwo.Formats.BinaryStreamFIle.Dff.Structs;

[Flags]
public enum FrameFlags : uint
{
    Unknown,

    Default = 0xFFFFFFFF,
}

public record struct Frame(
    Vector3 Rotation1,
    Vector3 Rotation2,
    Vector3 Rotation3,
    Vector3 Position,
    uint Parent,
    FrameFlags Flags
)
{
    public const int Size = 56;
};

public class FrameListStruct : BinaryStreamStruct
{
    public uint FrameCount
    {
        get => BitConverter.ToUInt32(Data, 0);
        set => Data.ReplaceUint32(0, value);
    }

    public IEnumerable<Frame> Frames
    {
        get
        {
            var stream = new MemoryStream(Data)
            {
                Position = 4
            };

            while (stream.Position < stream.Length)
            {
                yield return new Frame(
                    Rotation1: stream.ReadVector(),
                    Rotation2: stream.ReadVector(),
                    Rotation3: stream.ReadVector(),
                    Position: stream.ReadVector(),
                    Parent: stream.ReadUint32(),
                    Flags: (FrameFlags)stream.ReadUint32()
                );
            }
        }
        set
        {
            var count = value.Count();

            var stream = new MemoryStream(4 + count * Frame.Size);

            stream.WriteUint32((uint)count);
            foreach (var frame in value)
            {
                stream.WriteVector(frame.Rotation1);
                stream.WriteVector(frame.Rotation2);
                stream.WriteVector(frame.Rotation3);
                stream.WriteVector(frame.Position);
                stream.WriteUint32(frame.Parent);
                stream.WriteUint32((uint)frame.Flags);
            }

            Data = stream.GetBuffer();
        }
    }
}
