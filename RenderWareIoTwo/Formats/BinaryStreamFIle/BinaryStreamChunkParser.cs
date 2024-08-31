using RenderWareIoTwo.Formats.BinaryStreamFile.Enums;
using RenderWareIoTwo.Formats.BinaryStreamFile.Structs;
using RenderWareIoTwo.Formats.BinaryStreamFIle.Dff.DataChunks;
using RenderWareIoTwo.Formats.BinaryStreamFIle.Dff.Structs;

namespace RenderWareIoTwo.Formats.BinaryStreamFile;

public static class BinaryStreamChunkParser
{
    public static BinaryStreamChunk Parse(Stream stream, BinaryStreamChunkType? parent)
    {
        var header = new BinaryStreamHeader();
        header.ReadFrom(stream);

        switch (header.Type)
        {
            case BinaryStreamChunkType.Struct:
                return ParseStruct(header, stream, parent);

            case BinaryStreamChunkType.Frame:
                var frame = new FrameChunk();
                frame.ReadFrom(stream, header);
                return frame;

            case BinaryStreamChunkType.String:
                var stringChunk = new StringChunk();
                stringChunk.ReadFrom(stream, header);
                return stringChunk;

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
                var chunk = new BinaryStreamChunk();
                chunk.ReadFrom(stream, header);
                return chunk;

        }

        var childless = new ChildlessChunk();
        childless.ReadFrom(stream, header);
        return childless;
    }

    private static BinaryStreamStruct ParseStruct(BinaryStreamHeader header, Stream stream, BinaryStreamChunkType? parent)
    {
        switch (parent)
        {
            case BinaryStreamChunkType.Atomic:
                var atomic = new AtomicStruct();
                atomic.ReadFrom(stream, header);
                return atomic;

            case BinaryStreamChunkType.Clump:
                var clump = new ClumpStruct();
                clump.ReadFrom(stream, header);
                return clump;

            case BinaryStreamChunkType.FrameList:
                var frameList = new FrameListStruct();
                frameList.ReadFrom(stream, header);
                return frameList;

            case BinaryStreamChunkType.GeometryList:
                var geometryList = new GeometryListStruct();
                geometryList.ReadFrom(stream, header);
                return geometryList;

            case BinaryStreamChunkType.Geometry:
                var geometry = new GeometryStruct();
                geometry.ReadFrom(stream, header);
                return geometry;

            case BinaryStreamChunkType.Material:
                var material = new MaterialStruct();
                material.ReadFrom(stream, header);
                return material;

            case BinaryStreamChunkType.Texture:
                var texture = new TextureStruct();
                texture.ReadFrom(stream, header);
                return texture;

            case BinaryStreamChunkType.TextureDictionary:
                var textureDictionary = new TextureDictionaryStruct();
                textureDictionary.ReadFrom(stream, header);
                return textureDictionary;

            case BinaryStreamChunkType.Raster:
                var textureNative = new TextureNativeStruct();
                textureNative.ReadFrom(stream, header);
                return textureNative;

        }

        var unknown = new UnknownDffStruct();
        unknown.ReadFrom(stream, header);
        return unknown;
    }
}
