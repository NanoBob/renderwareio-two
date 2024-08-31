namespace RenderWareIoTwo.Formats.BinaryStreamFile;

public abstract class BinaryStreamStruct : BinaryStreamChunk
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

        this.ReadPosition = stream.Position;

        var size = Header.Size;

        Data = new byte[size];
        stream.Read(Data, 0, (int)size);
    }

    public override void UpdateHeaderSize()
    {
        this.Header.Size = (uint)Data.Length;
    }
}

public class UnknownDffStruct : BinaryStreamStruct
{

}