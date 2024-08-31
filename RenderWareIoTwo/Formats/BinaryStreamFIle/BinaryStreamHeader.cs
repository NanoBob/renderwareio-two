using RenderWareIoTwo.Formats.Common;
using RenderWareIoTwo.Formats.BinaryStreamFile.Enums;

namespace RenderWareIoTwo.Formats.BinaryStreamFile;

public class BinaryStreamHeader : IStreamReadable, IStreamWriteable
{
    public BinaryStreamChunkType Type { get; set; }
    public uint Size { get; set; }
    public uint LibraryId { get; set; } = 0x1803FFFF;

    public void WriteTo(Stream stream)
    {
        stream.WriteUint32((uint)Type);
        stream.WriteUint32(Size);
        stream.WriteUint32(LibraryId);
    }

    public void ReadFrom(Stream stream)
    {
        this.Type = (BinaryStreamChunkType)stream.ReadUint32();
        this.Size = stream.ReadUint32();
        this.LibraryId = stream.ReadUint32();
    }
}
