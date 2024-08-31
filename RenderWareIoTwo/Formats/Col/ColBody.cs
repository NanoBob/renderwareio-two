using System.Diagnostics;
using System.Numerics;
using System.Reflection.PortableExecutable;
using RenderWareIoTwo.Formats.Col.BodyStructs;
using RenderWareIoTwo.Formats.Col.Enums;

namespace RenderWareIoTwo.Formats.Col;

public struct Box
{
    public Vector3 Min { get; set; }
    public Vector3 Max { get; set; }
    public ColSurface Surface { get; set; }
}

public struct FaceGroup
{
    public Vector3 Min { get; set; }
    public Vector3 Max { get; set; }
    public ushort StartFace { get; set; }
    public ushort EndFace { get; set; }
}

public struct Face
{
    public uint A { get; set; }
    public uint B { get; set; }
    public uint C { get; set; }
    public MaterialId Material { get; set; }
    public byte Light { get; set; }

    public ColSurface? Surface { get; set; }
}

public class ColBody
{
    // version 1+
    public List<ColSphere> Spheres { get; set; } = [];
    public List<Box> Boxes { get; set; } = [];
    public List<ColVertex> Vertices { get; set; } = [];

    public List<Face> Faces { get; set; } = [];

    // version 2+
    public List<FaceGroup> FaceGroups { get; set; } = [];

    // version 3
    public List<ColVertex> ShadowMeshVertices { get; set; } = [];
    public List<Face> ShadowMeshFaces { get; set; } = [];


    public void ReadFrom(Stream stream, ColHeader header)
    {
        if (header.ColVersion == 1)
            ReadVersion1(stream, header);
        else
        {
            if (header.ColVersion >= 2)
                ReadVersion2(stream, header);
            if (header.ColVersion >= 3)
                ReadVersion3(stream, header);
        }
    }

    private void ReadVersion1(Stream stream, ColHeader header)
    {
        var sphereCount = stream.ReadUint32();
        for (int i = 0; i < sphereCount; i++)
        {
            var sphere = new ColSphere() { Version = header.ColVersion };
            sphere.Read(stream);
            this.Spheres.Add(sphere);
        }

        var unknownUnusedCount = stream.ReadUint32();

        var boxCount = stream.ReadUint32();
        for (int i = 0; i < boxCount; i++)
        {
            var min = stream.ReadVector();
            var max = stream.ReadVector();
            var surface = new ColSurface();
            surface.Read(stream);

            this.Boxes.Add(new Box()
            {
                Min = min,
                Max = max,
                Surface = surface
            });
        }

        var vertexCount = stream.ReadUint32();
        for (int i = 0; i < vertexCount; i++)
        {
            var vertex = new ColVertex() { Version = header.ColVersion };
            vertex.Read(stream);
            this.Vertices.Add(vertex);
        }

        var faceCount = stream.ReadUint32();
        for (int i = 0; i < faceCount; i++)
        {
            var a = stream.ReadUint32();
            var b = stream.ReadUint32();
            var c = stream.ReadUint32();
            var surface = new ColSurface();
            surface.Read(stream);

            this.Faces.Add(new Face()
            {
                A = a,
                B = b,
                C = c,
                Surface = surface
            });
        }
    }

    private void ReadVersion2(Stream stream, ColHeader header)
    {
        JumpTo(stream, header, header.SphereOffset);
        for (int i = 0; i < header.SphereCount; i++)
        {
            var sphere = new ColSphere() { Version = header.ColVersion };
            sphere.Read(stream);
            this.Spheres.Add(sphere);
        }

        JumpTo(stream, header, header.BoxOffset);
        for (int i = 0; i < header.BoxCount; i++)
        {
            var min = stream.ReadVector();
            var max = stream.ReadVector();
            var surface = new ColSurface();
            surface.Read(stream);

            this.Boxes.Add(new Box()
            {
                Min = min,
                Max = max,
                Surface = surface
            });
        }

        JumpTo(stream, header, header.FaceOffset);
        if (header.Flags.HasFlag(ColllisionFlags.HasFaceGroups))
        {
            var faceGroupCount = stream.ReadUint32();
            stream.Position = header.ReferencePosition + header.FaceOffset - faceGroupCount * 28;

            for (int i = 0; i < faceGroupCount; i++)
            {
                var faceGroup = new FaceGroup()
                {
                    Min = stream.ReadVector(),
                    Max = stream.ReadVector(),
                    StartFace = stream.ReadUint16(),
                    EndFace = stream.ReadUint16()
                };
                this.FaceGroups.Add(faceGroup);
            }
        }

        for (int i = 0; i < header.FaceCount; i++)
        {
            this.Faces.Add(new Face()
            {
                A = stream.ReadUint16(),
                B = stream.ReadUint16(),
                C = stream.ReadUint16(),
                Material = (MaterialId)stream.ReadSingleByte(),
                Light = stream.ReadSingleByte()
            });
        }

        JumpTo(stream, header, header.VertexOffset);
        var vertexCount = this.Faces.Any() ? this.Faces.Max(x => Math.Max(Math.Max(x.A, x.B), x.C)) + 1 : 0;
        for (int i = 0; i <  vertexCount; i++)
        {
            var vertex = new ColVertex() { Version = header.ColVersion };
            vertex.Read(stream);
            this.Vertices.Add(vertex);
        }

        JumpTo(stream, header, header.Size + 4);
    }

    private void ReadVersion3(Stream stream, ColHeader header)
    {
        if (!header.Flags.HasFlag(ColllisionFlags.HasShadowMesh))
            return;

        JumpTo(stream, header, header.ShadowMeshVertexOffset);
        var shadowVertexCount = (header.ShadowMeshFaceOffset - header.ShadowMeshVertexOffset) / 6;
        for (int i = 0; i < shadowVertexCount; i++)
        {
            var vertex = new ColVertex() { Version = header.ColVersion };
            vertex.Read(stream);
            this.ShadowMeshVertices.Add(vertex);
        }

        JumpTo(stream, header, header.ShadowMeshFaceOffset);
        for (int i = 0; i < header.ShadowMeshFaceCount; i++)
        {
            this.ShadowMeshFaces.Add(new Face()
            {
                A = stream.ReadUint16(),
                B = stream.ReadUint16(),
                C = stream.ReadUint16(),
                Material = (MaterialId)stream.ReadSingleByte(),
                Light = stream.ReadSingleByte()
            });
        }

        JumpTo(stream, header, header.Size + 4);
    }

    private void JumpTo(Stream stream, ColHeader header, long position)
    {
        if (stream.Position != header.ReferencePosition + position)
        {
            Debug.WriteLine($"Position mismatch when reading vertices, expected {header.ReferencePosition + position} got {stream.Position}");
            stream.Position = header.ReferencePosition + position;
        }
    }

    public void WriteTo(Stream stream, ColHeader header)
    {
        if (header.ColVersion == 1)
            WriteVersion1(stream, header);
        else
        {
            if (header.ColVersion >= 2)
                WriteVersion2(stream, header);
            if (header.ColVersion >= 3)
                WriteVersion3(stream, header);
        }
    }

    private void WriteVersion1(Stream stream, ColHeader header)
    {
        stream.WriteUint32((uint)this.Spheres.Count);
        foreach (var sphere in this.Spheres)
        {
            var writeCopy = sphere;
            writeCopy.Version = header.ColVersion;
            writeCopy.Write(stream);
        }

        stream.WriteUint32(0);

        stream.WriteUint32((uint)this.Boxes.Count);
        foreach (var box in this.Boxes)
        {
            stream.WriteVector(box.Min);
            stream.WriteVector(box.Max);
            box.Surface.Write(stream);
        }

        stream.WriteUint32((uint)this.Vertices.Count);
        foreach (var vertex in this.Vertices)
        {
            var writeCopy = vertex;
            writeCopy.Version = header.ColVersion;
            writeCopy.Write(stream);
        }

        stream.WriteUint32((uint)this.Faces.Count);
        foreach (var face in this.Faces)
        {
            if (face.Surface == null)
                throw new Exception("Faces need a surface for COL1 / COLL files");

            stream.WriteUint32(face.A);
            stream.WriteUint32(face.B);
            stream.WriteUint32(face.C);
            face.Surface.Value.Write(stream);
        }
    }

    private void WriteVersion2(Stream stream, ColHeader header)
    {
        foreach (var sphere in this.Spheres)
        {
            var writeCopy = sphere;
            writeCopy.Version = header.ColVersion;
            writeCopy.Write(stream);
        }

        foreach (var box in this.Boxes)
        {
            stream.WriteVector(box.Min);
            stream.WriteVector(box.Max);
            box.Surface.Write(stream);
        }

        foreach (var vertex in this.Vertices)
        {
            var writeCopy = vertex;
            writeCopy.Version = header.ColVersion;
            writeCopy.Write(stream);
        }

        if ((this.Vertices.Count * 6) % 4 != 0)
        {
            stream.WriteChars(['\0', '\0']);
        }

        if (header.Flags.HasFlag(ColllisionFlags.HasFaceGroups))
        {
            stream.WriteUint32((uint)this.FaceGroups.Count);
            foreach (var faceGroup in this.FaceGroups)
            {
                stream.WriteVector(faceGroup.Min);
                stream.WriteVector(faceGroup.Max);
                stream.WriteUint16(faceGroup.StartFace);
                stream.WriteUint16(faceGroup.EndFace);
            }
        }

        foreach (var face in this.Faces)
        {
            stream.WriteUint16((ushort)face.A);
            stream.WriteUint16((ushort)face.B);
            stream.WriteUint16((ushort)face.C);
            stream.WriteByte((byte)face.Material);
            stream.WriteByte(face.Light);
        }
    }

    private void WriteVersion3(Stream stream, ColHeader header)
    {
        foreach (var shadowMeshVertex in this.ShadowMeshVertices)
        {
            var writeCopy = shadowMeshVertex;
            writeCopy.Version = header.ColVersion;
            writeCopy.Write(stream);
        }

        if ((this.ShadowMeshVertices.Count * 6) % 4 != 0)
        {
            stream.WriteChars(['\0', '\0']);
        }

        foreach (var face in this.ShadowMeshFaces)
        {
            stream.WriteUint32(face.A);
            stream.WriteUint32(face.B);
            stream.WriteUint32(face.C);
            stream.WriteByte((byte)face.Material);
            stream.WriteByte(face.Light);
        }
    }

    public int GetSize(int version)
    {
        switch (version)
        {
            case 1:
                return
                    4 + this.Spheres.Count * 20 +
                    4 +
                    4 + this.Boxes.Count * 28 +
                    4 + this.Vertices.Count * 12 +
                    4 + this.Faces.Count * 16;

            case 2:
                return 
                    this.Spheres.Count * 20 +
                    this.Boxes.Count * 28 +
                    this.Vertices.Count * 6 +
                    ((((this.Vertices.Count * 6) % 4) != 0) ? 2 : 0) +
                    this.FaceGroups.Count * 28 +
                    (this.FaceGroups.Count > 0 ? 4 : 0) +
                    this.Faces.Count * 8;

            case 3:
                return
                    GetSize(2) +
                    this.ShadowMeshVertices.Count * 6 +
                    ((((this.ShadowMeshVertices.Count * 6) % 4) != 0) ? 2 : 0) +
                    this.ShadowMeshFaces.Count * 8;

            case 4:
                return
                    GetSize(3) + 4;

            default:
                throw new Exception($"Unsupported COL version COL{version}");
        }
    }
}
