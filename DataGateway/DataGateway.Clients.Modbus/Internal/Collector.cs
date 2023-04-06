namespace DataGateway.Clients.Modbus.Internal;

internal class Collector : IDisposable
{
    private readonly IModbusClientConnection _connection;
    private readonly int _updateRate;
    private readonly IReadOnlyList<ModbusVariableGroup> _groups;
    private readonly CancellationTokenSource _cts;
    private readonly Task _executeTask;

    public Collector(IModbusClientConnection connection, int updateRate, IReadOnlyList<ModbusVariableGroup> groups)
    {
        _connection = connection;
        _updateRate = updateRate;
        _groups = groups;
        _cts = new CancellationTokenSource();
        _executeTask = Task.Run(() => RunAsync(_cts.Token));
    }

    private async Task RunAsync(CancellationToken cancellationToken)
    {
        while (cancellationToken.IsCancellationRequested)
        {
            foreach (var group in _groups)
            {
                using var owner = await _connection.ReadAsync(group.SlaveAddress, group.Register, group.StartAddress, group.AddressLength);
                await Task.Delay(30, cancellationToken);
                foreach (var variable in group.Variables)
                {
                    var index = variable.Address.Address - group.StartAddress;
                    var byteSize = variable.Address.ByteSize;
                    var source = owner.Memory.Slice(index, byteSize);
                    variable.SetValue(source.Span);
                }
            }
            await Task.Delay(_updateRate, cancellationToken);
        }
    }

    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();
    }
}