namespace RenderWareIoTwo.Formats.BinaryStreamFile.Structs;

public class TextureDictionaryStruct : BinaryStreamStruct
{
    public ushort TextureCount
    {
        get => BitConverter.ToUInt16(Data, 0);
        set => Data.ReplaceUint16(0, value);
    }

    public ushort DeviceId
    {
        get => BitConverter.ToUInt16(Data, 2);
        set => Data.ReplaceUint16(2, value);
    }
}
