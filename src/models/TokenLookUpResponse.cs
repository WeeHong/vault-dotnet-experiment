using Newtonsoft.Json;

public class TokenLookUpResponse
{
  [JsonProperty("data")]
  public required TokenData Data { get; init; } = new();

  public record TokenData
  {
    [JsonProperty("ttl")]
    public int TTL { get; init; } = 0;
  }
}