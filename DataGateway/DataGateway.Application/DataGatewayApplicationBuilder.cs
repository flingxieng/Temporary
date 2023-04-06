using DataGateway.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DataGateway.Application;

public class DataGatewayApplicationBuilder : IDataGatewayApplicationBuilder
{
    public IServiceCollection Services { get; }

    public IDataGatewayApplicationBuilder UseService(Type type)
    {
        if (!type.IsAssignableTo(typeof(IDataGatewayService)))
        {
            throw new InvalidOperationException($"无效的数据网关服务'{type}'");
        }
        Services.TryAddEnumerable(ServiceDescriptor.Singleton(type));
        Services.TryAddEnumerable(ServiceDescriptor.Singleton(service => (IDataGatewayService)service.GetRequiredService(type)));
        return this;
    }

    public IDataGatewayApplicationBuilder UseService<T>() where T : IDataGatewayService
    {
        UseService(typeof(T));
        return this;
    }

    public DataGatewayApplicationBuilder(IServiceCollection services)
    {
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IDataGatewayApplication, DataGatewayApplication>());
        Services = services;
    }
}