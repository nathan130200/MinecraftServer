using System.Diagnostics;

namespace MinecraftServer.Entities.Chat;

[DebuggerDisplay("{Color,nq}")]
public readonly struct TextColor : IEquatable<TextColor>
{
    public char Type { get; }
    public string Name { get; }
    public ConsoleColor Color { get; }

    TextColor(char type, string name, ConsoleColor color)
    {
        Type = type;
        Name = name;
        Color = color;
    }

    public static readonly TextColor Black = new('0', "black", ConsoleColor.Black);
    public static readonly TextColor DarkBlue = new('1', "dark_blue", ConsoleColor.DarkBlue);
    public static readonly TextColor DarkGreen = new('2', "dark_green", ConsoleColor.DarkGreen);
    public static readonly TextColor DarkCyan = new('3', "dark_aqua", ConsoleColor.DarkCyan);
    public static readonly TextColor DarkRed = new('4', "dark_red", ConsoleColor.DarkRed);
    public static readonly TextColor DarkMagenta = new('5', "dark_purple", ConsoleColor.DarkMagenta);
    public static readonly TextColor DarkYellow = new('6', "gold", ConsoleColor.DarkYellow);
    public static readonly TextColor Gray = new('7', "gray", ConsoleColor.Gray);
    public static readonly TextColor DarkGray = new('8', "dark_gray", ConsoleColor.DarkGray);
    public static readonly TextColor Blue = new('9', "blue", ConsoleColor.Blue);
    public static readonly TextColor Green = new('a', "green", ConsoleColor.Green);
    public static readonly TextColor Cyan = new('b', "aqua", ConsoleColor.Cyan);
    public static readonly TextColor Red = new('c', "red", ConsoleColor.Red);
    public static readonly TextColor Magenta = new('d', "light_purple", ConsoleColor.Magenta);
    public static readonly TextColor Yellow = new('e', "yellow", ConsoleColor.Yellow);
    public static readonly TextColor White = new('f', "white", ConsoleColor.White);

    public override int GetHashCode()
        => Type.GetHashCode();

    public override bool Equals(object obj)
        => obj is TextColor other && Equals(other);

    public override string ToString()
        => $"\U000000A7{Type}";

    public bool Equals(TextColor other)
    {
        return Type == other.Type;
    }

    public static bool operator ==(TextColor left, TextColor right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(TextColor left, TextColor right)
    {
        return !(left == right);
    }
}
