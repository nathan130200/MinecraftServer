using System;
using System.Diagnostics;

namespace Minecraft.Entities;

[DebuggerDisplay("X={X}; Y={Y}; Z={Z}")]
public struct BlockPosition : IEquatable<BlockPosition>
{
    public int X
    {
        readonly get;
        set;
    }

    public int Y
    {
        readonly get;
        set;
    }

    public int Z
    {
        readonly get;
        set;
    }

    public BlockPosition() => X = Y = Z = 0;
    public BlockPosition(int value) => X = Y = Z = value;
    public BlockPosition(int x, int y, int z) => (X, Y, Z) = (x, y, z);

    public override bool Equals(object obj)
        => obj is BlockPosition other && Equals(other);

    public bool Equals(BlockPosition other)
        => X == other.X && Y == other.Y && Z == other.Z;

    public override int GetHashCode()
        => HashCode.Combine(X, Y, Z);

    public static bool operator ==(BlockPosition lhs, BlockPosition rhs)
       => lhs.Equals(rhs);

    public static bool operator !=(BlockPosition lhs, BlockPosition rhs)
        => !(lhs == rhs);

    public static implicit operator BlockPosition(Position pos) => new((int)pos.X, (int)pos.Y, (int)pos.Z);
    public static BlockPosition operator +(BlockPosition a, BlockPosition b) => new(a.X + b.Y, a.Y + b.Y, a.Z + b.Z);
    public static BlockPosition operator +(BlockPosition a, int b) => new(a.X + b, a.Y + b, a.Z + b);
    public static BlockPosition operator -(BlockPosition a, BlockPosition b) => new(a.X - b.Y, a.Y - b.Y, a.Z - b.Z);
    public static BlockPosition operator -(BlockPosition a, int b) => new(a.X - b, a.Y - b, a.Z - b);
    public static BlockPosition operator *(BlockPosition a, BlockPosition b) => new(a.X * b.Y, a.Y * b.Y, a.Z * b.Z);
    public static BlockPosition operator *(BlockPosition a, int b) => new(a.X * b, a.Y * b, a.Z * b);
    public static BlockPosition operator /(BlockPosition a, BlockPosition b) => new(a.X / b.Y, a.Y / b.Y, a.Z / b.Z);
    public static BlockPosition operator /(BlockPosition a, int b) => new(a.X / b, a.Y / b, a.Z / b);

    // ============================================================================================ //

    public static BlockPosition operator +(BlockPosition a, Position b) => new(a.X + (int)b.X, a.Y + (int)b.Y, a.Z + (int)b.Z);
    public static BlockPosition operator -(BlockPosition a, Position b) => new(a.X - (int)b.X, a.Y - (int)b.Y, a.Z - (int)b.Z);
    public static BlockPosition operator *(BlockPosition a, Position b) => new(a.X * (int)b.X, a.Y * (int)b.Y, a.Z * (int)b.Z);
    public static BlockPosition operator /(BlockPosition a, Position b) => new(a.X / (int)b.X, a.Y / (int)b.Y, a.Z / (int)b.Z);

    // ============================================================================================ //

    public static BlockPosition Zero => new(0);
    public static BlockPosition One => new(1);
    public static BlockPosition Up => new(0, 1, 0);
    public static BlockPosition Down => new(0, -1, 0);
    public static BlockPosition Left => new(-1, 0, 0);
    public static BlockPosition Right => new(1, 0, 0);
    public static BlockPosition Backward => new(0, 0, -1);
    public static BlockPosition Forward => new(0, 0, 1);
    public static BlockPosition East => new(1, 0, 0);
    public static BlockPosition West => new(-1, 0, 0);
    public static BlockPosition North => new(0, 0, -1);
    public static BlockPosition South => new(0, 0, 1);
}
