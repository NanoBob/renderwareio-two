using RenderWareIoTwo.Facades.Dff;
using System.Drawing;
using System.Numerics;

public static partial class Examples
{
    public static void GenerateDff(string outputPath)
    {
        var facade = DffFacade.Create();
        var geometry = facade.AddGeometry();

        var material = geometry.AddMaterial("wood", "", Color.White);

        var corner1 = geometry.AddVertex(new Vector3(-1, 0, 0), Vector3.UnitZ, Vector2.Zero);
        var corner2 = geometry.AddVertex(new Vector3(1, 0, 0), Vector3.UnitZ, Vector2.Zero);
        var corner3 = geometry.AddVertex(new Vector3(-1, -1, 0), Vector3.UnitZ, Vector2.Zero);
        var corner4 = geometry.AddVertex(new Vector3(1, -1, 0), Vector3.UnitZ, Vector2.Zero);
        var top = geometry.AddVertex(Vector3.UnitZ, Vector3.UnitZ, Vector2.Zero);

        geometry.AddFace(corner1, corner2, top, material);
        geometry.AddFace(corner2, corner3, top, material);
        geometry.AddFace(corner3, corner4, top, material);
        geometry.AddFace(corner4, corner1, top, material);

        geometry.AddFace(corner1, corner2, corner3, material);
        geometry.AddFace(corner1, corner3, corner4, material);

        if (File.Exists(outputPath))
            File.Delete(outputPath);

        var dff = facade.ToFile();

        var stream = File.OpenWrite(outputPath);
        dff.WriteTo(stream);
        stream.Close();

        Console.WriteLine(dff);
    }
}