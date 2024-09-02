using RenderWareIoTwo.Formats.BinaryStreamFile;
using RenderWareIoTwo.Formats.BinaryStreamFile.Enums;

namespace RenderWareIoTwo.Formats.BinaryStreamFIle.Dff.Chunks;

public class Clump : BinaryStreamChunk
{
    public Clump()
    {
        this.Header.Type = BinaryStreamChunkType.Clump;
    }
}
