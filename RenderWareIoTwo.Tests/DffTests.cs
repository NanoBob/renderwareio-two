using FluentAssertions;
using RenderWareIoTwo.Formats.Dff;

namespace RenderWareIoTwo.Tests;

public class DffTests
{
    [Theory]
    [InlineData(@"Files\Dff\basic-input.dff", $@"Files\Dff\{nameof(DffReadAndWrite_ShouldEqual)}-output.dff")]
    public void DffReadAndWrite_ShouldEqual(string inputPath, string outputPath)
    {
        if (File.Exists(outputPath))
            File.Delete(outputPath);

        using var input = File.OpenRead(inputPath);
        var dff = new DffFile(input);
        input.Close();

        Console.WriteLine(dff);

        using var output = File.OpenWrite(outputPath);
        dff.WriteTo(output);
        output.Close();


        var inputBuffer = File.ReadAllBytes(inputPath);
        var outputBuffer = File.ReadAllBytes(outputPath);

        outputBuffer.Should().BeEquivalentTo(inputBuffer);
    }
}