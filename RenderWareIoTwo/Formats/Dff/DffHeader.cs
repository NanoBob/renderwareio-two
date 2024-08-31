using RenderWareIoTwo.Formats.Common;
using RenderWareIoTwo.Formats.Dff.Enums;

namespace RenderWareIoTwo.Formats.Dff;

public class DffHeader : IStreamReadable, IStreamWriteable
{
    public DffChunkType Type { get; set; }
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
        this.Type = (DffChunkType)stream.ReadUint32();
        this.Size = stream.ReadUint32();
        this.LibraryId = stream.ReadUint32();
    }
}
