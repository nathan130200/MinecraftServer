using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Minecraft.IO;

public static class Primitive
{
    delegate object DecoderFunction(ReadOnlySpan<byte> span);
    delegate T DecoderFunction<T>(ReadOnlySpan<byte> span);

    delegate void EncoderFunction(Span<byte> span, object value);
    delegate void EncoderFunction<T>(Span<byte> span, T value);

    static readonly Dictionary<Type, int> s_PrimitiveSizes = new()
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

    static readonly Dictionary<Type, DecoderFunction> s_Decoders = new()
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

    static readonly Dictionary<Type, EncoderFunction> s_Encoders = new()
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
        => new(xs => func(xs));

    static EncoderFunction GetEncoderFunction<T>(EncoderFunction<T> func)
        => new((span, value) => func(span, (T)value));

    public static int GetLength(Type type)
        => s_PrimitiveSizes[type];

    public static int GetLength<T>()
        => GetLength(typeof(T));

    public static object Read(Type type, Stream source)
    {
        var len = GetLength(type);
        Span<byte> buf = stackalloc byte[len];
        Debug.Assert(source.Read(buf) == len);
        return s_Decoders[type](buf);
    }

    public static T Read<T>(Stream source)
        => (T)Read(typeof(T), source);

    public static void Write(Type type, Stream destination, object value)
    {
        var len = GetLength(type);
        Span<byte> buf = stackalloc byte[len];
        s_Encoders[type](buf, value);
        destination.Write(buf);
    }

    public static void Write<T>(Stream destination, T value)
        => Write(typeof(T), destination, value);
}
