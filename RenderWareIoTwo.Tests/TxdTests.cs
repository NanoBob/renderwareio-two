using FluentAssertions;
using RenderWareIoTwo.Formats.BinaryStreamFile;
using RenderWareIoTwo.Formats.BinaryStreamFile.Enums;
using RenderWareIoTwo.Formats.BinaryStreamFile.Structs;
using RenderWareIoTwo.Formats.BinaryStreamFIle.Txd;
using Xunit.Abstractions;

namespace RenderWareIoTwo.Tests;

public class TxdTests(ITestOutputHelper testOutput)
{
    [Theory]
    [InlineData(@"Files\Txd\two-textures.txd", $@"Files\Txd\{nameof(ReadAndWrite_ShouldEqual)}-two-textures.txd")]
    public void ReadAndWrite_ShouldEqual(string inputPath, string outputPath)
    {
        if (File.Exists(outputPath))
            File.Delete(outputPath);

        using var input = File.OpenRead(inputPath);
        var txd = new TxdFile(input);
        input.Close();

        using var output = File.OpenWrite(outputPath);
        txd.WriteTo(output);
        output.Close();

        var inputBuffer = File.ReadAllBytes(inputPath);
        var outputBuffer = File.ReadAllBytes(outputPath);

        outputBuffer.Should().BeEquivalentTo(inputBuffer);
    }

    [Theory]
    [InlineData(@"Files\Txd\two-textures.txd", new string[] { "wood", "leaves" })]
    public void Read_ShouldIncludeExpectedTextureNames(string inputPath, string[] textureNames)
    {
        using var input = File.OpenRead(inputPath);
        var txd = new TxdFile(input);
        input.Close();

        testOutput.WriteLine(txd.ToString());

        var names = txd.TextureDictionary
            .GetChildren<BinaryStreamChunk>(BinaryStreamChunkType.Raster, true)
            .Select(x => x.Struct as TextureNativeStruct)
            .Select(x => x?.TextureName);

        names.Should().BeEquivalentTo(textureNames);
    }
}
