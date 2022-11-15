using MinecraftServer.IO;
using MinecraftServer.Net;

namespace MinecraftServer.Protocol.States;

[State(ClientStateType.Handshake)]
public class HandshakeState : ClientState
{
    public HandshakeState(Client c) : base(c)
    {

    }

    protected override async Task HandlePacket(Packet packet)
    {
        switch (packet.Type)
        {
            case 0x00:
            {
                var protocolNum = packet.ReadVarInt();
                var serverHost = packet.ReadString();
                var serverPort = packet.ReadUInt16();
                var nextState = (ClientStateType)packet.ReadVarInt();

                Log.Debug($"Client '{Client.RemoteAddress}' is connecting to {serverHost}:{serverPort}");

                Client.CurrentProtocol = protocolNum;

                if (nextState == ClientStateType.Login || nextState == ClientStateType.Status)
                    await Client.ChangeStateAsync(nextState);
                else
                {
                    Log.Debug($"Client '{Client.RemoteAddress}' cannot direct goto state '{nextState}'");
                    await Client.CloseAsync();
                }
            }
            break;

            default:
            {
                Log.Warn("Unknown packet at handshake state: " + packet.Type);
                await Client.CloseAsync();
            }
            break;
        }
    }
}

[State(ClientStateType.Login)]
public class LoginState : ClientState
{
    public LoginState(Client client) : base(client)
    {

    }

    protected override async Task HandlePacket(Packet packet)
    {
        await Task.Yield();
    }
}