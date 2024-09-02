using RenderWareIoTwo.Formats.BinaryStreamFile;

namespace RenderWareIoTwo.Formats.BinaryStreamFIle.Dff.Structs;

public enum AtomicFlags : uint
{
    None,

    CollisionTest = 0x01,
    Render = 0x04,

    All = CollisionTest | Render
}

public class AtomicStruct : BinaryStreamStruct
{
    public AtomicStruct()
    {
        this.Data = new byte[16];
    }

    public uint FrameIndex
    {
        get => BitConverter.ToUInt32(Data, 0);
        set => Data.ReplaceUint32(0, value);
    }

    public uint GeometryIndex
    {
        get => BitConverter.ToUInt32(Data, 4);
        set => Data.ReplaceUint32(4, value);
    }

    public AtomicFlags Flags
    {
        get => (AtomicFlags)BitConverter.ToUInt32(Data, 8);
        set => Data.ReplaceUint32(8, (uint)value);
    }

    public uint Unused
    {
        get => BitConverter.ToUInt32(Data, 12);
        set => Data.ReplaceUint32(12, value);
    }
}