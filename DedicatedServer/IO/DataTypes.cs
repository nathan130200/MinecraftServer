using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Minecraft.IO;

public static class DataTypes
{
    delegate object DecoderFunction(ReadOnlySpan<byte> span);
    delegate T DecoderFunction<T>(ReadOnlySpan<byte> span);

    delegate void EncoderFunction(Span<byte> span, object value);
    delegate void EncoderFunction<T>(Span<byte> span, T value);

    static readonly Dictionary<Type, int> _sizeOf = new()
    {
        [typeof(short)] = 2,
        [typeof(int)] = 4,
        [typeof(long)] = 8,
        [typeof(ushort)] = 2,
        [typeof(uint)] = 4,
        [typeof(ulong)] = 8,
        [typeof(float)] = 4,
        [typeof(double)] = 8,
    };

    static readonly Dictionary<Type, DecoderFunction> _decoders = new()
    {
        [typeof(short)] = GetDecoderFunction(BinaryPrimitives.ReadInt16BigEndian),
        [typeof(int)] = GetDecoderFunction(BinaryPrimitives.ReadInt32BigEndian),
        [typeof(long)] = GetDecoderFunction(BinaryPrimitives.ReadInt64BigEndian),
        [typeof(ushort)] = GetDecoderFunction(BinaryPrimitives.ReadUInt16BigEndian),
        [typeof(uint)] = GetDecoderFunction(BinaryPrimitives.ReadUInt32BigEndian),
        [typeof(ulong)] = GetDecoderFunction(BinaryPrimitives.ReadUInt64BigEndian),
        [typeof(float)] = GetDecoderFunction(BinaryPrimitives.ReadSingleBigEndian),
        [typeof(double)] = GetDecoderFunction(BinaryPrimitives.ReadDoubleBigEndian),
    };

    static readonly Dictionary<Type, EncoderFunction> _encoders = new()
    {
        [typeof(short)] = GetEncoderFunction<short>(BinaryPrimitives.WriteInt16BigEndian),
        [typeof(int)] = GetEncoderFunction<int>(BinaryPrimitives.WriteInt32BigEndian),
        [typeof(long)] = GetEncoderFunction<long>(BinaryPrimitives.WriteInt64BigEndian),
        [typeof(ushort)] = GetEncoderFunction<ushort>(BinaryPrimitives.WriteUInt16BigEndian),
        [typeof(uint)] = GetEncoderFunction<uint>(BinaryPrimitives.WriteUInt32BigEndian),
        [typeof(ulong)] = GetEncoderFunction<ulong>(BinaryPrimitives.WriteUInt64BigEndian),
        [typeof(float)] = GetEncoderFunction<float>(BinaryPrimitives.WriteSingleBigEndian),
        [typeof(double)] = GetEncoderFunction<double>(BinaryPrimitives.WriteDoubleBigEndian),
    };

    static DecoderFunction GetDecoderFunction<T>(DecoderFunction<T> func)
        => new(span => func(span));

    static EncoderFunction GetEncoderFunction<T>(EncoderFunction<T> func)
        => new((span, value) => func(span, (T)value));

    public static int GetLength(Type type)
        => _sizeOf[type];

    public static int GetLength<T>()
        => GetLength(typeof(T));

    public static object Read(Type type, Stream source)
    {
        var len = GetLength(type);
        Span<byte> buf = stackalloc byte[len];
        Debug.Assert(source.Read(buf) == len);
        return _decoders[type](buf);
    }

    public static T Read<T>(Stream source)
        => (T)Read(typeof(T), source);

    public static byte[] Write(Type type, object value)
    {
        var buf = new byte[GetLength(type)];
        _encoders[type](buf, value);
        return buf;
    }

    public static void Write(Type type, Stream destination, object value)
    {
        var len = GetLength(type);
        Span<byte> buf = stackalloc byte[len];
        _encoders[type](buf, value);
        destination.Write(buf);
    }

    public static byte[] Write<T>(T value)
        => Write(typeof(T), value);

    public static void Write<T>(Stream destination, T value)
        => Write(typeof(T), destination, value);
}
