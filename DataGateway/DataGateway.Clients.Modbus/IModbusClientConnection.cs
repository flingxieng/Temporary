using NModbus;

namespace DataGateway.Clients.Modbus;

public interface IModbusClientConnection : IDisposable
{
    bool Connected { get; }
    public IModbusMaster Client { get; }
    Task ConnectAsync();
}