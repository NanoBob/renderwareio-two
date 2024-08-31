using RenderWareIoTwo.Formats.Dff;

namespace RenderWareIoTwo.Formats.Common;

public interface IDffStreamReadable
{
    public void ReadFrom(Stream stream, DffHeader? header = null);
}

public interface IStreamReadable
{
    public void ReadFrom(Stream stream);
}