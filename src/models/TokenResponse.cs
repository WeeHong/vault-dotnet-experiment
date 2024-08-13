using Newtonsoft.Json;

public class TokenResponse
{
    [JsonProperty("auth")]
    public required AuthData Auth { get; init; } = new();

    public record AuthData
    {
        [JsonProperty("client_token")]
        public string ClientToken { get; init; } = string.Empty;
    }
}