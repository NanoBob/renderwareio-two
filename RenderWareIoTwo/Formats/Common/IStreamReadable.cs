using RenderWareIoTwo.Formats.BinaryStreamFile;

namespace RenderWareIoTwo.Formats.Common;

public interface IDffStreamReadable
{
    public void ReadFrom(Stream stream, BinaryStreamHeader? header = null);
}

public interface IStreamReadable
{
    public void ReadFrom(Stream stream);
}