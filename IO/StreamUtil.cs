#pragma warning disable

using System;
using System.Buffers.Binary;

namespace MinecraftServer.IO;

public static partial class StreamUtil
{
	public static short ReadInt16(this Stream stream)
	{
		var buffer = new byte[2];
		int count = stream.Read(buffer);

		if(count != 2)
			throw new IOException("Unable to read an 'Int16' from stream.");

		return BinaryPrimitives.ReadInt16BigEndian(buffer);
	}

	public static Stream ReadInt16(this Stream stream, out short result)
	{
		result = stream.ReadInt16();
		return stream;
	}

	public static Stream WriteInt16(this Stream stream, short value)
	{
		var buffer = new byte[2];
		BinaryPrimitives.WriteInt16BigEndian(buffer, value);
		stream.Write(buffer);
		return stream;
	}

  	public static int ReadInt32(this Stream stream)
	{
		var buffer = new byte[4];
		int count = stream.Read(buffer);

		if(count != 4)
			throw new IOException("Unable to read an 'Int32' from stream.");

		return BinaryPrimitives.ReadInt32BigEndian(buffer);
	}

	public static Stream ReadInt32(this Stream stream, out int result)
	{
		result = stream.ReadInt32();
		return stream;
	}

	public static Stream WriteInt32(this Stream stream, int value)
	{
		var buffer = new byte[4];
		BinaryPrimitives.WriteInt32BigEndian(buffer, value);
		stream.Write(buffer);
		return stream;
	}

  	public static long ReadInt64(this Stream stream)
	{
		var buffer = new byte[8];
		int count = stream.Read(buffer);

		if(count != 8)
			throw new IOException("Unable to read an 'Int64' from stream.");

		return BinaryPrimitives.ReadInt64BigEndian(buffer);
	}

	public static Stream ReadInt64(this Stream stream, out long result)
	{
		result = stream.ReadInt64();
		return stream;
	}

	public static Stream WriteInt64(this Stream stream, long value)
	{
		var buffer = new byte[8];
		BinaryPrimitives.WriteInt64BigEndian(buffer, value);
		stream.Write(buffer);
		return stream;
	}

  	public static ushort ReadUInt16(this Stream stream)
	{
		var buffer = new byte[2];
		int count = stream.Read(buffer);

		if(count != 2)
			throw new IOException("Unable to read an 'UInt16' from stream.");

		return BinaryPrimitives.ReadUInt16BigEndian(buffer);
	}

	public static Stream ReadUInt16(this Stream stream, out ushort result)
	{
		result = stream.ReadUInt16();
		return stream;
	}

	public static Stream WriteUInt16(this Stream stream, ushort value)
	{
		var buffer = new byte[2];
		BinaryPrimitives.WriteUInt16BigEndian(buffer, value);
		stream.Write(buffer);
		return stream;
	}

  	public static uint ReadUInt32(this Stream stream)
	{
		var buffer = new byte[4];
		int count = stream.Read(buffer);

		if(count != 4)
			throw new IOException("Unable to read an 'UInt32' from stream.");

		return BinaryPrimitives.ReadUInt32BigEndian(buffer);
	}

	public static Stream ReadUInt32(this Stream stream, out uint result)
	{
		result = stream.ReadUInt32();
		return stream;
	}

	public static Stream WriteUInt32(this Stream stream, uint value)
	{
		var buffer = new byte[4];
		BinaryPrimitives.WriteUInt32BigEndian(buffer, value);
		stream.Write(buffer);
		return stream;
	}

  	public static ulong ReadUInt64(this Stream stream)
	{
		var buffer = new byte[8];
		int count = stream.Read(buffer);

		if(count != 8)
			throw new IOException("Unable to read an 'UInt64' from stream.");

		return BinaryPrimitives.ReadUInt64BigEndian(buffer);
	}

	public static Stream ReadUInt64(this Stream stream, out ulong result)
	{
		result = stream.ReadUInt64();
		return stream;
	}

	public static Stream WriteUInt64(this Stream stream, ulong value)
	{
		var buffer = new byte[8];
		BinaryPrimitives.WriteUInt64BigEndian(buffer, value);
		stream.Write(buffer);
		return stream;
	}

  }