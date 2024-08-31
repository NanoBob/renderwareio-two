namespace RenderWareIoTwo.Formats.BinaryStreamFile.Structs;

public class TextureNativeStruct : BinaryStreamStruct
{
    public uint Version { get; set; } = 0x09;
    public uint FilterFlags { get; set; } = 0x1106;
    public string TextureName { get; set; } = "texture";
    public string AlphaName { get; set; } = "texture";
    public uint AlphaFlags { get; set; } = 0x8200;
    public char[] TextureFormat { get; set; } = ['D', 'X', 'T', '1'];
    public ushort Width { get; set; }
    public ushort Height { get; set; }
    public byte Depth { get; set; }
    public byte MipMapCount { get; set; }
    public byte TextureCodeType { get; set; }
    public byte Flags { get; set; }
    public byte[] Pallette { get; set; } = [];
    public uint DataSize { get; set; }
    public List<byte[]> MipMaps { get; set; } = [];

    public override void WriteTo(Stream stream)
    {
        if (this.TextureName.Length > 31)
            throw new Exception("Texture names can not be longer than 31 characters");
        if (this.AlphaName.Length > 31)
            throw new Exception("Texture alpha names can not be longer than 31 characters");
        if (this.TextureFormat.Length != 4)
            throw new Exception("Texture format must be 4 characters long");

        Header.WriteTo(stream);

        stream.WriteUint32(this.Version);
        stream.WriteUint32(this.FilterFlags);
        stream.WriteChars(this.TextureName.PadRight(32, '\0').ToCharArray());
        stream.WriteChars(this.AlphaName.PadRight(32, '\0').ToCharArray());

        stream.WriteUint32(this.AlphaFlags);
        stream.WriteChars(this.TextureFormat);
        stream.WriteUint16(this.Width);
        stream.WriteUint16(this.Height);
        stream.WriteByte(this.Depth);
        stream.WriteByte(this.MipMapCount);
        stream.WriteByte(this.TextureCodeType);
        stream.WriteByte(this.Flags);

        stream.Write(this.Pallette);
        stream.WriteUint32(this.DataSize);
        stream.Write(this.Data);

        foreach (var mipmap in this.MipMaps)
        {
            stream.WriteUint32((uint)mipmap.Length);
            stream.Write(mipmap);
        }
    }

    public override void ReadFrom(Stream stream, BinaryStreamHeader? header = null)
    {
        if (header == null)
            Header.ReadFrom(stream);
        else
            Header = header;

        this.Version = stream.ReadUint32();
        this.FilterFlags = stream.ReadUint32();
        this.TextureName = new string(stream.ReadChars(32)).Trim('\0');
        this.AlphaName = new string(stream.ReadChars(32)).Trim('\0');

        this.AlphaFlags = stream.ReadUint32();
        this.TextureFormat = stream.ReadChars(4);
        this.Width = stream.ReadUint16();
        this.Height = stream.ReadUint16();
        this.Depth = stream.ReadSingleByte();
        this.MipMapCount = stream.ReadSingleByte();
        this.TextureCodeType = stream.ReadSingleByte();
        this.Flags = stream.ReadSingleByte();

        var size = this.Depth == 7 ? 256 * 4 : 0;
        this.Pallette = new byte[size];
        stream.Read(this.Pallette, 0, size);

        this.DataSize = stream.ReadUint32();
        this.Data = new byte[this.DataSize];
        stream.Read(this.Data, 0, (int)this.DataSize);

        for (int i = 0; i < this.MipMapCount - 1; i++)
        {
            var mipmapSize = stream.ReadUint32();
            var mipmapData = new byte[mipmapSize];
            stream.Read(mipmapData, 0, (int)mipmapSize);
            this.MipMaps.Add(mipmapData);
        }
    }

    public override void UpdateHeaderSize()
    {
        Header.Size = (uint)(
            4 + 4 + 32 + 32 +
            4 + 4 + 2 + 2 +
            1 + 1 + 1 + 1 +
            this.Pallette.Length +
            4 + this.Data.Length +
            this.MipMaps.Sum(x => x.Length + 4)
        );
    }
}
