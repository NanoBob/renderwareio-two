using RenderWareIoTwo.Formats.Common;
using System.Diagnostics;
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

    public void UpdateHeader(bool includeBoundingBox = true, bool zeroUnusedOffsets = false)
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
            var padding = ((this.Body.Vertices.Count() % 4 != 0) ? 2u : 0u);

            this.Header.SphereCount = (ushort)this.Body.Spheres.Count;
            this.Header.BoxCount = (ushort)this.Body.Boxes.Count;
            this.Header.FaceCount = (ushort)this.Body.Faces.Count;
            this.Header.LineCount = 0;

            this.Header.SphereOffset = 116;
            this.Header.BoxOffset = this.Header.SphereOffset + this.Header.SphereCount * 20u;
            this.Header.LineOffset = this.Header.BoxOffset + this.Header.BoxCount * 28u;
            this.Header.VertexOffset = this.Header.LineOffset + this.Header.LineCount * 0u;
            this.Header.FaceOffset = this.Header.VertexOffset + (uint)this.Body.Vertices.Count * 6u + padding;
            this.Header.TrianglePlaneOffset = this.Header.FaceOffset + this.Header.FaceCount * 8u;

            this.Header.Size += 36u;
        }

        if (this.Header.ColVersion >= 3)
        {
            var padding = ((this.Body.ShadowMeshVertices.Count() % 4 != 0) ? 2u : 0u);

            this.Header.ShadowMeshFaceCount = (uint)this.Body.ShadowMeshFaces.Count;
            this.Header.ShadowMeshVertexOffset = this.Header.TrianglePlaneOffset + 0u;
            this.Header.ShadowMeshFaceOffset = this.Header.ShadowMeshVertexOffset + (uint)this.Body.ShadowMeshVertices.Count * 6u + padding;

            this.Header.Size += 12u;
        }

        if (this.Header.ColVersion >= 4)
        {
            this.Header.Size += 4u;
        }

        if (zeroUnusedOffsets)
        {
            if (!this.Body.Spheres.Any())
                this.Header.SphereOffset = 0;
            if (!this.Body.Boxes.Any())
                this.Header.BoxOffset = 0;
            if (!this.Body.Faces.Any())
                this.Header.FaceOffset = 0;
            if (!this.Body.Vertices.Any())
                this.Header.VertexOffset = 0;

            this.Header.LineOffset = 0;
            this.Header.TrianglePlaneOffset = 0;

            if (!this.Body.ShadowMeshVertices.Any())
                this.Header.ShadowMeshVertexOffset = 0;
            if (!this.Body.ShadowMeshFaces.Any())
                this.Header.ShadowMeshFaceOffset = 0;
        }
    }
}
