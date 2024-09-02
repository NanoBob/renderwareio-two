using RenderWareIoTwo.Formats.BinaryStreamFile.Enums;
using RenderWareIoTwo.Formats.BinaryStreamFile.Structs;
using RenderWareIoTwo.Formats.BinaryStreamFIle.Dff.DataChunks;
using RenderWareIoTwo.Formats.BinaryStreamFIle.Dff.Structs;

namespace RenderWareIoTwo.Formats.BinaryStreamFile;

public static class BinaryStreamChunkBuilder
{
    public static BinaryStreamChunk Create(BinaryStreamChunkType type, bool withStruct = false)
    {
        BinaryStreamChunk? chunk = null;

        switch (type)
        {
            case BinaryStreamChunkType.Frame:
                chunk = new FrameChunk();
                break;

            case BinaryStreamChunkType.String:
                chunk = new StringChunk();
                break;

            case BinaryStreamChunkType.Clump:
            case BinaryStreamChunkType.FrameList:
            case BinaryStreamChunkType.GeometryList:
            case BinaryStreamChunkType.Geometry:
            case BinaryStreamChunkType.Extension:
            case BinaryStreamChunkType.MaterialList:
            case BinaryStreamChunkType.Material:
            case BinaryStreamChunkType.Texture:
            case BinaryStreamChunkType.Atomic:
            case BinaryStreamChunkType.TextureDictionary:
            case BinaryStreamChunkType.Raster:
                chunk = new BinaryStreamChunk();
                break;
        }

        chunk ??= new ChildlessChunk();

        chunk.Header.Type = type;

        if (withStruct)
            chunk.AddChild(CreateStruct(type));

        return chunk;
    }

    public static BinaryStreamStruct CreateStruct(BinaryStreamChunkType type)
    {
        BinaryStreamStruct chunk = type switch
        {
            BinaryStreamChunkType.Atomic => new AtomicStruct(),
            BinaryStreamChunkType.Clump => new ClumpStruct(),
            BinaryStreamChunkType.FrameList => new FrameListStruct(),
            BinaryStreamChunkType.GeometryList => new GeometryListStruct(),
            BinaryStreamChunkType.Geometry => new GeometryStruct(),
            BinaryStreamChunkType.Material => new MaterialStruct(),
            BinaryStreamChunkType.Texture => new TextureStruct(),
            BinaryStreamChunkType.TextureDictionary => new TextureDictionaryStruct(),
            BinaryStreamChunkType.Raster => new TextureNativeStruct(),
            BinaryStreamChunkType.MaterialList => new MaterialListStruct(),
            _ => new UnknownDffStruct(),
        };

        chunk.Header.Type = BinaryStreamChunkType.Struct;

        return chunk;
    }
}
