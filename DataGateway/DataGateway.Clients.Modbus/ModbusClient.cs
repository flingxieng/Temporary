using DataGateway.Abstractions;
using DataGateway.Clients.Modbus.Internal;

namespace DataGateway.Clients.Modbus;

public sealed class ModbusClient : IClient
{
    private readonly IModbusClientConnection _connection;
    private Collector? _collector;
    public string Name { get; }
    
    public ModbusClient(string name, IModbusClientConnection connection)
    {
        _connection = connection;
        Name = name;
    }

    public void ProvideValues(int updateRate, IEnumerable<ModbusVariable> variables)
    {
        var groups = ModbusVariableGroup.GenerateGroups(variables);
        _collector =new Collector(_connection, updateRate, groups);
    }
    
    public double Read(Variable variable)
    {
        return ReadAsync(variable).GetAwaiter().GetResult();
    }

    public async Task<double> ReadAsync(Variable variable, CancellationToken cancellationToken = default)
    {
        var address = variable.Metadata.GetMetadata<ModbusAddress>();
        if (address == null)
        {
            return default;
        }
        var owner = await _connection.ReadAsync(address.SlaveAddress, address.Register, address.Address, address.AddressLength);
        return Utilities.ReadValue(owner.Memory.Span, address.ValueType, address.BitIndex);
    }

    public void Write(Variable variable, double value)
    {
        throw new NotImplementedException();
    }

    public Task WriteAsync(Variable variable, double value, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        _collector?.Dispose();
        _connection.Dispose();
    }
}