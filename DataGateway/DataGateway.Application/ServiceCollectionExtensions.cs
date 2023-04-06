using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DataGateway.Application;

public static class ServiceCollectionExtensions
{
    public static DataGatewayApplicationBuilder
        AddDataGatewayApplication<T>(this IServiceCollection services) where T : IVariableProvider
    {
        services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(IVariableProvider), typeof(T)));
        return new DataGatewayApplicationBuilder(services);
    }
    
    public static DataGatewayApplicationBuilder
        AddDataGatewayApplication(this IServiceCollection services, IVariableProvider provider)
    {
        services.TryAddEnumerable(ServiceDescriptor.Singleton(provider));
        return new DataGatewayApplicationBuilder(services);
    }
}