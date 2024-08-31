using RenderWareIoTwo.Formats.Col.Enums;

namespace RenderWareIoTwo.Formats.Col.BodyStructs;

public struct ColSurface
{
    public MaterialId Material { get; set; }
    public byte Flag { get; set; }
    public byte Brightness { get; set; }
    public byte Light { get; set; }

    public void Read(Stream stream)
    {
        this.Material = (MaterialId)stream.ReadSingleByte();
        this.Flag = stream.ReadSingleByte();
        this.Brightness = stream.ReadSingleByte();
        this.Light = stream.ReadSingleByte();
    }

    public readonly void Write(Stream stream)
    {
        stream.WriteByte((byte)this.Material);
        stream.WriteByte(this.Flag);
        stream.WriteByte(this.Brightness);
        stream.WriteByte(this.Light);
    }
}