namespace RenderWareIoTwo.Formats.Col;

public class ColFile
{
    public ColArchive Archive { get; set; } = new();

    public ColFile()
    {

    }

    public ColFile(Stream stream)
    {
        this.Archive.ReadFrom(stream);
    }

    public void WriteTo(Stream stream, bool updateBoundingBox = true)
    {
        this.Archive.WriteTo(stream, updateBoundingBox);
    }

    public override string? ToString()
        => this.Archive.ToString();
}
