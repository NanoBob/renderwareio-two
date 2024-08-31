using RenderWareIoTwo.Formats.BinaryStreamFile;
using RenderWareIoTwo.Formats.BinaryStreamFIle.Txd.Chunks;

namespace RenderWareIoTwo.Formats.BinaryStreamFIle.Txd;

public class TxdFile
{
    public TextureDictionary TextureDictionary { get; set; } = new();
    public BinaryStreamChunk? AdditionalChunk { get; set; } = new();

    public TxdFile()
    {
    }

    public TxdFile(Stream stream)
    {
        TextureDictionary.ReadFrom(stream);
        if (stream.Position != stream.Length)
        {
            this.AdditionalChunk = new();
            this.AdditionalChunk.ReadFrom(stream);
        }
    }

    public void WriteTo(Stream stream)
    {
        TextureDictionary.UpdateHeaderSize();
        TextureDictionary.WriteTo(stream);

        if (this.AdditionalChunk != null)
            this.AdditionalChunk.WriteTo(stream);
    }

    public override string ToString()
        => TextureDictionary.ToString();
}
