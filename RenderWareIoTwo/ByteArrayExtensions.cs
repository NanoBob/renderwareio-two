namespace RenderWareIoTwo;

public static class ByteArrayExtensions
{
    public static void ReplaceUint16(this byte[] array, int index, ushort value)
    {
        var data = BitConverter.GetBytes(value);
        array[index = 0] = data[0];
        array[index + 1] = data[1];
    }

    public static void ReplaceUint32(this byte[] array, int index, uint value)
    {
        var data = BitConverter.GetBytes(value);
        array[index = 0] = data[0];
        array[index + 1] = data[1];
        array[index + 2] = data[2];
        array[index + 3] = data[3];
    }

    public static void ReplaceSingle(this byte[] array, int index, float value)
    {
        var data = BitConverter.GetBytes((uint)value);
        array[index = 0] = data[0];
        array[index + 1] = data[1];
        array[index + 2] = data[2];
        array[index + 3] = data[3];
    }
}
