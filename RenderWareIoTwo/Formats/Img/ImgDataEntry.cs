namespace RenderWareIoTwo.Formats.Img;

public class ImgDataEntry(Stream stream, ImgDirectoryEntry entry)
{
    public const int SectorSize = 2048;

    private readonly Stream stream = stream;
    private readonly ImgDirectoryEntry entry = entry;

    private byte[]? data;
    public byte[] Data
    {
        get
        {
            if (this.data != null)
                return this.data;

            int byteCount = entry.StreamingSize * SectorSize;
            byte[] buffer = new byte[byteCount];

            long oldPosition = this.stream.Position;

            int offset = (int)entry.Offset * SectorSize;
            this.stream.Position = offset;
            this.stream.Read(buffer, 0, byteCount);
            this.data = buffer;

            this.stream.Position = oldPosition;
            return buffer;
        }
        set
        {
            this.data = value;
        }
    }

    public uint SizeInArchive => (uint)(Math.Ceiling(this.Data.Length / (float)SectorSize) * SectorSize); 
}