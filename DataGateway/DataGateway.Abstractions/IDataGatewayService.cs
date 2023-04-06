namespace DataGateway.Abstractions;

public interface IDataGatewayService
{
    string Name { get; }
    Task StartAsync(IDataGatewayApplication application, CancellationToken cancellationToken = default);
    Task StopAsync(CancellationToken cancellationToken = default);
}