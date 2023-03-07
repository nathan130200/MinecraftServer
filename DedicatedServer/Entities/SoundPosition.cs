namespace Minecraft.Entities;

public readonly struct SoundPosition
{
    public int X { readonly get; init; }
    public int Y { readonly get; init; }
    public int Z { readonly get; init; }

    public SoundPosition()
    {

    }

    public SoundPosition(int x, int y, int z)
    {
        X = x * 8;
        Y = y * 8;
        Z = z * 8;
    }

    public SoundPosition(double x, double y, double z)
    {
        X = (int)(x * 8);
        Y = (int)(y * 8);
        Z = (int)(z * 8);
    }
}
