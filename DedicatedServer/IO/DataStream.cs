using System;
using System.Diagnostics;
using System.IO;

namespace Minecraft.IO;

[DebuggerDisplay("Offset = {Position}; Count = {Length}")]
public class DataStream : IDisposable
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    protected Stream _baseStream;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    protected readonly bool _leaveOpen;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    protected readonly object _syncLock = new();

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    protected volatile bool _disposed;

    public long Length
    {
        get => _baseStream?.Length ?? 0;
        set => _baseStream.SetLength(value);
    }

    public long Position
    {
        get => _baseStream?.Position ?? 0;
        set => _baseStream.Position = value;
    }

    public DataStream(Stream stream, bool leaveOpen = true)
    {
        _baseStream = stream;
        _leaveOpen = leaveOpen;
    }

    public DataStream(byte[] buffer) : this()
    {
        _baseStream.Write(buffer, 0, buffer.Length);
        _baseStream.Position = 0;
    }

    public DataStream()
    {
        _baseStream = new MemoryStream();
    }

    ~DataStream()
    {
        Dispose();
    }

    public virtual byte[] GetBytes()
    {
        ThrowIfDisposed();

        lock (_syncLock)
        {
            if (_baseStream is not MemoryStream)
                throw new InvalidOperationException("Stream is not subclass of '" + typeof(MemoryStream).FullName + "'...");

            var pos = _baseStream.Position;
            _baseStream.Position = 0;

            var buffer = new byte[_baseStream.Length];
            _baseStream.Read(buffer, 0, buffer.Length);
            _baseStream.Position = pos;

            return buffer;
        }
    }

    public void CopyTo(DataStream dest, uint bufferSize = 4096)
        => CopyTo(dest._baseStream, bufferSize);

    public void CopyTo(Stream dest, uint bufferSize = 4096)
    {
        ThrowIfDisposed();

        lock (_syncLock)
        {
            var buff = new byte[bufferSize];
            int len;

            long pos = 0;

            if (_baseStream.CanSeek)
            {
                pos = _baseStream.Position;
                _baseStream.Position = 0;
            }

            while ((len = _baseStream.Read(buff, 0, buff.Length)) > 0)
            {
                dest.Write(buff, 0, len);
                ThrowIfDisposed();
            }

            if (_baseStream.CanSeek)
                _baseStream.Position = pos;

            dest.Flush();
        }
    }

    protected void ThrowIfDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(GetType().FullName);
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;

        lock (_syncLock)
        {
            if (_baseStream != null)
            {
                if (!_leaveOpen)
                    _baseStream.Dispose();

                _baseStream = null;
            }
        }

        GC.SuppressFinalize(this);
    }
}