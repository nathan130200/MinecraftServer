using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MinecraftServer.Entities.Chat;

internal class TextSerializer : JsonConverter<Text>
{
    static readonly Dictionary<TextStyle, string> StyleKeyMapping = new()
    {
        [TextStyle.Bold] = "bold",
        [TextStyle.Italic] = "italic",
        [TextStyle.Underline] = "underline",
        [TextStyle.Strikethrough] = "strikethrough",
        [TextStyle.Obfuscated] = "obfuscated"
    };

    static readonly Dictionary<string, TextColor> TextColorMapping = new()
    {
        ["black"] = TextColor.Black,
        ["dark_blue"] = TextColor.DarkBlue,
        ["dark_green"] = TextColor.DarkGreen,
        ["dark_aqua"] = TextColor.DarkCyan,
        ["dark_red"] = TextColor.DarkRed,
        ["dark_purple"] = TextColor.DarkMagenta,
        ["gold"] = TextColor.DarkYellow,
        ["gray"] = TextColor.Gray,
        ["dark_gray"] = TextColor.DarkGray,
        ["blue"] = TextColor.Blue,
        ["green"] = TextColor.Green,
        ["aqua"] = TextColor.Cyan,
        ["red"] = TextColor.Red,
        ["light_purple"] = TextColor.Magenta,
        ["yellow"] = TextColor.Yellow,
        ["white"] = TextColor.White
    };

    public override void WriteJson(JsonWriter writer, Text value, JsonSerializer serializer)
    {
        var obj = new JObject();

        foreach (var (type, key) in StyleKeyMapping)
        {
            if (value.Style.HasFlag(type))
                obj[key] = true;
            else
                obj[key] = false;
        }

        string text = value.Value;

        if (string.IsNullOrEmpty(text))
            text = string.Empty;

        obj["text"] = text;

        if (value.Click != null)
            obj["clickEvent"] = JObject.FromObject(value.Click);

        obj["color"] = value.Color.Name;

        if (value.Hover != null)
            obj["hoverEvent"] = JObject.FromObject(value.Hover);

        if (value.Extra.Count > 0)
            obj["extra"] = JArray.FromObject(value.Extra);

        obj.WriteTo(writer);
    }

    public override Text ReadJson(JsonReader reader, Type objectType, Text existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var obj = JToken.ReadFrom(reader);

        var style = TextStyle.None;

        foreach (var (type, key) in StyleKeyMapping)
        {
            if (obj.Value<bool?>(key) == true)
                style |= type;
        }

        List<Text> extra = default;

        if (obj.Contains("extra"))
            extra = obj.Value<List<Text>>("extra");

        var result = new Text
        {
            Value = obj["text"]?.Value<string>(),
            Style = style,
            Click = obj["clickEvent"]?.ToObject<ClickComponent>(),
            Hover = obj["hoverEvent"]?.ToObject<HoverComponent>(),
            Extra = extra
        };

        if (obj["color"] != null)
        {
            var colorName = obj.Value<string>("color");

            if (TextColorMapping.TryGetValue(colorName, out var color))
                result.Color = color;
            else
                result.Color = TextColor.Gray;
        }

        return result;
    }
}
