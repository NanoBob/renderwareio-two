﻿using RenderWareIoTwo.Formats.Dff.Chunks;

namespace RenderWareIoTwo.Formats.Dff;

public class DffFile
{
    public Clump Clump { get; set; } = new();

    public DffFile()
    {
    }

    public DffFile(Stream stream)
    {
        this.Clump.ReadFrom(stream);
    }

    public void WriteTo(Stream stream)
    {
        this.Clump.UpdateHeaderSize();
        this.Clump.WriteTo(stream);
    }

    public override string ToString()
        => this.Clump.ToString();
}
