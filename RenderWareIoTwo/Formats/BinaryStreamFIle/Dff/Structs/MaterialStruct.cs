using RenderWareIoTwo.Formats.BinaryStreamFile;
using RenderWareIoTwo.Formats.Common;

namespace RenderWareIoTwo.Formats.BinaryStreamFIle.Dff.Structs;

public enum MaterialFlags : uint
{
    Unknown,
}

public class MaterialStruct : BinaryStreamStruct
{
    public MaterialFlags Flags
    {
        get => (MaterialFlags)BitConverter.ToUInt32(Data, 0);
        set => Data.ReplaceUint32(0, (uint)value);
    }

    public Color Color
    {
        get => new(Data[4], Data[5], Data[6], Data[7]);
        set
        {
            Data[4] = value.R;
            Data[5] = value.G;
            Data[6] = value.B;
            Data[7] = value.A;
        }
    }

    public uint Unknown
    {
        get => BitConverter.ToUInt32(Data, 8);
        set => Data.ReplaceUint32(8, value);
    }

    public bool IsTextured
    {
        get => BitConverter.ToUInt32(Data, 12) == 1;
        set => Data.ReplaceUint32(12, (uint)(value ? 1 : 0));
    }

    public float Ambient
    {
        get => BitConverter.ToSingle(Data, 16);
        set => Data.ReplaceSingle(12, value);
    }

    public float Specular
    {
        get => BitConverter.ToSingle(Data, 20);
        set => Data.ReplaceSingle(12, value);
    }

    public float Diffuse
    {
        get => BitConverter.ToSingle(Data, 24);
        set => Data.ReplaceSingle(12, value);
    }
}
