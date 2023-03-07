using System.IO;

namespace Minecraft.IO;

public class DataOutputStream : DataStream
{
    public DataOutputStream()
    {
    }

    public DataOutputStream(byte[] buffer) : base(buffer)
    {
    }

    public DataOutputStream(Stream stream, bool leaveStreamOpen = true) : base(stream, leaveStreamOpen)
    {
    }

    public void WriteInt16(short value) => Primitive.Write(m_BaseStream, value);
    public void WriteInt32(int value) => Primitive.Write(m_BaseStream, value);
    public void WriteInt64(long value) => Primitive.Write(m_BaseStream, value);
    public void WriteUInt16(ushort value) => Primitive.Write(m_BaseStream, value);
    public void WriteUInt32(uint value) => Primitive.Write(m_BaseStream, value);
    public void WriteUInt64(ulong value) => Primitive.Write(m_BaseStream, value);
}
