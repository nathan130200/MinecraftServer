using System.Net;
using System.Net.Sockets;
using System.Reflection;
using MinecraftServer.IO;
using MinecraftServer.Protocol;
using MinecraftServer.Protocol.States;

namespace MinecraftServer.Net;

public class Client : IDisposable
{
    static IReadOnlyDictionary<ClientStateType, Func<Client, IClientState>> ClientStateFactory;

    static Client()
    {
        var temp = new Dictionary<ClientStateType, Func<Client, IClientState>>();

        foreach (var type in typeof(Client).Assembly.GetTypes()
            .Where(xt => xt.IsAssignableTo(typeof(IClientState)))
            .Where(xt => !xt.IsAbstract && xt.IsPublic))
        {
            var attr = type.GetCustomAttribute<StateAttribute>();

            if (attr == null)
            {
                Log.Warn("Type '" + type + "' does not implement correct state attribute for client state factory");
                continue;
            }

            var ctor = type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                new Type[] { typeof(Client) });

            if (ctor == null)
            {
                Log.Warn("Unable to find sutable ctor for client state in type '" + type + "'");
                continue;
            }

            temp[attr.Type] = (client) =>
            {
                return (IClientState)ctor.Invoke(new object[] { client });
            };
        }

        if (temp.Count == 0)
            Log.Error("Cannot find an client state implementations, server will not work correctly!");

        ClientStateFactory = temp;
    }

    private Func<Client, Task> onConnected;
    private Func<Client, Task> onDisconnected;
    private Socket socket;
    private Stream stream;
    private readonly CancellationTokenSource cts;
    private PacketQueue queue;

    public IPAddress RemoteAddress { get; }

    public ClientStateType CurrentState
    {
        get => State switch
        {
            HandshakeState => ClientStateType.Handshake,
            _ => ClientStateType.Disconnected
        };
    }

    internal IClientState State { get; set; }
    internal int CurrentProtocol { get; set; }

    public Client(Socket client)
    {
        socket = client;
        stream = new NetworkStream(socket);
        RemoteAddress = ((IPEndPoint)socket.RemoteEndPoint).Address;

        queue = new PacketQueue();
        cts = new CancellationTokenSource();
    }

    public event Func<Client, Task> Connected
    {
        add => onConnected += value;
        remove => onConnected += value;
    }

    public event Func<Client, Task> Disconnected
    {
        add => onDisconnected += value;
        remove => onDisconnected += value;
    }

    internal async Task ChangeStateAsync(ClientStateType type)
    {
        var oldState = State;

        if (oldState != null)
        {
            if (oldState is IDisposable d)
                d.Dispose();
        }

        await queue.FlushAsync();

        if(type != ClientStateType.Disconnected)
        {
            if (!ClientStateFactory.TryGetValue(type, out var func))
            {
                Log.Error("Client state '" + type + "' not implemented.");
                await CloseAsync();
                return;
            }

            State = func(this);

            Log.Debug("Client '" + RemoteAddress + "' change state to '" + type + "'");
        }
    }

    internal async Task OpenAsync()
    {
        State = new HandshakeState(this);

        _ = Task.Run(BeginSend);
        _ = Task.Run(BeginReceive);

        if (onConnected != null)
            await onConnected(this);
    }

    internal async Task CloseAsync()
    {
        if (cts.IsCancellationRequested)
            return;

        cts.Cancel();

        if (stream != null)
        {
            stream.Dispose();
            stream = null;
        }

        if (socket != null)
        {
            socket.Dispose();
            socket = null;
        }

        if (onDisconnected != null)
            await onDisconnected(this);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        CloseAsync().GetAwaiter().GetResult();
    }

    internal void CloseInternal()
        => Task.Run(CloseAsync);

    protected async Task BeginReceive()
    {
        try
        {
            while (cts != null && !cts.IsCancellationRequested)
            {
                var len = stream.ReadVarInt();

                if (len <= 0)
                    break;
                else
                {
                    if (len == 0xFA || len == 0xFE)
                    {
                        if (CurrentState == ClientStateType.Handshake)
                        {
                            Log.Warn($"Client '{RemoteAddress}' sent legacy list ping. Dropping connection...");
                            break;
                        }
                    }

                    var buffer = new byte[len];
                    await stream.ReadAsync(buffer);

                    var packet = new Packet(buffer);

                    if (State != null)
                        await State.HandlePacket(packet).ConfigureAwait(false);
                }

                await Task.Delay(1);
            }
        }
        catch (Exception ex)
        {
            Log.Warn("Recv failed. " + ex);
        }
        finally
        {
            CloseInternal();
        }
    }

    protected async Task BeginSend()
    {
        try
        {
            while (cts != null && !cts.IsCancellationRequested)
            {
                while (queue.TryDequeue(out var entry))
                {
                    try
                    {
                        if (entry.Packet != null)
                            stream.Write(entry.Packet.Serialize());
                    }
                    finally
                    {
                        entry.Dispose();
                    }

                    await Task.Delay(1);
                }

                await Task.Delay(1);
            }
        }
        catch (Exception ex)
        {
            Log.Warn("Send failed. " + ex);
        }

        CloseInternal();
    }

    internal Task SendAsync(Packet packet)
        => queue.EnqueueAsync(packet);

    internal void Send(Packet packet)
        => queue.Enqueue(packet);
}
