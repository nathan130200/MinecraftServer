using System.Text;

namespace MinecraftServer.IO;

public static partial class StreamUtil
{
    public static byte ReadUInt8(this Stream stream)
    {
        var buffer = new byte[1];

        if (stream.Read(buffer) != 1)
            throw new IOException("Unable to read an 'UInt8' from stream.");

        return buffer[0];
    }

    public static Stream WriteUInt8(this Stream stream, byte value)
    {
        stream.Write(new[] { value });
        return stream;
    }

    public static int ReadVarInt(this Stream stream)
    {
        int value = 0;
        int position = 0;
        byte currentByte;

        while (true)
        {
            currentByte = stream.ReadUInt8();
            value |= (currentByte & 127) << position;

            if ((currentByte & 128) == 0)
                break;

            position += 7;

            if (position >= 32)
                throw new IOException("VarInt is too big");
        }

        return value;
    }

    public static long ReadVarLong(this Stream stream)
    {
        long value = 0;
        int position = 0;
        byte currentByte;

        while (true)
        {
            currentByte = stream.ReadUInt8();
            value |= (long)(currentByte & 127) << position;

            if ((currentByte & 128) == 0) break;

            position += 7;

            if (position >= 64)
                throw new IOException("VarLong is too big");
        }

        return value;
    }

    public static Stream ReadVarInt(this Stream stream, out int result)
    {
        result = stream.ReadVarInt();
        return stream;
    }

    public static Stream ReadVarLong(this Stream stream, out long result)
    {
        result = stream.ReadVarLong();
        return stream;
    }

    public static Stream WriteVarInt(this Stream stream, int value)
    {
        while (true)
        {
            if ((value & ~127) == 0)
            {
                stream.WriteUInt8((byte)value);
                return stream;
            }

            stream.WriteUInt8((byte)((value & 127) | 128));
            value >>>= 7;
        }
    }

    public static Stream WriteVarLong(this Stream stream, long value)
    {
        while (true)
        {
            if ((value & ~127) == 0)
            {
                stream.WriteUInt8((byte)value);
                return stream;
            }

            stream.WriteUInt8((byte)((value & 127) | 128));
            value >>>= 7;
        }
    }

    public static string ReadString(this Stream stream, int maxLength = short.MaxValue)
    {
        if (maxLength > short.MaxValue)
            maxLength = short.MaxValue;

        if (maxLength <= 0)
            maxLength = short.MaxValue;

        var numBytes = stream.ReadVarInt();

        var buffer = new byte[numBytes];
        var count = stream.Read(buffer);

        if (count != numBytes)
            throw new IOException("Unable to read string from transport socket. Length mismatch");

        var str = Encoding.UTF8.GetString(buffer, 0, count);

        if (str.Length > maxLength)
            str = str[0..maxLength];

        return str;
    }

    public static Stream ReadString(this Stream stream, out string result, int maxLength = short.MaxValue)
    {
        result = stream.ReadString(maxLength);
        return stream;
    }

    public static Stream WriteString(this Stream stream, string value, int maxLength = short.MaxValue)
    {
        if (maxLength > short.MaxValue)
            maxLength = short.MaxValue;

        if (maxLength <= 0)
            maxLength = short.MaxValue;

        if (value.Length > maxLength)
            value = value[0..maxLength];

        var buffer = Encoding.UTF8.GetBytes(value);

        stream.WriteVarInt(buffer.Length);
        stream.Write(buffer);
        return stream;
    }
}
