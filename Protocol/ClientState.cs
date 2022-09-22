using MinecraftServer.IO;
using MinecraftServer.Net;

namespace MinecraftServer.Protocol;

public enum ClientStateType
{
    Disconnected = byte.MaxValue,
    Handshake = 0,
    Status = 1,
    Login = 2,
    Play = 3
}

[AttributeUsage(AttributeTargets.Class)]
public sealed class StateAttribute : Attribute
{
    public readonly ClientStateType Type;

    public StateAttribute(ClientStateType type)
        => Type = type;
}

public abstract class ClientState : IClientState
{
    protected Client Client { get; private set; }

    public ClientState(Client client)
        => Client = client;

    protected virtual Task HandlePacket(Packet packet)
    {
        return Task.CompletedTask;
    }

    Task IClientState.HandlePacket(Packet packet)
        => HandlePacket(packet);
}

public interface IClientState
{
    Task HandlePacket(Packet packet);
}