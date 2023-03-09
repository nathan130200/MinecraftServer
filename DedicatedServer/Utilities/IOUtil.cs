using System.IO;
using System.Numerics;
using Minecraft.IO;

namespace Minecraft.Utilities;

public static class IOUtil
{
    public static byte UnsafeReadByte(this Stream s)
    {
        int b = s.ReadByte();

        if (b == -1)
            throw new EndOfStreamException();

        return (byte)b;
    }

    public static DataInputStream GetReader(this Stream s)
        => new(s);

    public static DataOutputStream GetWriter(this Stream s)
        => new(s);

    public static (DataInputStream, DataOutputStream) GetIO(this Stream s)
        => (s.GetReader(), s.GetWriter());

    public static int ReadVarInt(Stream s, out int bytesRead)
    {
        bytesRead = 0;

        int position = 0, value = 0;
        byte currentByte;

        while (true)
        {
            currentByte = s.UnsafeReadByte();
            value |= (currentByte & 127) << position;
            bytesRead++;

            if ((currentByte & 128) == 0)
                break;

            position += 7;

            if (position >= 32)
                throw new IOException("Varint is too big!");
        }

        return value;
    }

    public static long ReadVarLong(Stream s, out int bytesRead)
    {
        bytesRead = 0;

        long value = 0;
        int position = 0;
        byte currentByte;

        while (true)
        {
            currentByte = s.UnsafeReadByte();
            value |= (long)(currentByte & 127) << position;
            bytesRead++;

            if ((currentByte & 128) == 0)
                break;

            position += 7;

            if (position >= 64)
                throw new IOException("Varint is too big!");
        }

        return value;
    }

    public static int CalcVarIntLength(int value)
        => CalcVarSizeLength(value);

    public static int CalcVarLongLength(long value)
        => CalcVarSizeLength(value);

    static int CalcVarSizeLength<T>(T value) where T : struct,
        INumber<T>,
        IBitwiseOperators<T, T, T>,
        IShiftOperators<T, int, T>
    {
        int numBytes = 0;

        var bits = (T)(dynamic)127;
        var def = default(T);

        while (true)
        {
            numBytes++;

            if ((value & ~bits) == def)
                break;

            value >>>= 7;
        }

        return numBytes;
    }

    public static void WriteVarInt(Stream s, int value, out int bytesWritten)
    {
        bytesWritten = 0;

        while (true)
        {
            bytesWritten++;

            if ((value & ~127) == 0)
            {
                s.WriteByte((byte)value);
                break;
            }

            s.WriteByte((byte)((value & 127) | 128));
            value >>>= 7;
        }
    }

    public static void WriteVarLong(Stream s, long value, out int bytesWritten)
    {
        bytesWritten = 0;

        while (true)
        {
            bytesWritten++;

            if ((value & ~127L) == 0)
            {
                s.WriteByte((byte)value);
                break;
            }

            s.WriteByte((byte)((value & 127) | 128));
            value >>>= 7;
        }
    }
}