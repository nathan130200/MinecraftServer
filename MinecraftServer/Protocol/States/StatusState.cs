using MinecraftServer.Entities;
using MinecraftServer.IO;
using MinecraftServer.Net;
using Newtonsoft.Json;

namespace MinecraftServer.Protocol.States;

[State(ClientStateType.Status)]
public class StatusState : ClientState
{
    public StatusState(Client client) : base(client)
    {

    }

    bool isMotdSent = false;

    protected override async Task HandlePacket(Packet packet)
    {
        switch (packet.Type)
        {
            case 0x00 when !isMotdSent:
            {
                var serverInfo = new ServerInfo
                {
                    Description = "A Minecraft Server",
                };

                var json = JsonConvert.SerializeObject(serverInfo);

                packet = new Packet(0x00);
                packet.WriteString(json);
                await Client.SendAsync(packet);
                isMotdSent = true;
            }
            break;

            case 0x01 when isMotdSent:
            {
                await Client.SendAsync(packet);
            }
            break;
        }
    }
}