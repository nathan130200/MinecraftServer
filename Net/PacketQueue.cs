using System.Collections.Concurrent;

namespace MinecraftServer.Net;

public class PacketQueue
{
    private ConcurrentQueue<Entry> queue;

    public PacketQueue()
        => queue = new ConcurrentQueue<Entry>();

    public Task FlushAsync()
    {
        var tcs = new TaskCompletionSource();

        queue.Enqueue(new Entry
        {
            Completition = tcs
        });

        return tcs.Task;
    }

    public void Enqueue(Packet packet)
    {
        queue.Enqueue(new Entry
        {
            Packet = packet
        });
    }

    public Task EnqueueAsync(Packet packet)
    {
        var tcs = new TaskCompletionSource();

        queue.Enqueue(new Entry
        {
            Packet = packet,
            Completition = tcs
        });

        return tcs.Task;
    }

    internal bool TryDequeue(out Entry result)
        => queue.TryDequeue(out result);

    public bool TryDequeue(out Packet packet, out TaskCompletionSource tcs)
    {
        packet = default;
        tcs = default;

        var result = queue.TryDequeue(out var temp);

        if (result)
        {
            packet = temp.Packet;
            tcs = temp.Completition;
        }

        return result;
    }

    internal class Entry : IDisposable
    {
        public Packet Packet { get; set; }
        public TaskCompletionSource Completition { get; set; }

        public void Dispose()
        {
            GC.SuppressFinalize(this);

            if (Packet != null)
            {
                Packet.Dispose();
                Packet = null;
            }

            if (Completition != null)
            {
                Completition.TrySetResult();
                Completition = null;
            }
        }
    }
}
