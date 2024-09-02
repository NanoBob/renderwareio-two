using FluentAssertions;
using RenderWareIoTwo.Facades.Dff;
using RenderWareIoTwo.Formats.BinaryStreamFIle.Dff;
using RenderWareIoTwo.Formats.BinaryStreamFIle.Dff.Structs;
using System.Drawing;
using System.Numerics;

namespace RenderWareIoTwo.Tests;

public class DffFacadeTests
{
    [Fact]
    public void GenerateDff_ShouldEqualExpectedOutput()
    {
        var outputPath = $"Files/Dff/generated.dff";

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

        using var input = File.OpenRead(outputPath);
        var readDff = new DffFile(input);
        input.Close();

        readDff.ToString().Trim().Should().Be("""
            Clump (593 bytes @ 12) - [0x10]
                FrameList (101 bytes @ 24) - [0x0E]
                    Struct (60 bytes @ 36) - [0x01]
                    Extension (17 bytes @ 108) - [0x03]
                        Frame (5 bytes @ 120) - [0x253F2FE]
                GeometryList (428 bytes @ 137) - [0x1A]
                    Struct (4 bytes @ 149) - [0x01]
                    Geometry (400 bytes @ 165) - [0x0F]
                        Struct (248 bytes @ 177) - [0x01]
                        MaterialList (128 bytes @ 437) - [0x08]
                            Struct (8 bytes @ 449) - [0x01]
                            Material (96 bytes @ 469) - [0x07]
                                Struct (28 bytes @ 481) - [0x01]
                                Texture (44 bytes @ 521) - [0x06]
                                    Struct (4 bytes @ 533) - [0x01]
                                    String (4 bytes @ 549) - [0x02]
                                    String (0 bytes @ 565) - [0x02]
                Atomic (28 bytes @ 577) - [0x14]
                    Struct (16 bytes @ 589) - [0x01]
            """.Trim());
    }

    [Fact]
    public void NewDffFacade_ParsesProperly()
    {
        var inputPath = @"Files/Dff/cube.dff";


        using var input = File.OpenRead(inputPath);
        var dff = new DffFile(input);
        input.Close();

        var facade = dff.GetFacade();

        facade.Geometries.Should().HaveCount(1);

        var geometry = facade.Geometries.First();
        geometry.Vertices.Count().Should().Be(24);
        geometry.Faces.Count().Should().Be(12);
        geometry.Flags.Should().Be((GeometryFlags)0x76);
    }
}
