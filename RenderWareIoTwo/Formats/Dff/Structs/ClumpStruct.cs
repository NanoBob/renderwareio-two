namespace RenderWareIoTwo.Formats.Dff;

public class ClumpStruct : DffStruct
{
    public uint AtomicCount
    {
        get => BitConverter.ToUInt32(this.Data, 0);
        set => this.Data.ReplaceUint32(0, value);
    }

    public uint LightCount
    {
        get => BitConverter.ToUInt32(this.Data, 4);
        set => this.Data.ReplaceUint32(4, value);
    }

    public uint CameraCount
    {
        get => BitConverter.ToUInt32(this.Data, 8);
        set => this.Data.ReplaceUint32(8, value);
    }
}
