using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Reflection;

internal class ConsoleHostedService : IHostedService
{
    private readonly ILogger _logger;
    private readonly IHostApplicationLifetime _appLifetime;
    private readonly IConverterController _app;
    
    public ConsoleHostedService(ILogger<ConsoleHostedService> logger, IHostApplicationLifetime appLifetime, IConverterController app)
    {
        _logger = logger;
        _appLifetime = appLifetime;
        _app = app;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _appLifetime.ApplicationStarted.Register(() =>
        {
            Task.Run(async () =>
            {
                string? appName = Assembly.GetExecutingAssembly().GetName().Name;

                try
                {
                    await _app.RunAsync();

                    _logger.LogDebug("Process complete");

                    _appLifetime.StopApplication();
                }
                catch (PathException ex)
                {
                    _logger.LogCritical($"Process exited with Path exception: {ex.Message}");

                    Environment.Exit(-1);
                }
                catch (JsonParsingException ex)
                {
                    _logger.LogCritical(ex.JsonParsingErrorMessage);

                    Environment.Exit(-1);
                }
                catch (Exception ex)
                {
                    _logger.LogCritical($"Process exited with exception: {ex.Message}");

                    Environment.Exit(-1);
                }
            });
        });

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}

