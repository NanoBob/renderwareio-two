using RenderWareIoTwo.Formats.Col;
using RenderWareIoTwo.Formats.Col.BodyStructs;
using RenderWareIoTwo.Formats.BinaryStreamFile;
using RenderWareIoTwo.Formats.BinaryStreamFIle.Dff.Structs;
using RenderWareIoTwo.Formats.BinaryStreamFIle.Dff;

public static partial class Examples
{
    public static void GenerateCol1(string inputDffPath, string outputColPath)
    {
        using var input = File.OpenRead(inputDffPath);
        var dff = new DffFile(input);
        input.Close();

        Console.WriteLine(dff);

        var geometryChunk = dff.Clump
            .GetChild<BinaryStreamChunk>(RenderWareIoTwo.Formats.BinaryStreamFile.Enums.BinaryStreamChunkType.Geometry, true);

        var geometry = geometryChunk?.Struct as GeometryStruct;
        if (geometry == null)
            throw new Exception("No geometry struct!");

        var vertices = geometry
            .MorphTargets
            .SelectMany(x => x.Vertices);

        var faces = geometry
            .Triangles;

        var col = new ColFile()
        {
            Archive = new ColArchive()
            {
                Collisions = [
                    new ColCombo()
                    {
                        Header = new()
                        {
                            ColVersion = 1,
                            Name = "LARGE".PadRight(22).ToCharArray()
                        },
                        Body = new()
                        {
                            Spheres = [],
                            Boxes = [],
                            Vertices = vertices.Select(x => new ColVertex(){ X = x.X, Y = x.Y, Z = x.Z}).ToList(),
                            FaceGroups = [],
                            Faces = faces.Select(x => new Face(){
                                A = x.VertexIndex3,
                                B = x.VertexIndex2,
                                C = x.VertexIndex1,
                                Light = 15,
                                Material = RenderWareIoTwo.Formats.Col.Enums.MaterialId.Default,
                                Surface = new ColSurface()
                                {
                                    Brightness = 15,
                                    Flag = 0,
                                    Light = 15,
                                    Material = RenderWareIoTwo.Formats.Col.Enums.MaterialId.Default
                                }
                            }).ToList(),
                            ShadowMeshVertices = [],
                            ShadowMeshFaces = []
                        }
                    }
                ]
            }
        };

        using var output = File.OpenWrite(outputColPath);
        col.WriteTo(output);
        output.Close();

        using var reopenend = File.OpenRead(outputColPath);
        var col2 = new ColFile(reopenend);
        reopenend.Close();

        Console.WriteLine("Re-read file successfully");
    }
}