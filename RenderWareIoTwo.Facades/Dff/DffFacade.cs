using RenderWareIoTwo.Formats.BinaryStreamFile;
using RenderWareIoTwo.Formats.BinaryStreamFile.Enums;
using RenderWareIoTwo.Formats.BinaryStreamFIle.Dff;
using RenderWareIoTwo.Formats.BinaryStreamFIle.Dff.Chunks;
using RenderWareIoTwo.Formats.BinaryStreamFIle.Dff.DataChunks;
using RenderWareIoTwo.Formats.BinaryStreamFIle.Dff.Structs;
using System.Drawing;
using System.Numerics;

namespace RenderWareIoTwo.Facades.Dff;

public static class DffExtensions
{
    public static DffFacade GetFacade(this DffFile dff) => new(dff.Clump);
    public static DffFacade GetFacade(this Clump clump) => new(clump);
}

public class DffFacade
{
    private Clump clump;

    public List<GeometryFacade> Geometries = [];

    public DffFacade(Clump clump)
    {
        this.clump = clump;

        ParseClump();
    }

    public GeometryFacade AddGeometry(Vector3? position = null, Vector3? rotation = null, string name = "Model")
    {
        var facade = new GeometryFacade(position, rotation, name);

        this.Geometries.Add(facade);

        return facade;
    }

    public DffFile ToFile()
    {
        RecreateClump();

        return new DffFile()
        {
            Clump = this.clump,
        };
    }

    public Clump ToClump()
    {
        RecreateClump();

        return this.clump;
    }

    private void RecreateClump()
    {
        var clump = new Clump();

        var frameList = BinaryStreamChunkBuilder.Create(BinaryStreamChunkType.FrameList, true);
        var frameListStruct = (FrameListStruct)frameList.Struct!;

        var geometryList = BinaryStreamChunkBuilder.Create(BinaryStreamChunkType.GeometryList, true);
        var geometryListStruct = (GeometryListStruct)geometryList.Struct!;
        geometryListStruct.GeometryCount = (uint)this.Geometries.Count;

        clump.AddChild(frameList);
        clump.AddChild(geometryList);

        int geometryIndex = 0;
        foreach (var geometry in this.Geometries)
        {
            var frameExtension = BinaryStreamChunkBuilder.Create(BinaryStreamChunkType.Extension);
            frameExtension.AddChild(new FrameChunk()
            {
                Value = geometry.Name
            });
            frameList.AddChild(frameExtension);

            frameListStruct.FrameCount++;
            frameListStruct.Frames = [
                ..frameListStruct.Frames,
                new Frame()
                {
                    Position = geometry.Position,
                    Rotation1 = Vector3.Zero,
                    Rotation2 = Vector3.Zero,
                    Rotation3 = Vector3.Zero,
                    Parent = 0,
                    Flags = 0
                }
            ];

            var atomic = BinaryStreamChunkBuilder.Create(BinaryStreamChunkType.Atomic, true);
            var atomicStruct = (AtomicStruct)atomic.Struct!;

            atomicStruct.FrameIndex = (uint)geometryIndex;
            atomicStruct.GeometryIndex = (uint)geometryIndex;
            atomicStruct.Flags = AtomicFlags.All;
            clump.AddChild(atomic);

            var geometryChunk = CreateGeometry(geometry);
            geometryList.AddChild(geometryChunk);

            geometryIndex++;
        }

        clump.UpdateHeaderSize();

        this.clump = clump;
    }

    private BinaryStreamChunk CreateGeometry(GeometryFacade geometry)
    {
        var geometryChunk = BinaryStreamChunkBuilder.Create(BinaryStreamChunkType.Geometry, true);
        var geometryStruct = (GeometryStruct)geometryChunk.Struct!;

        geometryChunk.AddChild(CreateMaterialList(geometry.Materials));

        var min = new Vector3(
            geometry.Vertices.Min(v => v.Position.X), 
            geometry.Vertices.Min(v => v.Position.Y), 
            geometry.Vertices.Min(v => v.Position.Z));
        var max = new Vector3(
            geometry.Vertices.Max(v => v.Position.X), 
            geometry.Vertices.Max(v => v.Position.Y), 
            geometry.Vertices.Max(v => v.Position.Z));
        var center = min + (max - min) * 0.5f;
        var radius = geometry.Vertices.Max(vertex => 
            (new Vector3(vertex.Position.X, vertex.Position.Y, vertex.Position.Z) - center)
            .Length());

        var morphTarget = new MorphTarget()
        {
            Vertices = [],
            Normals = [],
            HasNormals = geometry.Vertices.Any(x => x.Normal != null),
            HasPosition = geometry.Vertices.Any(),
            Sphere = new Sphere(center, radius)
        };

        geometryStruct.Flags = geometry.Flags;
        geometryStruct.TriangleCount = (uint)geometry.Faces.Count();
        geometryStruct.VertexCount = (uint)geometry.Vertices.Count();

        var uvLayer = new List<Uv>();

        foreach (var vertex in geometry.Vertices)
        {
            var color = vertex.DaytimeColor ?? Color.White;
            geometryStruct.Colors.Add(new Formats.Common.Color(color.R, color.G, color.B, color.A));
            uvLayer.Add(new Uv(vertex.Uv.X, vertex.Uv.Y));

            morphTarget.Vertices.Add(vertex.Position);

            if (vertex.Normal.HasValue)
                morphTarget.Normals.Add(vertex.Normal.Value);
        }

        geometryStruct.UvLayers.Add(uvLayer);

        foreach (var face in geometry.Faces)
        {
            geometryStruct.Triangles.Add(new Triangle()
            {
                VertexIndex1 = (ushort)face.Vertex1.Index,
                VertexIndex2 = (ushort)face.Vertex2.Index,
                VertexIndex3 = (ushort)face.Vertex3.Index,
                MaterialIndex = (ushort)face.Material.Index
            });
        }

        geometryStruct.MorphTargets.Add(morphTarget);
        geometryStruct.MorphTargetCount = 1;

        return geometryChunk;
    }

    private BinaryStreamChunk CreateMaterialList(IEnumerable<MaterialFacade> materials)
    {
        var materialListChunk = BinaryStreamChunkBuilder.Create(BinaryStreamChunkType.MaterialList, true);
        var materialListStruct = (MaterialListStruct)materialListChunk.Struct!;

        materialListStruct.MaterialCount = (uint)materials.Count();
        materialListStruct.MaterialIndexes = materials.Select(x => 0).ToArray();

        foreach (var material in materials)
        {
            var materialChunk = BinaryStreamChunkBuilder.Create(BinaryStreamChunkType.Material, true);
            var materialStruct = (MaterialStruct)materialChunk.Struct!;

            materialListChunk.AddChild(materialChunk);

            var color = material.Color ?? Color.White;

            materialStruct.Flags = material.Flags;
            materialStruct.Color = new Formats.Common.Color(color.R, color.G, color.B, color.A);
            materialStruct.Unknown = material.Unknown;
            materialStruct.IsTextured = material.IsTextured;

            materialStruct.Ambient = material.Ambient;
            materialStruct.Specular = material.Specular;
            materialStruct.Diffuse = material.Diffuse;

            var textureChunk = BinaryStreamChunkBuilder.Create(BinaryStreamChunkType.Texture, true);
            var textureStruct = (TextureStruct)textureChunk.Struct!;

            materialChunk.AddChild(textureChunk);
            textureStruct.FilterMode = material.FilterFlags;

            var nameChunk = new StringChunk() { Value = material.Name };
            var maskChunk = new StringChunk() { Value = material.Mask };

            textureChunk.AddChild(nameChunk);
            textureChunk.AddChild(maskChunk);
        }

        return materialListChunk;
    }

    private void ParseClump()
    {
        var geometries = this.clump
            .GetChildren(BinaryStreamChunkType.Geometry, true)
            .ToArray();

        if (!geometries.Any())
            return;

        var frameList = this.clump.GetChild(BinaryStreamChunkType.FrameList)!;
        var frameNames = frameList
            .GetChildren<FrameChunk>(BinaryStreamChunkType.Frame, true)
            .ToArray();

        var frames = ((FrameListStruct)frameList!.Struct!)
            .Frames
            .ToArray();

        var atomics = this.clump
            .GetChildren(BinaryStreamChunkType.Atomic, true)
            .Select(x => (AtomicStruct)x.Struct!)
            .ToArray();

        for (int i = 0; i < atomics.Length; i++)
        {
            var atomic = atomics[i];

            var frame = frames[atomic.FrameIndex];
            var geometryChunk = geometries[atomic.GeometryIndex];
            var geometryStruct = (GeometryStruct)geometryChunk.Struct!;


            // TODO: Use frame rotation
            var geometry = this.AddGeometry(frame.Position, Vector3.Zero, frameNames[atomic.FrameIndex].Value);


            var materials = geometryChunk
                .GetChildren<BinaryStreamChunk>(BinaryStreamChunkType.Material, true)
                .ToArray();

            var mappedMaterials = new MaterialFacade[materials.Length];
            for (int j = 0; j < materials.Length; j++)
            {
                var material = materials[j];

                var materialStruct = (MaterialStruct)material.Struct!;

                var texture = material.GetChild(BinaryStreamChunkType.Texture);

                var stringChildren = texture?.GetChildren<StringChunk>(BinaryStreamChunkType.String);
                var materialName = stringChildren?.First()?.Value;
                var materialMask = stringChildren?.Last()?.Value;

                var materialFacade = geometry.AddMaterial(
                    materialName ?? "Material",
                    materialMask ?? "Mask",
                    Color.FromArgb(
                        materialStruct.Color.A,
                        materialStruct.Color.R,
                        materialStruct.Color.G,
                        materialStruct.Color.B));

                if (texture != null)
                {
                    var textureStruct = (TextureStruct)texture!.Struct!;
                    materialFacade.FilterFlags = textureStruct.FilterMode;
                }

                mappedMaterials[j] = materialFacade;
            }

            var vertices = geometryStruct.MorphTargets
                .SelectMany(x => x.Vertices)
                .ToArray();
            var normals = geometryStruct.MorphTargets
                .SelectMany(x => x.Normals)
                .ToArray();

            var mappedVertices = new VertexFacade[geometryStruct.VertexCount];
            for (int j = 0; j < geometryStruct.VertexCount; j++)
            {
                mappedVertices[j] = geometry.AddVertex(
                    vertices[i],
                    normals.Any() ? normals[i] : null,
                    new (
                        geometryStruct.UvLayers.First()[j].X,
                        geometryStruct.UvLayers.First()[j].Y)
                );
            }

            foreach (var face in geometryStruct.Triangles)
            {
                geometry.AddFace(
                    mappedVertices[face.VertexIndex1],
                    mappedVertices[face.VertexIndex1],
                    mappedVertices[face.VertexIndex1],
                    mappedMaterials[face.MaterialIndex]
                );
            }
        }
    }

    public static DffFacade Create()
    {
        return new DffFacade(new Clump());
    }
}
