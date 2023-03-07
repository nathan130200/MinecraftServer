using System;
using System.IO;

namespace Minecraft.IO;

public class DataStream : IDisposable
{
    protected Stream baseStream;
    protected readonly bool leaveStreamOpen;
    protected readonly object syncLock = new();
    volatile bool disposed;

    public DataStream(Stream stream, bool leaveOpen = true)
    {
        baseStream = stream;
        leaveStreamOpen = leaveOpen;
    }

    public DataStream(byte[] buffer) : this()
    {
        baseStream.Write(buffer, 0, buffer.Length);
        baseStream.Position = 0;
    }

    public DataStream()
    {
        baseStream = new MemoryStream();
    }

    ~DataStream()
    {
        Dispose();
    }

    public virtual byte[] GetBytes()
    {
        ThrowIfDisposed();

        lock (syncLock)
        {
            if (baseStream is not MemoryStream)
                throw new InvalidOperationException("Stream is not subclass of '" + typeof(MemoryStream).FullName + "'...");

            var pos = baseStream.Position;
            baseStream.Position = 0;

            var buffer = new byte[baseStream.Length];
            baseStream.Read(buffer, 0, buffer.Length);
            baseStream.Position = pos;

            return buffer;
        }
    }

    public void CopyTo(DataStream dest, uint bufferSize = 4096)
        => CopyTo(dest.baseStream, bufferSize);

    public void CopyTo(Stream dest, uint bufferSize = 4096)
    {
        ThrowIfDisposed();

        lock (syncLock)
        {
            var buff = new byte[bufferSize];
            int len;

            long pos = 0;

            if (baseStream.CanSeek)
            {
                pos = baseStream.Position;
                baseStream.Position = 0;
            }

            while ((len = baseStream.Read(buff, 0, buff.Length)) > 0)
            {
                dest.Write(buff, 0, len);
                ThrowIfDisposed();
            }

            if (baseStream.CanSeek)
                baseStream.Position = pos;

            dest.Flush();
        }
    }

    protected void ThrowIfDisposed()
    {
        if (disposed)
            throw new ObjectDisposedException(GetType().FullName);
    }

    public void Dispose()
    {
        ThrowIfDisposed();

        disposed = true;

        lock (syncLock)
        {
            if (baseStream != null)
            {
                if (!leaveStreamOpen)
                    baseStream.Dispose();

                baseStream = null;
            }
        }

        GC.SuppressFinalize(this);
    }
}