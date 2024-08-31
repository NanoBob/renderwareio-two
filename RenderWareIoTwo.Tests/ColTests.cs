using FluentAssertions;
using RenderWareIoTwo.Formats.Col;
using RenderWareIoTwo.Formats.Col.BodyStructs;

namespace RenderWareIoTwo.Tests;

public class ColTests
{
    [Theory]
    [InlineData(@"Files\Col\cube-1.col", $@"Files\Col\{nameof(ReadAndWrite_ShouldEqual)}-cube-1.col")]
    [InlineData(@"Files\Col\cube-3.col", $@"Files\Col\{nameof(ReadAndWrite_ShouldEqual)}-cube-3.col")]
    [InlineData(@"Files\Col\donut-1.col", $@"Files\Col\{nameof(ReadAndWrite_ShouldEqual)}-donut-1.col")]
    [InlineData(@"Files\Col\donut-3.col", $@"Files\Col\{nameof(ReadAndWrite_ShouldEqual)}-donut-3.col")]
    [InlineData(@"Files\Col\monkey-1.col", $@"Files\Col\{nameof(ReadAndWrite_ShouldEqual)}-monkey-1.col")]
    [InlineData(@"Files\Col\monkey-3.col", $@"Files\Col\{nameof(ReadAndWrite_ShouldEqual)}-monkey-3.col")]
    [InlineData(@"Files\Col\col1-multiple-3.col", $@"Files\Col\{nameof(ReadAndWrite_ShouldEqual)}-col1-multiple-3.col")]
    [InlineData(@"Files\Col\col3-multiple-3.col", $@"Files\Col\{nameof(ReadAndWrite_ShouldEqual)}-col3-multiple-3.col", true)]
    public void ReadAndWrite_ShouldEqual(string inputPath, string outputPath, bool zeroUnusedOffsets = false)
    {
        if (File.Exists(outputPath))
            File.Delete(outputPath);

        using var input = File.OpenRead(inputPath);
        var col = new ColFile(input);
        input.Close();

        Console.WriteLine(col);

        using var output = File.OpenWrite(outputPath);
        col.WriteTo(output, false, zeroUnusedOffsets);
        output.Close();

        var inputBuffer = File.ReadAllBytes(inputPath);
        var outputBuffer = File.ReadAllBytes(outputPath);

        outputBuffer.Should().BeEquivalentTo(inputBuffer);
    }

    [Theory]
    [InlineData(@"Files\Col\mixed-multiple-2.col", 2)]
    [InlineData(@"Files\Col\mixed-multiple-6.col", 6)]
    [InlineData(@"Files\Col\col1-multiple-3.col", 3)]
    [InlineData(@"Files\Col\col3-multiple-3.col", 3)]
    public void ReadColWithMultipleCollidersShouldParseAll(string inputPath, int expectedCount)
    {
        using var input = File.OpenRead(inputPath);
        var col = new ColFile(input);
        input.Close();

        col.Archive.Collisions.Count().Should().Be(expectedCount);
    }

    [Theory]
    [InlineData(@"Files\Col\cube-1.col", 24, 12, 0, 0, 0)]
    [InlineData(@"Files\Col\cube-3.col", 24, 12, 0, 0, 0)]
    [InlineData(@"Files\Col\donut-1.col", 36864, 12288, 0, 0, 0)]
    [InlineData(@"Files\Col\donut-3.col", 36864, 12288, 0, 0, 0)]
    [InlineData(@"Files\Col\monkey-1.col", 2875, 968, 0, 0, 0)]
    [InlineData(@"Files\Col\monkey-3.col", 2875, 968, 0, 0, 0)]
    [InlineData(@"Files\Col\col3-chunk.col", 4225, 8192, 0, 0, 0)]
    public void ReadColWithSingleCollisionShouldContainExpectedCounts(
        string inputPath, 
        int vertexCount, 
        int faceCount, 
        int faceGroups, 
        int sphereCount, 
        int boxCount)
    {
        using var input = File.OpenRead(inputPath);
        var col = new ColFile(input);
        input.Close();

        var collision = col.Archive.Collisions.First();
        collision.Body.Vertices.Count().Should().Be(vertexCount);
        collision.Body.Faces.Count().Should().Be(faceCount);
        collision.Body.FaceGroups.Count().Should().Be(faceGroups);
        collision.Body.Spheres.Count().Should().Be(sphereCount);
        collision.Body.Boxes.Count().Should().Be(boxCount);
    }

    [Theory]
    [InlineData(257, 0, 0)]
    [InlineData(0, 257, 0)]
    [InlineData(0, 0, 257)]
    [InlineData(-257, 0, 0)]
    [InlineData(0, -257, 0)]
    [InlineData(0, 0, -257)]
    public void CreateColWithOutOfRangeVertexPositionShouldThrowException(float x, float y, float z)
    {
        var col = new ColFile()
        {
            Archive = new ColArchive()
            {
                Collisions = [
                    new ColCombo()
                    {
                        Header = new()
                        {
                            ColVersion = 3
                        },
                        Body = new()
                        {
                            Spheres = [],
                            Boxes = [],
                            Vertices = [
                                new ColVertex()
                                {
                                    X = x,
                                    Y = y,
                                    Z = z
                                }
                            ],
                            FaceGroups = [],
                            Faces = [],
                            ShadowMeshVertices = [],
                            ShadowMeshFaces = []
                        }
                    }
                ]
            }
        };

        using var stream = new MemoryStream();
        var action = () => col.WriteTo(stream);

        action.Should().Throw<CollisionVertexOutOfRangeException>();
    }

    [Theory]
    [InlineData(255, 0, 0)]
    [InlineData(0, 255, 0)]
    [InlineData(0, 0, 255)]
    [InlineData(-255, 0, 0)]
    [InlineData(0, -255, 0)]
    [InlineData(0, 0, -255)]
    public void CreateColWithInRangeVertexPositionShouldNotThrowException(float x, float y, float z)
    {
        var col = new ColFile()
        {
            Archive = new ColArchive()
            {
                Collisions = [
                    new ColCombo()
                    {
                        Header = new()
                        {
                            ColVersion = 3
                        },
                        Body = new()
                        {
                            Spheres = [],
                            Boxes = [],
                            Vertices = [
                                new ColVertex()
                                {
                                    X = x,
                                    Y = y,
                                    Z = z
                                }
                            ],
                            FaceGroups = [],
                            Faces = [],
                            ShadowMeshVertices = [],
                            ShadowMeshFaces = []
                        }
                    }
                ]
            }
        };

        using var stream = new MemoryStream();
        var action = () => col.WriteTo(stream);

        action.Should().NotThrow<CollisionVertexOutOfRangeException>();
    }

    [Theory]
    [InlineData(257, 0, 0)]
    [InlineData(0, 257, 0)]
    [InlineData(0, 0, 257)]
    [InlineData(-257, 0, 0)]
    [InlineData(0, -257, 0)]
    [InlineData(0, 0, -257)]
    public void CreateColWithOutOfRangeVertexPositionUsingCol1ShouldNotThrowException(float x, float y, float z)
    {
        var col = new ColFile()
        {
            Archive = new ColArchive()
            {
                Collisions = [
                    new ColCombo()
                    {
                        Header = new()
                        {
                            ColVersion = 1
                        },
                        Body = new()
                        {
                            Spheres = [],
                            Boxes = [],
                            Vertices = [
                                new ColVertex()
                                {
                                    X = x,
                                    Y = y,
                                    Z = z
                                }
                            ],
                            FaceGroups = [],
                            Faces = [],
                            ShadowMeshVertices = [],
                            ShadowMeshFaces = []
                        }
                    }
                ]
            }
        };

        using var stream = new MemoryStream();
        var action = () => col.WriteTo(stream);

        action.Should().NotThrow<CollisionVertexOutOfRangeException>();
    }
}