using System;
using System.IO;

namespace Minecraft.IO;

public class DataStream : IDisposable
{
    protected Stream m_BaseStream;
    protected readonly bool m_LeaveOpen;
    protected readonly object m_SyncLock = new();
    volatile bool m_Disposed;

    public DataStream(Stream stream, bool leaveOpen = true)
    {
        m_BaseStream = stream;
        m_LeaveOpen = leaveOpen;
    }

    public DataStream(byte[] buffer) : this()
    {
        m_BaseStream.Write(buffer, 0, buffer.Length);
        m_BaseStream.Position = 0;
    }

    public DataStream()
    {
        m_BaseStream = new MemoryStream();
    }

    ~DataStream()
    {
        Dispose();
    }

    public virtual byte[] GetBytes()
    {
        ThrowIfDisposed();

        lock (m_SyncLock)
        {
            if (m_BaseStream is not MemoryStream)
                throw new InvalidOperationException("Stream is not subclass of '" + typeof(MemoryStream).FullName + "'...");

            var pos = m_BaseStream.Position;
            m_BaseStream.Position = 0;

            var buffer = new byte[m_BaseStream.Length];
            m_BaseStream.Read(buffer, 0, buffer.Length);
            m_BaseStream.Position = pos;

            return buffer;
        }
    }

    public void CopyTo(DataStream dest, uint bufferSize = 4096)
        => CopyTo(dest.m_BaseStream, bufferSize);

    public void CopyTo(Stream dest, uint bufferSize = 4096)
    {
        ThrowIfDisposed();

        lock (m_SyncLock)
        {
            var buff = new byte[bufferSize];
            int len;

            long pos = 0;

            if (m_BaseStream.CanSeek)
            {
                pos = m_BaseStream.Position;
                m_BaseStream.Position = 0;
            }

            while ((len = m_BaseStream.Read(buff, 0, buff.Length)) > 0)
            {
                dest.Write(buff, 0, len);
                ThrowIfDisposed();
            }

            if (m_BaseStream.CanSeek)
                m_BaseStream.Position = pos;

            dest.Flush();
        }
    }

    protected void ThrowIfDisposed()
    {
        if (m_Disposed)
            throw new ObjectDisposedException(GetType().FullName);
    }

    public void Dispose()
    {
        ThrowIfDisposed();

        m_Disposed = true;

        lock (m_SyncLock)
        {
            if (m_BaseStream != null)
            {
                if (!m_LeaveOpen)
                    m_BaseStream.Dispose();

                m_BaseStream = null;
            }
        }

        GC.SuppressFinalize(this);
    }
}