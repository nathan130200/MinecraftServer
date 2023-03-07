using System.IO;

namespace Minecraft.IO;

public class DataInputStream : DataStream
{
    public DataInputStream()
    {
    }

    public DataInputStream(byte[] buffer) : base(buffer)
    {
    }

    public DataInputStream(Stream stream, bool leaveStreamOpen = true) : base(stream, leaveStreamOpen)
    {
    }

    public short ReadInt16() => Primitive.Read<short>(m_BaseStream);
    public int ReadInt32() => Primitive.Read<int>(m_BaseStream);
    public long ReadInt64() => Primitive.Read<long>(m_BaseStream);
    public ushort ReadUInt16() => Primitive.Read<ushort>(m_BaseStream);
    public uint ReadUInt32() => Primitive.Read<uint>(m_BaseStream);
    public ulong ReadUInt64() => Primitive.Read<ulong>(m_BaseStream);
}
