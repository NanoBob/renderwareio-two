namespace RenderWareIoTwo.Formats.Dff;

public class TextureStruct : DffStruct
{
    public byte FilterMode
    {
        get => this.Data[0];
        set => this.Data[0] = value;
    }

    public byte UvByte
    {
        get => this.Data[1];
        set => this.Data[1] = value;
    }

    public ushort Unknown
    {
        get => BitConverter.ToUInt16(this.Data, 2);
        set => this.Data.ReplaceUint32(2, value);
    }
}