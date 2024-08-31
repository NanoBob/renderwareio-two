namespace RenderWareIoTwo.Formats.Img;

public class ImgFile
{
    public ImgArchive Archive { get; set; } = new();

    public ImgFile()
    {

    }

    public ImgFile(Stream stream)
    {
        this.Archive.ReadFrom(stream);
    }

    public void WriteTo(Stream stream)
    {
        this.Archive.WriteTo(stream);
    }

    public override string? ToString()
        => this.Archive.ToString();
}
