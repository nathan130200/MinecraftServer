using System;

namespace Minecraft.Entities;

public struct Velocity
{
    public short X { readonly get; set; }
    public short Y { readonly get; set; }
    public short Z { readonly get; set; }

    public float Magnitude
        => MathF.Sqrt(X * X + Y + Y + Z * Z);

    public Velocity(short x, short y, short z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public static Velocity FromBlockPerSecond(float x, float y, float z)
        => new((short)(400f * x), (short)(400f * y), (short)(400f * z));

    public static Velocity FromBlockPerTick(float x, float y, float z)
        => new((short)(8000f * x), (short)(8000f * y), (short)(8000f * z));

    public static Velocity FromPosition(Position pos)
        => FromBlockPerSecond(pos.X, pos.Y, pos.Z);

    public static Velocity FromDirection(Position from, Position to)
        => FromPosition(to - from);
}