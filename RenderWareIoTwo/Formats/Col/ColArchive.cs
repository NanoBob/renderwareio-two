using RenderWareIoTwo.Formats.Common;

namespace RenderWareIoTwo.Formats.Col;

public class ColArchive : IStreamReadable
{
    public List<ColCombo> Collisions { get; set; } = [];

    public void ReadFrom(Stream stream)
    {
        while (stream.Position < stream.Length)
        {
            var collision = new ColCombo();
            collision.ReadFrom(stream);
            this.Collisions.Add(collision);
        }
    }

    public void WriteTo(Stream stream, bool updateBoundingBox = true)
    {
        foreach (var collision in this.Collisions)
        {
            collision.UpdateHeader(updateBoundingBox);
            collision.WriteTo(stream);
        }
    }
}
