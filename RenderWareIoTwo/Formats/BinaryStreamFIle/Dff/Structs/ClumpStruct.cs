using RenderWareIoTwo.Formats.BinaryStreamFile;

namespace RenderWareIoTwo.Formats.BinaryStreamFIle.Dff.Structs;

public class ClumpStruct : BinaryStreamStruct
{
    public uint AtomicCount
    {
        get => BitConverter.ToUInt32(Data, 0);
        set => Data.ReplaceUint32(0, value);
    }

    public uint LightCount
    {
        get => BitConverter.ToUInt32(Data, 4);
        set => Data.ReplaceUint32(4, value);
    }

    public uint CameraCount
    {
        get => BitConverter.ToUInt32(Data, 8);
        set => Data.ReplaceUint32(8, value);
    }
}
