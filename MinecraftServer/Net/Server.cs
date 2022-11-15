using System.Net;
using System.Net.Sockets;

namespace MinecraftServer.Net;

public class Server
{
    private Socket socket;
    private IPEndPoint endpoint;
    private List<Client> clients;

    public Server(ushort port = 25565)
    {
        endpoint = new IPEndPoint(IPAddress.Any, port);
    }

    public Task StartAsync()
    {
        Log.Info("Start listen...");

        clients = new List<Client>();
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.Bind(endpoint);
        socket.Listen();
        _ = Task.Run(BeginAccept);
        return Task.CompletedTask;
    }

    protected async Task BeginAccept()
    {
        while (socket != null)
        {
            try
            {
                var client = await socket.AcceptAsync();

                if (client != null)
                    _ = Task.Run(async () => await EndAccept(client));
            }
            catch (Exception ex)
            {
                Log.Error("Accept failed -> " + ex.ToString());
            }

            await Task.Delay(1);
        }
    }

    protected async Task EndAccept(Socket socket)
    {
        var client = new Client(socket);

        client.Connected += (self) =>
        {
            lock (clients)
                clients.Add(self);

            return Task.CompletedTask;
        };

        client.Disconnected += (self) =>
        {
            lock (clients)
                clients.Remove(self);

            return Task.CompletedTask;
        };

        await client.OpenAsync();

        Log.Info("Client connected '" + client.RemoteAddress + "'");
    }
}
