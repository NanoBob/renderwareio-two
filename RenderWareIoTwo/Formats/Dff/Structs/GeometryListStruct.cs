namespace RenderWareIoTwo.Formats.Dff;

public class GeometryListStruct : DffStruct
{
    public uint GeometryCount
    {
        get => BitConverter.ToUInt32(this.Data, 0);
        set => this.Data.ReplaceUint32(0, value);
    }
}
