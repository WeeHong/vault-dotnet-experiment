using System.Numerics;
using Newtonsoft.Json;

public class VaultKeyValueResponse
{
    [JsonProperty("data")]
    public required KeyValueData Data { get; init; } = new();
    public record KeyValueData
    {
        [JsonProperty("data")]
        public SecretData Data { get; init; } = new();
        public record SecretData
        {
            [JsonProperty("database")]
            public DatabaseData Database { get; init; } = new();
            public record DatabaseData
            {
                [JsonProperty("host")]
                public string Host { get; init; } = string.Empty;
                [JsonProperty("port")]
                public int Port {
                    get; init;
                } = 0;
                [JsonProperty("user")]
                public string User {
                    get; init;
                } = string.Empty;
                [JsonProperty("pass")]
                public string Pass {
                    get; init;
                } = string.Empty;
                [JsonProperty("name")]
                public string Name {
                    get; init;
                } = string.Empty;
            }
        }
    }
}