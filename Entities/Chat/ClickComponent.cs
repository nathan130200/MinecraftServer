using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MinecraftServer.Entities.Chat;

public class ClickComponent
{
    [JsonProperty("action"), JsonRequired, JsonConverter(typeof(StringEnumConverter))]
    public ClickComponentAction Action { get; init; }

    [JsonProperty("value"), JsonRequired]
    protected object valueInternal = "";

    public object Value
    {
        get => valueInternal;
        init
        {
            if (value is not string or int)
                throw new InvalidOperationException("Component value type mismatch. Excepted string or number");

            valueInternal = value;
        }
    }
}
