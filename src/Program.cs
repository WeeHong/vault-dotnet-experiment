using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host =
    Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration(
            (hostingContext, config) =>
            {
                config.AddJsonFile(
                    "appsettings.json", optional: true, reloadOnChange: true);
                config.AddEnvironmentVariables();
            })
        .ConfigureServices(
            (hostContext, services) =>
            {
                services.AddHttpClient<VaultClient>();
                services.AddHostedService<VaultTimerService>();
                services.AddHostedService<StartupService>();
            })
        .Build();

await host.RunAsync();

public class StartupService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<StartupService> _logger;

    public StartupService(
        IServiceProvider serviceProvider,
        ILogger<StartupService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var vaultClient =
            scope.ServiceProvider.GetRequiredService<VaultClient>();

        try
        {
            _logger.LogInformation("Fetching key-value secret from Vault");
            var kv = await vaultClient.GetKeyValueSecret();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching key-value secret from Vault");
        }
    }

    public Task
    StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}