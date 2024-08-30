using RenderWareIoTwo.Formats.Dff.DataChunks;
using RenderWareIoTwo.Formats.Dff.Enums;

namespace RenderWareIoTwo.Formats.Dff;

public static class DffChunkParser
{
    public static DffChunk Parse(Stream stream, DffChunkType? parent)
    {
        var header = new DffHeader();
        header.ReadFrom(stream);

        switch (header.Type)
        {
            case DffChunkType.Struct:
                return ParseStruct(header, stream, parent);

            case DffChunkType.Frame:
                var frame = new FrameChunk();
                frame.ReadFrom(stream, header);
                return frame;

            case DffChunkType.String:
                var stringChunk = new StringChunk();
                stringChunk.ReadFrom(stream, header);
                return stringChunk;

            case DffChunkType.Clump:
            case DffChunkType.FrameList:
            case DffChunkType.GeometryList:
            case DffChunkType.Geometry:
            case DffChunkType.Extension:
            case DffChunkType.MaterialList:
            case DffChunkType.Material:
            case DffChunkType.Texture:
            case DffChunkType.Atomic:
                var chunk = new DffChunk();
                chunk.ReadFrom(stream, header);
                return chunk;

        }

        var childless = new ChildlessChunk();
        childless.ReadFrom(stream, header);
        return childless;
    }

    private static DffStruct ParseStruct(DffHeader header, Stream stream, DffChunkType? parent)
    {
        switch (parent)
        {
            case DffChunkType.Atomic:
                var atomic = new AtomicStruct();
                atomic.ReadFrom(stream, header);
                return atomic;

            case DffChunkType.Clump:
                var clump = new ClumpStruct();
                clump.ReadFrom(stream, header);
                return clump;

            case DffChunkType.FrameList:
                var frameList = new FrameListStruct();
                frameList.ReadFrom(stream, header);
                return frameList;

            case DffChunkType.GeometryList:
                var geometryList = new GeometryListStruct();
                geometryList.ReadFrom(stream, header);
                return geometryList;

            case DffChunkType.Geometry:
                var geometry = new GeometryStruct();
                geometry.ReadFrom(stream, header);
                return geometry;

            case DffChunkType.Material:
                var material = new MaterialStruct();
                material.ReadFrom(stream, header);
                return material;

            case DffChunkType.Texture:
                var texture = new TextureStruct();
                texture.ReadFrom(stream, header);
                return texture;
        }

        var unknown = new UnknownDffStruct();
        unknown.ReadFrom(stream, header);
        return unknown;
    }
}
