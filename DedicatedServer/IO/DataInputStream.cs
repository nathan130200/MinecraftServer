using System;
using System.IO;
using Minecraft.Entities;
using Minecraft.Utilities;

namespace Minecraft.IO;

public class DataInputStream : DataStream
{
    public DataInputStream(byte[] buffer) : base(buffer)
    {
    }

    public DataInputStream(Stream stream, bool leaveStreamOpen = true) : base(stream, leaveStreamOpen)
    {
    }

    public short ReadInt16()
        => DataTypes.Read<short>(_baseStream);

    public int ReadInt32()
        => DataTypes.Read<int>(_baseStream);

    public long ReadInt64()
        => DataTypes.Read<long>(_baseStream);

    public ushort ReadUInt16()
        => DataTypes.Read<ushort>(_baseStream);

    public uint ReadUInt32()
        => DataTypes.Read<uint>(_baseStream);

    public ulong ReadUInt64()
        => DataTypes.Read<ulong>(_baseStream);

    public void ReadFloat()
        => DataTypes.Read<float>(_baseStream);

    public void ReadDouble()
        => DataTypes.Read<double>(_baseStream);

    public int ReadVarInt()
        => IOUtil.ReadVarInt(_baseStream, out _);

    public long ReadVarLong()
        => IOUtil.ReadVarLong(_baseStream, out _);

    public double ReadFixedPointDouble()
        => (double)ReadInt32() / 32;

    public BlockPosition ReadBlockPosition()
    {
        var value = ReadInt64();
        int x = (int)(value >> 38);
        int y = (int)((value >> 26) & 0xfff);
        int z = (int)((value << 38) >> 38);

        if (x >= MathPowCache.P2R25)
            x -= (int)MathPowCache.P2R26;

        if (y >= MathPowCache.P2R11)
            y -= (int)MathPowCache.P2R12;

        if (z >= MathPowCache.P2R25)
            z -= (int)MathPowCache.P2R26;

        return new BlockPosition(x, y, z);
    }
}

static class MathPowCache
{
    internal static readonly double P2R11 = Math.Pow(2, 11);
    internal static readonly double P2R12 = Math.Pow(2, 12);
    internal static readonly double P2R25 = Math.Pow(2, 25);
    internal static readonly double P2R26 = Math.Pow(2, 25);
}