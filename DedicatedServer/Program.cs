using System;
using System.IO;
using System.Threading.Tasks;
using Minecraft.IO;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Verbose()
    .WriteTo.Console()
    .CreateLogger();


using (var ms = new MemoryStream())
{
    var n = 12345U;
    Primitive.Write(ms, n);

    var buf = ms.ToArray();
    Console.WriteLine("{0} = uint8[{1:X2},{2:X2},{3:X2},{4:X2}] (0x{5})", n, buf[0], buf[1], buf[2], buf[3], Convert.ToHexString(buf));

    ms.Position = 0;
    var o = Primitive.Read<uint>(ms);

    Console.WriteLine("{0} == {1} -> {2}", n, o, n == o);
}

await Task.Delay(-1);