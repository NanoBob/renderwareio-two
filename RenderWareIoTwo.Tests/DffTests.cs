using FluentAssertions;
using RenderWareIoTwo.Formats.Dff;
using System.Diagnostics;
using Xunit.Abstractions;

namespace RenderWareIoTwo.Tests;

public class DffTests(ITestOutputHelper testOutput)
{
    [Theory]
    [InlineData(@"Files\Dff\basic-input.dff", $@"Files\Dff\{nameof(DffReadAndWrite_ShouldEqual)}-basic-output.dff")]
    [InlineData(@"Files\Dff\vehicle-input.dff", $@"Files\Dff\{nameof(DffReadAndWrite_ShouldEqual)}-vehicle-output.dff")]
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