using Microsoft.Extensions.Logging;

namespace DataGateway.Abstractions;

public interface IDataGatewayApplication
{
    IServiceProvider Services { get; }
    VariableCollection Variables { get; }
    ILoggerFactory LoggerFactory { get; }
    Task StartAsync(CancellationToken cancellationToken = default);
    Task StopAsync(CancellationToken cancellationToken = default);
}