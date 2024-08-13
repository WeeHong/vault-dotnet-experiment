using System.Net.Security;
using Newtonsoft.Json;

public class SecretIdResponse
{
    [JsonProperty("data")]
    public required SecretData Data { get; init; } = new();

    public record SecretData
    {
        [JsonProperty("secret_id")]
        public string SecretId { get; init; } = string.Empty;
    }
}