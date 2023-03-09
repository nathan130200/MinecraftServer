using System;
using System.IO;
using System.Text;
using Minecraft.Entities;
using Minecraft.Utilities;

namespace Minecraft.IO;

public class DataOutputStream : DataStream
{
    public DataOutputStream()
    {

    }

    public DataOutputStream(Stream stream, bool leaveStreamOpen = true) : base(stream, leaveStreamOpen)
    {

    }

    public void WriteUInt8(byte value)
        => _baseStream.WriteByte(value);

    public void WriteInt8(sbyte value)
        => _baseStream.WriteByte((byte)value);

    public void WriteUInt8Array(Span<byte> span)
    {
        WriteVarInt(span.Length);
        _baseStream.Write(span);
    }

    public void WriteBoolean(bool value)
        => WriteUInt8(value switch { true => 0x01, false or _ => 0x00 });

    public void WriteInt16(short value)
        => DataTypes.Write(_baseStream, value);

    public void WriteInt32(int value)
        => DataTypes.Write(_baseStream, value);

    public void WriteInt64(long value)
        => DataTypes.Write(_baseStream, value);

    public void WriteUInt16(ushort value)
        => DataTypes.Write(_baseStream, value);

    public void WriteUInt32(uint value)
        => DataTypes.Write(_baseStream, value);

    public void WriteUInt64(ulong value)
        => DataTypes.Write(_baseStream, value);

    public void WriteFloat(float value)
        => DataTypes.Write(_baseStream, value);

    public void WriteDouble(double value)
        => DataTypes.Write(_baseStream, value);

    public void WriteVarInt(int value)
        => IOUtil.WriteVarInt(_baseStream, value, out _);

    public void WriteVarLong(long value)
        => IOUtil.WriteVarLong(_baseStream, value, out _);

    public void WriteString(string s)
        => WriteUInt8Array(Encoding.UTF8.GetBytes(s));

    public void WriteFixedPointDouble(double value)
        => WriteInt32((int)value * 32);

    public void WriteBlockPosition(BlockPosition blockPos)
    {
        var (x, y, z) = blockPos;

        long value = ((long)x & 0x3FFFFFF) << 38
            | ((long)y & 0xfff) << 26
            | (long)z & 0x3FFFFFF;

        WriteInt64(value);
    }
}
