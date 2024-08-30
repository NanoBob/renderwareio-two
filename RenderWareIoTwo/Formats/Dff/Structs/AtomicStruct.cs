namespace RenderWareIoTwo.Formats.Dff;

public enum AtomicFlags : uint
{
    None,
    CollisionTest = 0x01,
    Render = 0x04,
}

public class AtomicStruct : DffStruct
{
    public uint FrameIndex
    {
        get => BitConverter.ToUInt32(this.Data, 0);
        set => this.Data.ReplaceUint32(0, value);
    }

    public uint GeometryIndex
    {
        get => BitConverter.ToUInt32(this.Data, 4);
        set => this.Data.ReplaceUint32(4, value);
    }

    public AtomicFlags Flags
    {
        get => (AtomicFlags)BitConverter.ToUInt32(this.Data, 8);
        set => this.Data.ReplaceUint32(8, (uint)value);
    }

    public uint Unused
    {
        get => BitConverter.ToUInt32(this.Data, 12);
        set => this.Data.ReplaceUint32(12, value);
    }
}