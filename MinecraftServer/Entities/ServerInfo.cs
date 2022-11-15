using MinecraftServer.Entities.Chat;
using Newtonsoft.Json;

namespace MinecraftServer.Entities;

public class ServerInfo
{
    [JsonProperty("version")]
    public VersionInfo Version { get; set; } = new("1.8", 47);

    [JsonProperty("players")]
    public PlayersInfo Players { get; } = new();

    [JsonProperty("description")]
    public Text Description { get; set; } = "A Minecraft Server";

    [JsonProperty("favicon", NullValueHandling = NullValueHandling.Ignore)]
    public string Favicon { get; set; }

    [JsonProperty("previewsChat", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public bool PreviewsChat { get; set; } = true;

    [JsonProperty("enforcesSecureChat", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public bool EnforcesSecureChat { get; set; } = false;

    // ------------------------------------------------------------------------ //

    public record class VersionInfo
    {
        [JsonProperty("name")]
        public string Name { get; init; }

        [JsonProperty("protocol")]
        public int Protocol { get; init; }

        public VersionInfo(string name, int protocol)
        {
            Name = name;
            Protocol = protocol;
        }
    }

    public class PlayersInfo
    {
        [JsonProperty("max")]
        public int Max { get; set; } = 9999;

        [JsonProperty("online")]
        public int Online { get; set; } = 0;

        [JsonProperty("sample", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<PlayerSampleInfo> Sample { get; set; } = default;


        protected bool ShouldSerializeSample()
            => Sample?.Count > 0;

        public class PlayerSampleInfo
        {
            [JsonProperty("id"), JsonRequired]
            public string Id { get; set; }

            [JsonProperty("name"), JsonRequired]
            public string Name { get; set; }
        }
    }
}

