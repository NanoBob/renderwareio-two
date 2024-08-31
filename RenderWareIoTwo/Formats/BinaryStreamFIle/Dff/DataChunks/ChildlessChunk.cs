using RenderWareIoTwo.Formats.BinaryStreamFile;

namespace RenderWareIoTwo.Formats.BinaryStreamFIle.Dff.DataChunks;

public class ChildlessChunk : BinaryStreamChunk
{
    public byte[] Data { get; set; } = [];

    public override void WriteTo(Stream stream)
    {
        Header.WriteTo(stream);

        stream.Write(Data);
    }

    public override void ReadFrom(Stream stream, BinaryStreamHeader? header = null)
    {
        if (header == null)
            Header.ReadFrom(stream);
        else
            Header = header;

        var size = Header.Size;
        ReadPosition = stream.Position;

        Data = new byte[size];
        stream.Read(Data, 0, (int)size);
    }

    public override void UpdateHeaderSize()
    {
        Header.Size = (uint)Data.Length;
    }
}
