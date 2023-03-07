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

    public short ReadInt16() => Primitive.Read<short>(baseStream);
    public int ReadInt32() => Primitive.Read<int>(baseStream);
    public long ReadInt64() => Primitive.Read<long>(baseStream);
    public ushort ReadUInt16() => Primitive.Read<ushort>(baseStream);
    public uint ReadUInt32() => Primitive.Read<uint>(baseStream);
    public ulong ReadUInt64() => Primitive.Read<ulong>(baseStream);
}
