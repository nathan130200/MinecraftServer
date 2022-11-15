namespace MinecraftServer.Entities.Chat;

[Flags]
public enum TextStyle
{
    None = 0,
    Bold = 1 << 1,
    Italic = 1 << 2,
    Underline = 1 << 3,
    Strikethrough = 1 << 4,
    Obfuscated = 1 << 5
}
