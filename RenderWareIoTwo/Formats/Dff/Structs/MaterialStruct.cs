using RenderWareIoTwo.Formats.Common;

namespace RenderWareIoTwo.Formats.Dff;

public enum MaterialFlags : uint
{
    Unknown,
}

public class MaterialStruct : DffStruct
{
    public MaterialFlags Flags
    {
        get => (MaterialFlags)BitConverter.ToUInt32(this.Data, 0);
        set => this.Data.ReplaceUint32(0, (uint)value);
    }

    public Color Color
    {
        get => new(this.Data[4], this.Data[5], this.Data[6], this.Data[7]);
        set
        {
            this.Data[4] = value.R;
            this.Data[5] = value.G;
            this.Data[6] = value.B;
            this.Data[7] = value.A;
        }
    }

    public uint Unknown
    {
        get => BitConverter.ToUInt32(this.Data, 8);
        set => this.Data.ReplaceUint32(8, value);
    }

    public bool IsTextured
    {
        get => BitConverter.ToUInt32(this.Data, 12) == 1;
        set => this.Data.ReplaceUint32(12, (uint)(value ? 1 : 0));
    }

    public float Ambient
    {
        get => BitConverter.ToSingle(this.Data, 16);
        set => this.Data.ReplaceSingle(12, value);
    }

    public float Specular
    {
        get => BitConverter.ToSingle(this.Data, 20);
        set => this.Data.ReplaceSingle(12, value);
    }

    public float Diffuse
    {
        get => BitConverter.ToSingle(this.Data, 24);
        set => this.Data.ReplaceSingle(12, value);
    }
}
