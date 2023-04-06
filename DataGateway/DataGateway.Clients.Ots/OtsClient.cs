using DataGateway.Abstractions;
using DataGateway.Clients.Ots.Internal;
using Ots.Client;

namespace DataGateway.Clients.Ots;

public sealed class OtsClient : IClient
{
    public string Name { get; }
    private readonly IOtsClient _client;
    private DataCollector? _collector;

    public OtsClient(string name, IOtsClient client)
    {
        _client = client;
        Name = name;
    }

    public void ProvideValues(int updateRate, IEnumerable<OtsVariable> variables)
    {
        var groups = variables
            .GroupBy(variable => variable.Address.GroupKey)
            .Select(x => new OtsVariableGroup(x.Key, x.OrderBy(x => x.Address.GroupIndex).ToArray()))
            .ToList();
        _collector = new DataCollector(_client, groups, updateRate);
    }
    
    public double Read(Variable variable)
    {
        var address = variable.Metadata.GetRequiredMetadata<OtsAddress>();
        return _client.ReadPropValue(address.LongName).GetAwaiter().GetResult();
    }
    
    public async Task<double> ReadAsync(Variable variable, CancellationToken cancellationToken = default)
    {
        var address = variable.Metadata.GetRequiredMetadata<OtsAddress>();
        return await _client.ReadPropValue(address.LongName, cancellationToken);
    }
    
    public void Write(Variable variable, double value)
    {
        var address = variable.Metadata.GetRequiredMetadata<OtsAddress>();
        _client.WritePropValue(address.LongName, value).GetAwaiter().GetResult();
    }
    
    public async Task WriteAsync(Variable variable, double value, CancellationToken cancellationToken = default)
    {
        var address = variable.Metadata.GetRequiredMetadata<OtsAddress>();
        await _client.WritePropValue(address.LongName, value, cancellationToken);
    }
    
    public void Dispose()
    {
        _collector?.Dispose();
        _client.Dispose();
    }
}