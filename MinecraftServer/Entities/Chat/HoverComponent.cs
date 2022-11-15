using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MinecraftServer.Entities.Chat;

public class HoverComponent
{
    [JsonProperty("action"), JsonRequired, JsonConverter(typeof(StringEnumConverter))]
    public HoverComponentAction Action { get; init; }

    [JsonProperty("value"), JsonRequired]
    private object valueInternal = "";

    [JsonIgnore]
    public object Value
    {
        get => valueInternal;
        init
        {
            if (value is not string or Text)
                throw new InvalidOperationException("Component value type mismatch. Excepted string or text component");

            valueInternal = value;
        }
    }
}
