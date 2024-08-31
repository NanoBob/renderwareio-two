using RenderWareIoTwo.Formats.BinaryStreamFile;

namespace RenderWareIoTwo.Formats.BinaryStreamFIle.Dff.Structs;

public class GeometryListStruct : BinaryStreamStruct
{
    public uint GeometryCount
    {
        get => BitConverter.ToUInt32(Data, 0);
        set => Data.ReplaceUint32(0, value);
    }
}
