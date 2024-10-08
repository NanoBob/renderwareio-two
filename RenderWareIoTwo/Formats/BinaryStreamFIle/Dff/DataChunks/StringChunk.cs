﻿using RenderWareIoTwo.Formats.BinaryStreamFile.Enums;
using System.Text;

namespace RenderWareIoTwo.Formats.BinaryStreamFIle.Dff.DataChunks;

public class StringChunk : ChildlessChunk
{
    public string Value
    {
        get => Encoding.ASCII.GetString(Data);
        set => Data = Encoding.ASCII.GetBytes(value);
    }

    public StringChunk()
    {
        this.Header.Type = BinaryStreamChunkType.String;
    }
}
