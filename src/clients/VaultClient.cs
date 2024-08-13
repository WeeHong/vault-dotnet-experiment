using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

public class VaultClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<VaultClient> _logger;
    private const string DefaultVaultAddress = "http://localhost:8200";

    public VaultClient(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<VaultClient> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;

        InitializeClient();
    }

    public VaultClient()
        : this(
              new HttpClient(),
              new ConfigurationBuilder().Build(),
              LoggerFactory.Create(builder => builder.AddConsole())
                  .CreateLogger<VaultClient>())
    {
    }

    private void InitializeClient()
    {
        _httpClient.BaseAddress =
            new Uri(_configuration["Vault:Address"] ?? DefaultVaultAddress);
        _logger.LogInformation(
            "VaultClient initialized with base address: {BaseAddress}",
            _httpClient.BaseAddress);

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer", _configuration["Vault:DefaultToken"]);
    }

    public async Task LookUpTokenAsync()
    {
        _logger.LogInformation("Attempting to lookup Vault token.");

        var response = await SendRequestAsync<TokenLookUpResponse>(
                    HttpMethod.Get, "/v1/auth/token/lookup-self");

        if (response is null)
            _logger.LogError("Failed to lookup Vault token.");

        _logger.LogInformation("Current Token with TTL: {TTL}", response?.Data.TTL.ToString() ?? "TTL is not available");
    }

    public async Task<bool> RenewTokenAsync()
    {
        _logger.LogInformation("Attempting to renew Vault token.");

        var payload = new { increment = "1h" };

        var response = await SendRequestAsync<TokenResponse>(
            HttpMethod.Post, "/v1/auth/token/renew-self", payload);

        if (response is null)
            return false;

        return true;
    }

    public async Task<VaultKeyValueResponse?> GetKeyValueSecret()
    {
        _logger.LogInformation("Attempting to fetch KV from Vault.");

        var secretId = await GenerateSecretId();
        var appRoleToken = await GenerateAppRoleToken(secretId);

        if (!string.IsNullOrEmpty(appRoleToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new("Bearer", appRoleToken);
        }

        return await FetchSecretFromVault();
    }

    private async Task<string> GenerateAppRoleToken(string secretId)
    {
        _logger.LogInformation("Attempting to generate AppRole token.");

        var roleId = _configuration["Vault:RoleId"] ??
                     throw new InvalidOperationException(
                         "Failed to fetch RoleId from configuration file.");

        var payload = new { role_id = roleId, secret_id = secretId };

        var response = await SendRequestAsync<TokenResponse>(
            HttpMethod.Post, "/v1/auth/approle/login", payload);

        if (response is null)
            return string.Empty;

        _logger.LogInformation("AppRole token generated successfully.");

        return response.Auth.ClientToken;
    }

    private async Task<T?> SendRequestAsync<T>(
        HttpMethod httpMethod,
        string path,
        object? payload = null)
    {
        using var request = new HttpRequestMessage(httpMethod, path);

        if (payload is not null)
        {
            request.Content = new StringContent(
                JsonConvert.SerializeObject(payload),
                Encoding.UTF8,
                MediaTypeNames.Application.Json);
        }

        _logger.LogInformation(
            "Sending {Method} request to Vault: {Path}", httpMethod, path);
        using var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            _logger.LogError(
                "HTTP request to Vault failed. Method: {Method}, Path: " +
                    "{Path}, Status Code: {StatusCode}, Error: {ErrorMessage}",
                httpMethod,
                path,
                response.StatusCode,
                errorMessage);
            return default;
        }

        var content = await response.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<T>(content) ?? default;
    }

    private async Task<string> GenerateSecretId()
    {
        _logger.LogInformation("Attempting to generate secret ID.");

        var payload = new
        {
            metadata = JsonConvert.SerializeObject(new { tag = "development" }),
            ttl = 600,
            num_uses = 50
        };
        var response = await SendRequestAsync<SecretIdResponse>(
            HttpMethod.Post,
            "v1/auth/approle/role/developer/secret-id",
            payload);

        return response?.Data.SecretId ?? string.Empty;
    }

    private async Task<VaultKeyValueResponse?> FetchSecretFromVault()
    {
        _logger.LogInformation("Attempting to fetch secret value from Vault.");

        var response = await SendRequestAsync<VaultKeyValueResponse>(
            HttpMethod.Get, "v1/secret/data/demo");

        if (response is null)
        {
            _logger.LogError("Failed to fetch secret from Vault");
        }

        return response;
    }
}
