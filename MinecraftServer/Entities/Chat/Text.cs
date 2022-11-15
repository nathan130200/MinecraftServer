using Newtonsoft.Json;

namespace MinecraftServer.Entities.Chat;

[JsonConverter(typeof(TextSerializer))]
public class Text
{
    public string Value { get; set; }
    public TextColor Color { get; set; } = TextColor.Gray;
    public TextStyle Style { get; set; } = TextStyle.None;
    public ClickComponent Click { get; init; }
    public HoverComponent Hover { get; init; }
    public List<Text> Extra { get; init; } = new();

    public Text AddExtra(Text component)
    {
        Extra.Add(component);
        return this;
    }

    public Text AddExtra(string text)
    {
        Extra.Add(new Text { Value = text });
        return this;
    }

    public Text AddExtra(string text, TextColor color)
    {
        Extra.Add(new Text { Value = text, Color = color });
        return this;
    }

    public static implicit operator Text(string value)
        => new() { Value = value };
}
