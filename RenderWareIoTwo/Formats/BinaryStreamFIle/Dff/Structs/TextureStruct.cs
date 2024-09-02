using RenderWareIoTwo.Formats.BinaryStreamFile;

namespace RenderWareIoTwo.Formats.BinaryStreamFIle.Dff.Structs;

public class TextureStruct : BinaryStreamStruct
{
    public TextureStruct()
    {
        this.Data = new byte[4];
    }

    public byte FilterMode
    {
        get => Data[0];
        set => Data[0] = value;
    }

    public byte UvByte
    {
        get => Data[1];
        set => Data[1] = value;
    }

    public ushort Unknown
    {
        get => BitConverter.ToUInt16(Data, 2);
        set => Data.ReplaceUint16(2, value);
    }
}