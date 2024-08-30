namespace RenderWareIoTwo.Formats.Common;

public record struct Color(byte R, byte G, byte B, byte A)
{
    public const int Size = 4;
}
