using RenderWareIoTwo.Formats.BinaryStreamFile.Enums;

namespace RenderWareIoTwo.Formats.BinaryStreamFIle.Dff.DataChunks;

public class FrameChunk : StringChunk
{
    public FrameChunk()
    {
        this.Header.Type = BinaryStreamChunkType.Frame;
    }
}
