using MinecraftServer;
using MinecraftServer.Entities;
using MinecraftServer.Entities.Chat;
using MinecraftServer.Net;
using Newtonsoft.Json;

var server = new Server();
await server.StartAsync();

var sample = new Text
{
    Value = "A Minecraft Server",
    Style = TextStyle.Italic,
    Color = TextColor.Gray
};

var info = new ServerInfo
{
    Description = sample
};

while (true)
{
    if (Console.KeyAvailable)
    {
        var keyinfo = Console.ReadKey(true);

        if (keyinfo.Key == ConsoleKey.Escape)
            break;
    }

    await Task.Delay(1);
}