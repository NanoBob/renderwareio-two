using RenderWareIoTwo.Formats.Common;
using System.Numerics;

namespace RenderWareIoTwo.Formats.Col;

public class ColCombo : IStreamReadable, IStreamWriteable
{
    public ColHeader Header { get; set; } = new();
    public ColBody Body { get; set; } = new();

    public void ReadFrom(Stream stream)
    {
        this.Header.ReadFrom(stream);
        this.Body.ReadFrom(stream, this.Header);
    }

    public void WriteTo(Stream stream)
    {
        this.Header.WriteTo(stream);
        this.Body.WriteTo(stream, this.Header);
    }

    public void UpdateHeader(bool includeBoundingBox = true)
    {
        this.Header.Flags =
            ColllisionFlags.IsNotEmpty |
            (this.Body.FaceGroups.Any() ? ColllisionFlags.HasFaceGroups : ColllisionFlags.None);

        if (includeBoundingBox)
        {
            this.Header.BoundingMin = new Vector3(
                this.Body.Vertices.Min(v => v.X),
                this.Body.Vertices.Min(v => v.Y),
                this.Body.Vertices.Min(v => v.Z));

            this.Header.BoundingMax = new Vector3(
                this.Body.Vertices.Max(v => v.X),
                this.Body.Vertices.Max(v => v.Y),
                this.Body.Vertices.Max(v => v.Z));

            this.Header.BoundingCenter = this.Header.BoundingMin + (this.Header.BoundingMax - this.Header.BoundingMin) * 0.5f;
            this.Header.BoundingRadius = this.Body.Vertices.Max(vertex => (new Vector3(vertex.X, vertex.Y, vertex.Z) - this.Header.BoundingCenter).Length());
        }

        // V1 header is 72 bytes, but size is counted after the size itself is written
        this.Header.Size = 72u - 8u + (uint)this.Body.GetSize(this.Header.ColVersion);

        if (this.Header.ColVersion >= 2)
        {
            this.Header.SphereCount = (ushort)this.Body.Spheres.Count;
            this.Header.BoxCount = (ushort)this.Body.Boxes.Count;
            this.Header.FaceCount = (ushort)this.Body.Faces.Count;
            this.Header.LineCount = 0;

            this.Header.SphereOffset = 116;
            this.Header.BoxOffset = this.Header.SphereOffset + this.Header.SphereCount * 20u;
            this.Header.LineOffset = this.Header.BoxOffset + this.Header.BoxCount * 28u;
            this.Header.VertexOffset = this.Header.LineOffset + this.Header.LineCount * 0u;
            this.Header.FaceOffset = this.Header.VertexOffset + (uint)this.Body.Vertices.Count * 6u;
            this.Header.TrianglePlaneOffset = this.Header.FaceOffset + this.Header.FaceCount * 8u;

            this.Header.Size += 36u;
        }

        if (this.Header.ColVersion >= 3)
        {
            this.Header.ShadowMeshFaceCount = (uint)this.Body.ShadowMeshFaces.Count;
            this.Header.ShadowMeshVertexOffset = this.Header.TrianglePlaneOffset + 0u;
            this.Header.ShadowMeshFaceOffset = this.Header.ShadowMeshVertexOffset + (uint)this.Body.ShadowMeshVertices.Count * 6u;

            this.Header.Size = (uint)(this.Body.GetSize(this.Header.ColVersion) + 64 + 48u);

            this.Header.Size += 12u;
        }

        if (this.Header.ColVersion >= 4)
        {
            this.Header.Size += 4u;
        }
    }
}
