using RenderWareIoTwo.Formats.BinaryStreamFIle.Dff.Chunks;

namespace RenderWareIoTwo.Formats.BinaryStreamFIle.Dff;

public class DffFile
{
    public Clump Clump { get; set; } = new();

    public DffFile()
    {
    }

    public DffFile(Stream stream)
    {
        Clump.ReadFrom(stream);
    }

    public void WriteTo(Stream stream)
    {
        Clump.UpdateHeaderSize();
        Clump.WriteTo(stream);
    }

    public override string ToString()
        => Clump.ToString();
}
