using RenderWareIoTwo.Formats.BinaryStreamFIle.Dff.Structs;
using RenderWareIoTwo.Formats.Col.Enums;
using System.Drawing;
using System.Numerics;

namespace RenderWareIoTwo.Facades.Dff;

/// <summary>
/// Represents a Vertex, a single point in 3D space
/// </summary>
public struct VertexFacade
{
    public int Index { get; set; }
    public Vector3 Position { get; set; }
    public Vector3? Normal { get; set; }
    public Vector2 Uv { get; set; }

    public Color? DaytimeColor { get; set; }
    public Color? NighttimeColor { get; set; }
}

/// <summary>
/// Represents a face, a connection of 3 vertices
/// </summary>
public struct FaceFacade
{
    public VertexFacade Vertex1 { get; set; }
    public VertexFacade Vertex2 { get; set; }
    public VertexFacade Vertex3 { get; set; }

    public MaterialFacade Material { get; set; }
}

/// <summary>
/// Represents a material, this is a link to a texture in a .txd.
/// </summary>
public struct MaterialFacade
{
    public int Index { get; set; }
    public string Name { get; set; } = "material";
    public string Mask { get; set; } = "mask";
    public Color? Color { get; set; }
    public byte FilterFlags { get; set; }

    public MaterialFlags Flags { get; set; }
    public uint Unknown { get; set; }
    public bool IsTextured { get; set; }
    public float Ambient { get; set; }
    public float Specular { get; set; }
    public float Diffuse { get; set; }

    public MaterialFacade() { }
}

/// <summary>
/// Represents a piece of "geometry". This is any individual shape in the model.
/// </summary>
/// <param name="dff"></param>
public class GeometryFacade(
    Vector3? position = null,
    Vector3? rotation = null,
    string name = "Model"
)
{
    public Vector3 Position { get; set; } = position ?? Vector3.Zero;
    public Vector3 Rotation { get; set; } = rotation ?? Vector3.Zero;
    public string Name { get; set; } = name;

    public GeometryFlags Flags { get; set; } = (GeometryFlags) 0x76;


    private readonly List<VertexFacade> vertices = [];
    public IEnumerable<VertexFacade> Vertices => this.vertices;


    private readonly List<FaceFacade> faces = [];
    public IEnumerable<FaceFacade> Faces => this.faces;


    private readonly List<MaterialFacade> materials = [];
    public IEnumerable<MaterialFacade> Materials => this.materials;

    public VertexFacade AddVertex(Vector3 position, Vector3? normal, Vector2 uv)
    {
        var vertex = new VertexFacade()
        {
            Index = this.vertices.Count,
            Position = position,
            Normal = normal,
            Uv = uv
        };

        this.vertices.Add(vertex);

        return vertex;
    }

    public VertexFacade AddPrelitVertex(Vector3 position, Vector3? normal, Vector2 uv, Color daytimeColor, Color? nighttimeColor)
    {
        var vertex = new VertexFacade()
        {
            Index = this.vertices.Count,
            Position = position,
            Normal = normal,
            Uv = uv,
            DaytimeColor = daytimeColor,
            NighttimeColor = nighttimeColor ?? daytimeColor
        };

        this.vertices.Add(vertex);

        return vertex;
    }

    public MaterialFacade AddMaterial(string name, string maskName, Color color)
    {
        var material = new MaterialFacade()
        {
            Index = this.materials.Count,
            Name = name,
            Mask = maskName,
            Color = color
        };

        this.materials.Add(material);

        return material;
    }

    public FaceFacade AddFace(VertexFacade vertex1,  VertexFacade vertex2, VertexFacade vertex3, MaterialFacade material)
    {
        var face = new FaceFacade()
        {
            Vertex1 = vertex1,
            Vertex2 = vertex2,
            Vertex3 = vertex3,
            Material = material
        };

        this.faces.Add(face);
        return face;
    }

    private void RecomputeIndexes()
    {

    }
}
