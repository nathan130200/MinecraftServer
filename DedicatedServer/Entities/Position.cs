using System;
using System.Diagnostics;

namespace Minecraft.Entities;

[DebuggerDisplay("X={X}; Y={Y}; Z={Z}")]
public struct Position : IEquatable<Position>
{
    public float X
    {
        readonly get;
        set;
    }

    public float Y
    {
        readonly get;
        set;
    }

    public float Z
    {
        readonly get;
        set;
    }

    public Position() => X = Y = Z = 0;
    public Position(float value) => X = Y = Z = value;
    public Position(float x, float y, float z) => (X, Y, Z) = (x, y, z);

    public override bool Equals(object obj)
        => obj is Position other && Equals(other);

    public bool Equals(Position other)
        => IsNear(X, other.X) && IsNear(Y, other.Y) && IsNear(Z, other.Z);

    public override int GetHashCode()
        => HashCode.Combine(X, Y, Z);

    static bool IsNear(float a, float b)
        => MathF.Abs(a - b) <= 0.01f;

    public override string ToString()
        => $"{X:0.0}:{Y:0.0}:{Z:0.0}";

    public static bool operator ==(Position a, Position b)
        => a.Equals(b);

    public static bool operator !=(Position a, Position b)
        => !(a == b);

    public static implicit operator Position(BlockPosition blockPos)
      => new(blockPos.X, blockPos.Y, blockPos.Z);

    public static Position operator +(Position a, Position b) => new(a.X + b.Y, a.Y + b.Y, a.Z + b.Z);
    public static Position operator +(Position a, float b) => new(a.X + b, a.Y + b, a.Z + b);
    public static Position operator -(Position a, Position b) => new(a.X - b.Y, a.Y - b.Y, a.Z - b.Z);
    public static Position operator -(Position a, float b) => new(a.X - b, a.Y - b, a.Z - b);
    public static Position operator *(Position a, Position b) => new(a.X * b.Y, a.Y * b.Y, a.Z * b.Z);
    public static Position operator *(Position a, float b) => new(a.X * b, a.Y * b, a.Z * b);
    public static Position operator /(Position a, Position b) => new(a.X / b.Y, a.Y / b.Y, a.Z / b.Z);
    public static Position operator /(Position a, float b) => new(a.X / b, a.Y / b, a.Z / b);

    // ============================================================================================ //

    public static Position operator +(Position a, BlockPosition b) => new(a.X + b.Y, a.Y + b.Y, a.Z + b.Z);
    public static Position operator -(Position a, BlockPosition b) => new(a.X - b.Y, a.Y - b.Y, a.Z - b.Z);
    public static Position operator *(Position a, BlockPosition b) => new(a.X * b.Y, a.Y * b.Y, a.Z * b.Z);
    public static Position operator /(Position a, BlockPosition b) => new(a.X / b.Y, a.Y / b.Y, a.Z / b.Z);

    // ============================================================================================ //

    public static Position Zero => new(0f);
    public static Position One => new(1f);
    public static Position Up => new(0f, 1f, 0f);
    public static Position Down => new(0f, -1f, 0f);
    public static Position Left => new(-1f, 0f, 0f);
    public static Position Right => new(1f, 0f, 0f);
    public static Position Backward => new(0f, 0f, -1f);
    public static Position Forward => new(0f, 0f, 1f);
    public static Position East => new(1f, 0f, 0f);
    public static Position West => new(-1f, 0f, 0f);
    public static Position North => new(0f, 0f, -1f);
    public static Position South => new(0f, 0f, 1f);
}
