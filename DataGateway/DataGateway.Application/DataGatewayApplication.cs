using DataGateway.Abstractions;
using Microsoft.Extensions.Logging;

namespace DataGateway.Application;

internal sealed class DataGatewayApplication : IDataGatewayApplication
{
    private readonly IVariableProvider _variableProvider;
    private readonly IDataGatewayService[] _services;
    private readonly ILogger<DataGatewayApplication> _logger;
    public IServiceProvider Services { get; }
    public VariableCollection Variables { get; private set; } = VariableCollection.Empty;
    public ILoggerFactory LoggerFactory { get; }

    public DataGatewayApplication(
        ILoggerFactory loggerFactory,
        IServiceProvider services,
        IVariableProvider variableProvider,
        IEnumerable<IDataGatewayService> gatewayServices)
    {
        Services = services;
        LoggerFactory = loggerFactory;
        _variableProvider = variableProvider;
        _services = gatewayServices.ToArray();
        _logger = loggerFactory.CreateLogger<DataGatewayApplication>();
    }
    
    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        Variables = await _variableProvider.ProvideAsync(cancellationToken);
        foreach (var service in _services)
        {
            try
            {
                await service.StartAsync(this, cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "服务'{Name}'启动失败", service.Name);
            }
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        foreach (var service in _services)
        {
            try
            {
                await service.StopAsync(cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "服务'{Name}'停止失败", service.Name);
            }
        }
    }
}