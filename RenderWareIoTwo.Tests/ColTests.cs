using FluentAssertions;
using RenderWareIoTwo.Formats.Col;

namespace RenderWareIoTwo.Tests;

public class ColTests
{
    [Theory]
    [InlineData(@"Files\Col\col1.col", $@"Files\Col\{nameof(ReadAndWrite_ShouldEqual)}-output-col1.col")]
    [InlineData(@"Files\Col\col3.col", $@"Files\Col\{nameof(ReadAndWrite_ShouldEqual)}-output-col3.col")]
    public void ReadAndWrite_ShouldEqual(string inputPath, string outputPath)
    {
        if (File.Exists(outputPath))
            File.Delete(outputPath);

        using var input = File.OpenRead(inputPath);
        var col = new ColFile(input);
        input.Close();

        Console.WriteLine(col);

        using var output = File.OpenWrite(outputPath);
        col.WriteTo(output, false);
        output.Close();

        var inputBuffer = File.ReadAllBytes(inputPath);
        var outputBuffer = File.ReadAllBytes(outputPath);

        outputBuffer.Should().BeEquivalentTo(inputBuffer);
    }

    [Theory]
    [InlineData(@"Files\Col\col1-multiple.col", 5)]
    public void ReadColWithMultipleCollidersShouldParseAll(string inputPath, int expectedCount)
    {
        using var input = File.OpenRead(inputPath);
        var col = new ColFile(input);
        input.Close();

        col.Archive.Collisions.Count().Should().Be(expectedCount);
    }

    [Theory]
    [InlineData(@"Files\Col\col1.col", 24, 16, 0, 23, 0)]
    [InlineData(@"Files\Col\col3.col", 8, 12, 0, 0, 0)]
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
}