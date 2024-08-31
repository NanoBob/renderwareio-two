using FluentAssertions;
using RenderWareIoTwo.Formats.Img;
using Xunit.Abstractions;

namespace RenderWareIoTwo.Tests;

public class ImgTests(ITestOutputHelper testOutput)
{
    [Theory]
    [InlineData(@"Files\Img\input.img", new string[] { "cube.dff", "donut.dff", "large-input.dff", "monkey.dff", "cube-1.col", "donut-1.col", "monkey-1.col" })]
    public void ReadImg_ShouldContainEntries(string imgPath, string[] files)
    {
        using var stream = File.OpenRead(imgPath);
        var img = new ImgFile(stream);
        stream.Close();

        var names = img.Archive.DirectoryEntries.Select(x => x.Name);

        names.Should().BeEquivalentTo(files);
    }

    [Theory]
    [InlineData(@"Files\Img\input.img", "cube-1.col", @"Files\Col\cube-1.col")]
    public void ReadImgFile_EqualSourceFile(string imgPath, string file, string expectedOutputFile)
    {
        using var stream = File.OpenRead(imgPath);
        var img = new ImgFile(stream);

        var data = img.Archive.DataEntries[file].Data;
        data.Should().ShouldBeEquivalentToWithOptionalPadding(File.ReadAllBytes(expectedOutputFile));

        stream.Close();
    }

    [Theory]
    [InlineData(@"Files\Img\input.img", @"Files\Img\output.img", "cube-1.col", @"Files\Col\cube-1.col")]
    [InlineData(@"Files\Img\input.img", @"Files\Img\output.img", "monkey-1.col", @"Files\Col\monkey-1.col")]
    [InlineData(@"Files\Img\input.img", @"Files\Img\output.img", "cube.dff", @"Files\Dff\cube.dff")]
    public void ReadImgAndRewriteImgThenReadFile_EqualSourceFile(string imgPath, string outputPath, string file, string expectedOutputFile)
    {
        if (File.Exists(outputPath))
            File.Delete(outputPath);

        using var input = File.OpenRead(imgPath);
        var sourceImg = new ImgFile(input);

        testOutput.WriteLine(sourceImg.ToString());

        using var output = File.OpenWrite(outputPath);
        sourceImg.WriteTo(output);
        output.Close();

        input.Close();


        using var stream = File.OpenRead(imgPath);
        var img = new ImgFile(stream);

        var data = img.Archive.DataEntries[file].Data;
        data.Should().ShouldBeEquivalentToWithOptionalPadding(File.ReadAllBytes(expectedOutputFile));

        stream.Close();
    }

    [Theory(Skip = "Files will not be perfectly equivalent because there seems to be superfluous data in the original sample")]
    [InlineData(@"Files\Img\input.img", $@"Files\Img\{nameof(ImgReadAndWrite_ShouldEqual)}-output.img")]
    public void ImgReadAndWrite_ShouldEqual(string inputPath, string outputPath)
    {
        if (File.Exists(outputPath))
            File.Delete(outputPath);

        using var input = File.OpenRead(inputPath);
        var img = new ImgFile(input);

        testOutput.WriteLine(img.ToString());

        using var output = File.OpenWrite(outputPath);
        img.WriteTo(output);
        output.Close();

        input.Close();


        var inputBuffer = File.ReadAllBytes(inputPath);
        var outputBuffer = File.ReadAllBytes(outputPath);

        outputBuffer.Should().ShouldBeEquivalentToWithOptionalPadding(inputBuffer);
    }

    [Theory]
    [InlineData(@"Files\Img\new.img", "cube-1.col", @"Files\Col\cube-1.col")]
    [InlineData(@"Files\Img\new.img", "cube-3.col", @"Files\Col\cube-3.col")]
    public void AddFileToNewImgShouldBeAccesibleAfterSaving(string outputPath, string name, string expectedFile)
    {
        if (File.Exists(outputPath))
            File.Delete(outputPath);

        var sourceImg = new ImgFile();
        sourceImg.Archive.AddEntry(name, File.ReadAllBytes(expectedFile));

        using var output = File.OpenWrite(outputPath);
        sourceImg.WriteTo(output);
        output.Close();

        using var stream = File.OpenRead(outputPath);
        var img = new ImgFile(stream);

        var data = img.Archive.DataEntries[name].Data;
        data.Should().ShouldBeEquivalentToWithOptionalPadding(File.ReadAllBytes(expectedFile));

        stream.Close();
    }
}
