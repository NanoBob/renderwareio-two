using System.Numerics;
using RenderWareIoTwo.Formats.BinaryStreamFile;
using RenderWareIoTwo.Formats.Common;

namespace RenderWareIoTwo.Formats.BinaryStreamFIle.Dff.Structs;

[Flags]
public enum GeometryFlags : uint
{
    Unknown,

    IsTriangleStrip = 0x01,
    HasVertexTranslation = 0x02,
    HasUv = 0x04,
    IsPrelit = 0x08,
    HasNormals = 0x10,
    IsLit = 0x20,
    ModuleMaterialColor = 0x40,
    HasOtherMoreDifferentUv = 0x80,
    HasNativeGeometry = 0x01000000,
}

public enum Material : ushort
{
    Unknown = 0,
}

public record struct Uv(float X, float Y)
{
    public const int Size = 8;
}

public record struct Triangle(ushort VertexIndex2, ushort VertexIndex1, ushort MaterialIndex, ushort VertexIndex3)
{
    public const int Size = 8;
}

public record struct Sphere(Vector3 Position, float Radius)
{
    public const int Size = 16;
}

public record struct MorphTarget(Sphere Sphere, bool HasPosition, bool HasNormals, List<Vector3> Vertices, List<Vector3> Normals);


public class GeometryStruct : BinaryStreamStruct
{
    public GeometryFlags Flags { get; set; }
    public uint TriangleCount { get; set; }
    public uint VertexCount { get; set; }
    public uint MorphTargetCount { get; set; }
    public List<Color> Colors { get; set; } = [];
    public List<List<Uv>> UvLayers { get; set; } = [];
    public List<Triangle> Triangles { get; set; } = [];
    public List<MorphTarget> MorphTargets { get; set; } = [];


    public override void WriteTo(Stream stream)
    {
        Header.WriteTo(stream);

        stream.WriteUint32((uint)Flags | (uint)UvLayers.Count << 16);
        stream.WriteUint32(TriangleCount);
        stream.WriteUint32(VertexCount);
        stream.WriteUint32(MorphTargetCount);

        if (Flags.HasFlag(GeometryFlags.IsPrelit))
            foreach (var color in Colors)
            {
                stream.WriteByte(color.R);
                stream.WriteByte(color.G);
                stream.WriteByte(color.B);
                stream.WriteByte(color.A);
            }

        foreach (var layer in UvLayers)
        {
            foreach (var uv in layer)
            {
                stream.WriteFloat(uv.X);
                stream.WriteFloat(uv.Y);
            }
        }

        foreach (var triangle in Triangles)
        {
            stream.WriteUint16(triangle.VertexIndex2);
            stream.WriteUint16(triangle.VertexIndex1);
            stream.WriteUint16(triangle.MaterialIndex);
            stream.WriteUint16(triangle.VertexIndex3);
        }

        foreach (var morphTarget in MorphTargets)
        {
            stream.WriteVector(morphTarget.Sphere.Position);
            stream.WriteFloat(morphTarget.Sphere.Radius);

            stream.WriteUint32((uint)(morphTarget.HasPosition ? 1 : 0));
            stream.WriteUint32((uint)(morphTarget.HasNormals ? 1 : 0));

            if (morphTarget.HasPosition)
                foreach (var vertex in morphTarget.Vertices)
                    stream.WriteVector(vertex);

            if (morphTarget.HasNormals)
                foreach (var normal in morphTarget.Normals)
                    stream.WriteVector(normal);
        }
    }

    public override void ReadFrom(Stream stream, BinaryStreamHeader? header = null)
    {
        if (header == null)
            Header.ReadFrom(stream);
        else
            Header = header;

        var start = stream.Position;
        var size = Header.Size;

        ReadPosition = start;

        var flagData = stream.ReadUint32();
        Flags = (GeometryFlags)(flagData & 0xFF00FFFF);
        TriangleCount = stream.ReadUint32();
        VertexCount = stream.ReadUint32();
        MorphTargetCount = stream.ReadUint32();

        if (!Flags.HasFlag(GeometryFlags.HasNativeGeometry))
        {
            if (Flags.HasFlag(GeometryFlags.IsPrelit))
                for (int i = 0; i < VertexCount; i++)
                    Colors.Add(new Color(
                        stream.ReadSingleByte(),
                        stream.ReadSingleByte(),
                        stream.ReadSingleByte(),
                        stream.ReadSingleByte()
                    ));

            if (Flags.HasFlag(GeometryFlags.HasUv) || Flags.HasFlag(GeometryFlags.HasOtherMoreDifferentUv))
            {
                var textureCount = (flagData & 0x00FF0000) >> 16;
                if (textureCount == 0)
                    textureCount =
                        Flags.HasFlag(GeometryFlags.HasUv) ? 1u :
                        Flags.HasFlag(GeometryFlags.HasOtherMoreDifferentUv) ? 2u
                        : 0u;

                for (int textureLayer = 0; textureLayer < textureCount; textureLayer++)
                {
                    var layer = new List<Uv>();
                    UvLayers.Add(layer);
                    for (int i = 0; i < VertexCount; i++)
                        layer.Add(new Uv(
                            stream.ReadFloat(),
                            stream.ReadFloat()
                        ));
                }
            }

            for (int i = 0; i < TriangleCount; i++)
                Triangles.Add(new Triangle(
                    stream.ReadUint16(),
                    stream.ReadUint16(),
                    stream.ReadUint16(),
                    stream.ReadUint16()
                ));
        }

        for (int i = 0; i < MorphTargetCount; i++)
        {
            var sphere = new Sphere(stream.ReadVector(), stream.ReadFloat());
            var hasVertices = stream.ReadUint32() != 0;
            var hasNormals = stream.ReadUint32() != 0;

            var vertices = new List<Vector3>();
            if (hasVertices)
                for (int j = 0; j < VertexCount; j++)
                    vertices.Add(stream.ReadVector());

            var normals = new List<Vector3>();
            if (hasNormals)
                for (int j = 0; j < VertexCount; j++)
                    normals.Add(stream.ReadVector());

            MorphTargets.Add(new MorphTarget(sphere, hasVertices, hasNormals, vertices, normals));
        }

        if (stream.Position != start + size)
            throw new Exception($"Haven't read full geometry struct, {start + size - stream.Position} bytes left");
    }

    public override void UpdateHeaderSize()
    {
        Header.Size = (uint)(
            4 + 4 + 4 + 4 +
            (Flags.HasFlag(GeometryFlags.IsPrelit) ? (Colors.Count * 4) : 0) +
            UvLayers.Sum(x => x.Count * 8) +
            TriangleCount * 8 +
            MorphTargets.Sum(x =>
                12 + 4 +
                4 + 4 +
                x.Vertices.Count * 12 +
                x.Normals.Count * 12
            )
        );
    }
}
