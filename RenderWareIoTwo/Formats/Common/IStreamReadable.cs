using RenderWareIoTwo.Formats.Dff;

namespace RenderWareIoTwo.Formats.Common;

public interface IStreamReadable
{
    public void ReadFrom(Stream stream, DffHeader? header = null);
}