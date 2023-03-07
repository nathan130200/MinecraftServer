using System;
using System.Buffers.Binary;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace Minecraft.Entities;

/// <summary>
/// Java&apos;s like UUID implementation in C#.
/// </summary>
[DebuggerDisplay("{ToString(true),nq}")]
public readonly struct Uuid : IEquatable<Uuid>,
    IParsable<Uuid>
{
    public static Uuid Empty => new(0, 0);

    [DebuggerBrowsable(DebuggerBrowsableState.Never), JsonIgnore]
    private readonly byte[] m_Buffer;

    /// <summary>
    /// Returns the most significant 64 bits of this UUID's 128 bit value.
    /// </summary>
    [JsonProperty("msb")]
    public long MostSignificantBits
    {
        get
        {
            return BinaryPrimitives.ReadInt64BigEndian(m_Buffer.AsSpan(0..8));
        }
    }

    /// <summary>
    /// Returns the least significant 64 bits of this UUID's 128 bit value.
    /// </summary>
    [JsonProperty("lsb")]
    public long LeastSignificantBits
    {
        get
        {
            return BinaryPrimitives.ReadInt64BigEndian(m_Buffer.AsSpan(8..16));
        }
    }

    /// <summary>
    /// The version number associated with this UUID. The version number describes how this was generated.
    /// </summary>
    [JsonIgnore]
    public int Version
        => m_Buffer[6] >> 4 & 0x0f;

    /// <summary>
    /// The UUID variant version.
    /// </summary>
    [JsonIgnore]
    public int Variant
    {
        get
        {
            var variantBits = m_Buffer[8] >> 5 & 0x07;
            if ((variantBits & 0x04) == 0) return 0;
            else if ((variantBits & 0x02) == 0) return 1;
            else if ((variantBits & 0x01) == 0) return 2;
            else return 3;
        }
    }

    /// <summary>
    /// Private constructor which uses a byte array to construct the new UUID.
    /// </summary>
    internal Uuid(byte[] data)
        => m_Buffer = data;

    /// <summary>
    /// Constructs a new <see cref="Uuid"/> using the specified data.
    /// </summary>
    /// <param name="msb">The most significant bits</param>
    /// <param name="lsb">The least significant bits</param>
    public Uuid(long msb, long lsb)
    {
        m_Buffer = new byte[16];
        BinaryPrimitives.WriteInt64BigEndian(m_Buffer.AsSpan(0..8), msb);
        BinaryPrimitives.WriteInt64BigEndian(m_Buffer.AsSpan(8..16), lsb);
    }

    public override string ToString()
        => ToString(false);

    public string ToString(bool includeDashes)
    {
        var str = Convert.ToHexString(m_Buffer);

        if (!includeDashes)
            return str;

        var sb = new StringBuilder(str);
        sb.Insert(8, '-');
        sb.Insert(13, '-');
        sb.Insert(18, '-');
        sb.Insert(23, '-');
        return sb.ToString();
    }

    public override int GetHashCode()
        => m_Buffer.GetHashCode();

    public override bool Equals(object obj)
        => obj is Uuid other && Equals(other);

    public bool Equals(Uuid other)
        => m_Buffer.SequenceEqual(other.m_Buffer);

    public static bool operator ==(Uuid left, Uuid right)
        => left.Equals(right);

    public static bool operator !=(Uuid left, Uuid right)
        => !(left == right);

    public static Uuid CreateNew()
    {
        byte[] buffer = new byte[16];
        RandomNumberGenerator.Fill(buffer);

        buffer[6] &= 0x0f;
        buffer[6] |= 0x40;
        buffer[8] &= 0x3f;
        buffer[8] |= 0x80;

        return new Uuid(buffer);
    }

    public static Uuid FromName(string name)
        => FromBytes(Encoding.UTF8.GetBytes(name));

    public static Uuid FromBytes(ReadOnlySpan<byte> buffer)
    {
        byte[] result = new byte[16];
        MD5.HashData(buffer, result);

        result[6] &= 0x0f;
        result[6] |= 0x30;
        result[8] &= 0x3f;
        result[8] |= 0x80;

        return new Uuid(result);
    }

    public static Uuid Parse(string str)
    {
        if (str.Contains('-'))
        {
            var components = str.Split('-', StringSplitOptions.RemoveEmptyEntries);

            if (components.Length != 5)
                goto FAIL;

            long mostSigBits = Convert.ToInt64(components[0], 16);
            mostSigBits <<= 16;
            mostSigBits |= Convert.ToInt64(components[1], 16);
            mostSigBits <<= 16;
            mostSigBits |= Convert.ToInt64(components[2], 16);

            long leastSigBits = Convert.ToInt64(components[3], 16);
            leastSigBits <<= 48;
            leastSigBits |= Convert.ToInt64(components[4], 16);

            return new Uuid(mostSigBits, leastSigBits);
        }

        var buffer = Convert.FromHexString(str);

        if (buffer.Length != 16)
            goto FAIL;

        return new Uuid(buffer);

    FAIL:
        throw new ArgumentException("UUID not well formed.");
    }

    public static bool TryParse(string str, out Uuid uuid)
    {
        bool result;

        try
        {

            uuid = Parse(str);
            result = true;
        }
        catch
        {
            uuid = Empty;
            result = false;
        }

        return result;
    }

    static Uuid IParsable<Uuid>.Parse(string input, IFormatProvider _)
        => Parse(input);

    static bool IParsable<Uuid>.TryParse(string input, IFormatProvider _, out Uuid result)
        => TryParse(input, out result);
}
