namespace RenderWareIoTwo.Formats.Img;

public class ImgDirectoryEntry
{
    public uint Offset { get; set; }
    public ushort StreamingSize { get; set; }
    public ushort Size { get; set; }
    public string Name { get; set; } = "";

    public void ReadFrom(Stream stream)
    {
        this.Offset = stream.ReadUint32();
        this.StreamingSize = stream.ReadUint16();
        this.Size = stream.ReadUint16();
        this.Name = new string(stream.ReadChars(24)).Split('\0')[0];
    }

    public void WriteTo(Stream stream)
    {
        if (this.Name.Length > 24)
            throw new Exception("An img archive directory entry may be at most 24 characters long.");

        stream.WriteUint32(this.Offset);
        stream.WriteUint16(this.StreamingSize);
        stream.WriteUint16(this.Size);
        stream.WriteChars(this.Name.PadRight(24, '\0').ToCharArray());
    }
}
