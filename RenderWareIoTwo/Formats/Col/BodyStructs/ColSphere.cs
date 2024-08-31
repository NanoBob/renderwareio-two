using System.Numerics;

namespace RenderWareIoTwo.Formats.Col.BodyStructs;

public struct ColSphere
{
    public int Version { get; set; }

    public Vector3 Center { get; set; }
    public float Radius { get; set; }
    public ColSurface Surface { get; set; }


    public void Read(Stream stream)
    {
        if (Version == 1)
        {
            this.Radius = stream.ReadFloat();
            this.Center = stream.ReadVector();
            this.Surface = new ColSurface();
            this.Surface.Read(stream);
        }
        else
        {
            this.Center = stream.ReadVector();
            this.Radius = stream.ReadFloat();
            this.Surface = new ColSurface();
            this.Surface.Read(stream);
        }
    }

    public readonly void Write(Stream stream)
    {
        if (Version == 1)
        {
            stream.WriteFloat(this.Radius);
            stream.WriteVector(this.Center);
            this.Surface.Write(stream);
        }
        else
        {
            stream.WriteVector(this.Center);
            stream.WriteFloat(this.Radius);
            this.Surface.Write(stream);
        }
    }
}
