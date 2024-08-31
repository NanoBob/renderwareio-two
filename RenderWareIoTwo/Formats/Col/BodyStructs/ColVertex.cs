namespace RenderWareIoTwo.Formats.Col.BodyStructs;

public struct ColVertex
{
    public int Version { get; set; }

    private float xFloat;
    private short xShort;
    public float X
    {
        readonly get => xFloat;
        set
        {
            xFloat = value;
            xShort = (short)(value * 128);
        }
    }

    private float yFloat;
    private short yShort;
    public float Y
    {
        readonly get => yFloat;
        set
        {
            yFloat = value;
            yShort = (short)(value * 128);
        }
    }

    private float zFloat;
    private short zShort;
    public float Z
    {
        readonly get => zFloat;
        set
        {
            zFloat = value;
            zShort = (short)(value * 128);
        }
    }


    public void Read(Stream stream)
    {
        if (Version == 1)
        {
            X = stream.ReadFloat();
            Y = stream.ReadFloat();
            Z = stream.ReadFloat();
        }
        else
        {
            X = stream.ReadInt16() / 128f;
            Y = stream.ReadInt16() / 128f;
            Z = stream.ReadInt16() / 128f;
        }
    }

    public readonly void Write(Stream stream)
    {
        if (Version == 1)
        {
            stream.WriteFloat(xFloat);
            stream.WriteFloat(yFloat);
            stream.WriteFloat(zFloat);
        }
        else
        {
            stream.WriteFloat(xShort);
            stream.WriteFloat(yShort);
            stream.WriteFloat(zShort);
        }
    }
}
