namespace RenderWareIoTwo.Formats.Dff.DataChunks;

public class ChildlessChunk : DffChunk
{
    public byte[] Data { get; set; } = [];

    public override void WriteTo(Stream stream)
    {
        this.Header.WriteTo(stream);

        stream.Write(this.Data);
    }

    public override void ReadFrom(Stream stream, DffHeader? header = null)
    {
        if (header == null)
            this.Header.ReadFrom(stream);
        else
            this.Header = header;

        var size = this.Header.Size;
        this.ReadPosition = stream.Position;

        this.Data = new byte[size];
        stream.Read(Data, 0, (int)size);
    }

    public override void UpdateHeaderSize()
    {
        this.Header.Size = (uint)Data.Length;
    }
}
