using System.Numerics;

namespace RenderWareIoTwo;

public static class StreamExtensions
{
    public static byte ReadSingleByte(this Stream stream)
    {
        return (byte)stream.ReadByte();
    }

    public static void WriteByte(this Stream stream, byte value)
    {
        stream.WriteByte(value);
    }

    public static ushort ReadUint16(this Stream stream)
    {
        return BitConverter.ToUInt16([
            ReadSingleByte(stream),
            ReadSingleByte(stream)
        ], 0);
    }

    public static short ReadInt16(this Stream stream)
    {
        return BitConverter.ToInt16([
            ReadSingleByte(stream),
            ReadSingleByte(stream)
        ], 0);
    }

    public static void WriteUint16(this Stream stream, ushort value)
    {
        stream.Write(BitConverter.GetBytes(value), 0, 2);
    }

    public static void WriteInt16(this Stream stream, short value)
    {
        stream.Write(BitConverter.GetBytes(value), 0, 2);
    }

    public static uint ReadUint32(this Stream stream)
    {
        return BitConverter.ToUInt32([
            ReadSingleByte(stream),
            ReadSingleByte(stream),
            ReadSingleByte(stream),
            ReadSingleByte(stream)
        ], 0);
    }

    public static void WriteUint32(this Stream stream, uint value)
    {
        stream.Write(BitConverter.GetBytes(value), 0, 4);
    }

    public static float ReadFloat(this Stream stream)
    {
        return BitConverter.ToSingle([
            ReadSingleByte(stream),
            ReadSingleByte(stream),
            ReadSingleByte(stream),
            ReadSingleByte(stream)
        ], 0);
    }

    public static void WriteFloat(this Stream stream, float value)
    {
        stream.Write(BitConverter.GetBytes(value), 0, 4);
    }

    public static Vector3 ReadVector(this Stream stream)
    {
        return new Vector3(
            ReadFloat(stream),
            ReadFloat(stream),
            ReadFloat(stream)
        );
    }

    public static void WriteVector(this Stream stream, Vector3 value)
    {
        WriteFloat(stream, value.X);
        WriteFloat(stream, value.Y);
        WriteFloat(stream, value.Z);
    }

    public static char[] ReadChars(this Stream stream, int count)
    {
        char[] chars = new char[count];
        for (int i = 0; i < count; i++)
        {
            chars[i] = (char)ReadSingleByte(stream);
        }
        return chars;
    }

    public static void WriteChars(this Stream stream, char[] value)
    {
        for (int i = 0; i < value.Length; i++)
        {
            WriteByte(stream, (byte)value[i]);
        }
    }

    public static string ReadString(this Stream stream, char terminator = '\0', int padTo = 4)
    {
        string value = "";
        char lastChar = ' ';
        while (lastChar != terminator)
        {
            lastChar = (char)ReadSingleByte(stream);
            value += lastChar;
        }

        if (padTo > 0 && value.Length % padTo > 0)
        {
            int remainder = padTo - (value.Length % padTo);
            for (int i = 0; i < remainder; i++)
            {
                ReadSingleByte(stream);
            }
        }

        return value;
    }

    public static void WriteString(this Stream stream, string value, int padTo = 4)
    {
        WriteChars(stream, value.ToCharArray());

        if (padTo > 0 && value.Length % padTo > 0)
        {
            int remainder = padTo - (value.Length % padTo);
            for (int i = 0; i < remainder; i++)
            {
                WriteByte(stream, 0);
            }
        }
    }
}
