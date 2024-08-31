namespace RenderWareIoTwo.Formats.Img;

public class ImgArchive
{
    public char[] Version { get; set; } = ['V', 'E', 'R', '2'];
    public uint ItemCount { get; set; }

    public List<ImgDirectoryEntry> DirectoryEntries { get; set; } = [];
    public Dictionary<string, ImgDataEntry> DataEntries { get; set; } = [];

    public void ReadFrom(Stream stream)
    {
        this.Version = stream.ReadChars(4);
        this.ItemCount = stream.ReadUint32();

        for (int i = 0; i < this.ItemCount; i++)
        {
            var entry = new ImgDirectoryEntry();
            entry.ReadFrom(stream);
            this.DirectoryEntries.Add(entry);
        }

        foreach (var directoryEntry in this.DirectoryEntries)
        {
            string sanitizedKey = directoryEntry.Name.Trim('\0').ToLower();
            this.DataEntries[sanitizedKey] = new ImgDataEntry(stream, directoryEntry);
        }
    }

    public void WriteTo(Stream stream)
    {
        if (this.Version.Length != 4)
            throw new Exception("Img archive version must be 4 characters long.");

        stream.WriteChars(this.Version);
        stream.WriteUint32(this.ItemCount);

        var headerSize = this.ItemCount * 32;

        var headerStart = stream.Position;
        var sectionStart = stream.Position + headerSize;

        var currentSection = (uint)sectionStart;
        foreach (var entry in this.DirectoryEntries)
        {
            entry.Offset = currentSection;
            entry.WriteTo(stream);

            currentSection += (uint)(entry.StreamingSize * ImgDataEntry.SectorSize);
        }

        foreach (var entry in this.DataEntries)
            stream.Write(entry.Value.Data);
    }
}
