using MinecraftServer.IO;

namespace MinecraftServer.Net;

public class Packet : MemoryStream
{
    public int Type
    {
        get;
        protected internal set;
    }

    Packet() : base()
    {

    }

    public Packet(int type) : this()
    {
        this.WriteVarInt(Type = type);
    }

    public Packet(byte[] buffer) : this()
    {
        Log.Trace("Packet::Deserialize(): " + Convert.ToHexString(buffer));
        Write(buffer);
        Position = 0;

        Type = this.ReadVarInt();
    }

    public void Reset()
    {
        SetLength(0);
        this.WriteVarInt(Type);
    }

    public byte[] Serialize()
    {
        using (var result = new MemoryStream())
        {
            var buffer = ToArray();
            Log.Trace("Packet::Serialize(): " + Convert.ToHexString(buffer));

            result.WriteVarInt(buffer.Length);
            result.Write(buffer);
            return result.ToArray();
        }
    }
}
