using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class VaultTimerService : BackgroundService
{
    private readonly VaultClient _vaultClient;
    private readonly ILogger<VaultTimerService> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(1);

    public VaultTimerService(
        VaultClient vaultClient,
        ILogger<VaultTimerService> logger)
    {
        _vaultClient =
            vaultClient ?? throw new ArgumentNullException(nameof(vaultClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _logger.LogInformation("VaultTimerService constructed");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "VaultTimerService starting with interval: {Interval}", _interval);

        while (!stoppingToken.IsCancellationRequested)
        {
            await DoWorkAsync();
            await Task.Delay(_interval, stoppingToken);
        }
    }

    private async Task DoWorkAsync()
    {
        try
        {
            _logger.LogInformation(
                "Timer triggered at: {Time}", DateTimeOffset.Now);
            _logger.LogDebug("Attempting to renew Vault token");

            await _vaultClient.LookUpTokenAsync();
            var result = await _vaultClient.RenewTokenAsync();

            if (result)
            {
                _logger.LogInformation("Vault token renewed successfully");
            }
            else
            {
                _logger.LogWarning("Failed to renew Vault token");
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Token renewal operation was canceled");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex, "An error occurred while renewing the Vault token");
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("VaultTimerService is stopping");
        return base.StopAsync(cancellationToken);
    }
}