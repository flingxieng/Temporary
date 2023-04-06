using DataGateway.Abstractions;

namespace DataGateway.Application;

public interface IVariableProvider
{
    Task<VariableCollection> ProvideAsync(CancellationToken cancellation = default);
}