using RenderWareIoTwo.Formats.Common;
using System.Numerics;

namespace RenderWareIoTwo.Formats.Col;

[Flags]
public enum ColllisionFlags : uint
{
    None = 0,
    UseConesInsteadOfLines = 0x01,
    IsNotEmpty = 0x02,
    HasFaceGroups = 0x08,
    HasShadowMesh = 0x16,
}

public class ColHeader : IStreamReadable, IStreamWriteable
{
    // version 1
    public char[] FourCC { get; set; } = ['C', 'O', 'L', '3'];
    public uint Size { get; set; }
    public char[] Name { get; set; } = new char[22];
    public short ModelId { get; set; }

    public float BoundingRadius { get; set; }
    public Vector3 BoundingCenter { get; set; }
    public Vector3 BoundingMin { get; set; }
    public Vector3 BoundingMax { get; set; }

    // version 2
    public ushort SphereCount { get; set; }
    public ushort BoxCount { get; set; }
    public ushort FaceCount { get; set; }
    public byte LineCount { get; set; }

    public byte Padding { get; set; }
    public ColllisionFlags Flags { get; set; }

    public uint SphereOffset { get; set; }
    public uint BoxOffset { get; set; }
    public uint LineOffset { get; set; }
    public uint VertexOffset { get; set; }
    public uint FaceOffset { get; set; }
    public uint TrianglePlaneOffset { get; set; }

    // version 3
    public uint ShadowMeshFaceCount { get; set; }
    public uint ShadowMeshVertexOffset { get; set; }
    public uint ShadowMeshFaceOffset { get; set; }

    // version 4
    public uint Unknown { get; set; }

    public long ReferencePosition { get; set; }
    public long SizeReferencePosition { get; set; }

    public byte ColVersion
    {
        get
        {
            if (this.FourCC[0] != 'C' || this.FourCC[1] != 'O' || this.FourCC[2] != 'L')
                throw new Exception($"Malformed COL file, four CC reads \"{string.Join(" ", this.FourCC)}\"");

            var value = this.FourCC[3];
            return value switch
            {
                'L' => 1,
                '2' => 2,
                '3' => 3,
                '4' => 4,
                _ => throw new Exception($"Unsupported COL version \"COL{value}\"")
            };
        }
        set
        {
            this.FourCC[3] = value switch
            {
                1 => 'L',
                2 => '2',
                3 => '3',
                4 => '4',
                _ => throw new Exception($"Unsupported COL version \"COL{value}\"")
            };
        }
    }

    public void ReadFrom(Stream stream)
    {
        this.FourCC = stream.ReadChars(4);
        this.ReferencePosition = stream.Position;
        this.Size = stream.ReadUint32();
        this.SizeReferencePosition = stream.Position;

        this.Name = stream.ReadChars(22);
        this.ModelId = (short)stream.ReadUint16();

        if (this.ColVersion == 1)
        {
            this.BoundingRadius = stream.ReadFloat();
            this.BoundingCenter = stream.ReadVector();
            this.BoundingMin = stream.ReadVector();
            this.BoundingMax = stream.ReadVector();
        }
        else
        {
            this.BoundingMin = stream.ReadVector();
            this.BoundingMax = stream.ReadVector();
            this.BoundingCenter = stream.ReadVector();
            this.BoundingRadius = stream.ReadFloat();
        }

        if (this.ColVersion >= 2)
        {
            this.SphereCount = stream.ReadUint16();
            this.BoxCount = stream.ReadUint16();
            this.FaceCount = stream.ReadUint16();
            this.LineCount = stream.ReadSingleByte();

            this.Padding = stream.ReadSingleByte();
            this.Flags = (ColllisionFlags)stream.ReadUint32();

            this.SphereOffset = stream.ReadUint32();
            this.BoxOffset = stream.ReadUint32();
            this.LineOffset = stream.ReadUint32();
            this.VertexOffset = stream.ReadUint32();
            this.FaceOffset = stream.ReadUint32();
            this.TrianglePlaneOffset = stream.ReadUint32();
        }

        if (this.ColVersion >= 3)
        {
            this.ShadowMeshFaceCount = stream.ReadUint32();
            this.ShadowMeshVertexOffset = stream.ReadUint32();
            this.ShadowMeshFaceOffset = stream.ReadUint32();
        }

        if (this.ColVersion >= 4)
        {
            this.Unknown = stream.ReadUint32();
        }
    }

    public void WriteTo(Stream stream)
    {
        if (this.Name.Length != 22)
            throw new CollisionNameSizeMismatchException($"A collision name must be exactly 22 characters long, got {this.Name.Length} (\"{new string(this.Name)}\")");

        stream.WriteChars(this.FourCC);
        stream.WriteUint32(this.Size);
        stream.WriteChars(this.Name);
        stream.WriteUint16((ushort)this.ModelId);

        if (this.ColVersion == 1)
        {
            stream.WriteFloat(this.BoundingRadius);
            stream.WriteVector(this.BoundingCenter);
            stream.WriteVector(this.BoundingMin);
            stream.WriteVector(this.BoundingMax);
        }
        else
        {
            stream.WriteVector(this.BoundingMin);
            stream.WriteVector(this.BoundingMax);
            stream.WriteVector(this.BoundingCenter);
            stream.WriteFloat(this.BoundingRadius);
        }

        if (this.ColVersion >= 2)
        {
            stream.WriteUint16(this.SphereCount);
            stream.WriteUint16(this.BoxCount);
            stream.WriteUint16(this.FaceCount);
            stream.WriteByte(this.LineCount);

            stream.WriteByte(this.Padding);
            stream.WriteUint32((uint)this.Flags);

            stream.WriteUint32(this.SphereOffset);
            stream.WriteUint32(this.BoxOffset);
            stream.WriteUint32(this.LineOffset);
            stream.WriteUint32(this.VertexOffset);
            stream.WriteUint32(this.FaceOffset);
            stream.WriteUint32(this.TrianglePlaneOffset);
        }

        if (this.ColVersion >= 3)
        {
            stream.WriteUint32(this.ShadowMeshFaceCount);
            stream.WriteUint32(this.ShadowMeshVertexOffset);
            stream.WriteUint32(this.ShadowMeshFaceOffset);
        }

        if (this.ColVersion >= 4)
        {
            stream.WriteUint32(this.Unknown);
        }
    }
}
