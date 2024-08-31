using RenderWareIoTwo.Formats.Img;

public static partial class Examples
{
    public static void GenerateEmptyImg(string outputPath)
    {
        var img = new ImgFile();

        using var output = File.OpenWrite(outputPath);
        img.WriteTo(output);
        output.Close();
    }
}