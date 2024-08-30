using System.Text;

namespace RenderWareIoTwo.Formats.Dff.DataChunks;

public class StringChunk : ChildlessChunk
{
    public string Value
    {
        get => Encoding.ASCII.GetString(this.Data);
        set => Data = Encoding.ASCII.GetBytes(value);
    }
}
