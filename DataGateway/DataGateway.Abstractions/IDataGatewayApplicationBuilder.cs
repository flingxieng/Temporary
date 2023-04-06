using Microsoft.Extensions.DependencyInjection;

namespace DataGateway.Abstractions;

public interface IDataGatewayApplicationBuilder
{
    IServiceCollection Services { get; }
    IDataGatewayApplicationBuilder UseService(Type type);
    IDataGatewayApplicationBuilder UseService<T>() where T : IDataGatewayService;
}