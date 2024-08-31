using FluentAssertions;
using RenderWareIoTwo.Formats.BinaryStreamFile;
using RenderWareIoTwo.Formats.BinaryStreamFIle.Dff;
using Xunit.Abstractions;

namespace RenderWareIoTwo.Tests;

public class DffTests(ITestOutputHelper testOutput)
{
    [Theory]
    [InlineData(@"Files\Dff\cube.dff", $@"Files\Dff\{nameof(DffReadAndWrite_ShouldEqual)}-cube.dff")]
    [InlineData(@"Files\Dff\monkey.dff", $@"Files\Dff\{nameof(DffReadAndWrite_ShouldEqual)}-monkey.dff")]
    [InlineData(@"Files\Dff\donut.dff", $@"Files\Dff\{nameof(DffReadAndWrite_ShouldEqual)}-donut.dff")]
    //[InlineData(@"Files\Dff\vehicle-input.dff", $@"Files\Dff\{nameof(DffReadAndWrite_ShouldEqual)}-vehicle.dff")]
    public void DffReadAndWrite_ShouldEqual(string inputPath, string outputPath)
    {
        if (File.Exists(outputPath))
            File.Delete(outputPath);

        using var input = File.OpenRead(inputPath);
        var dff = new DffFile(input);
        input.Close();

        testOutput.WriteLine(dff.ToString());

        using var output = File.OpenWrite(outputPath);
        dff.WriteTo(output);
        output.Close();


        var inputBuffer = File.ReadAllBytes(inputPath);
        var outputBuffer = File.ReadAllBytes(outputPath);

        outputBuffer.Should().BeEquivalentTo(inputBuffer);
    }
}
