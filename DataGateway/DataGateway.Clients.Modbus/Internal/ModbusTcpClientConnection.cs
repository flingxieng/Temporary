using System.Net.Sockets;
using NModbus;

namespace DataGateway.Clients.Modbus.Internal;

internal class ModbusTcpClientConnection : IModbusClientConnection
{
    private readonly TcpClient _tcp = new();
    private readonly NetworkConnectionOptions _options;
	
    public bool Connected => _tcp.Connected;
    public IModbusMaster Client { get; }
	
    public ModbusTcpClientConnection(NetworkConnectionOptions options)
    {
        _options = options;
        Client = new ModbusFactory().CreateMaster(_tcp);
    }
	
    public async Task ConnectAsync()
    {
        await _tcp.ConnectAsync(_options.Ip, _options.Port);
        Client.Transport.ReadTimeout = _options.ReadTimeout;
        Client.Transport.WriteTimeout = _options.WriteTimeout;
    }
	
    public void Dispose()
    {
        Client.Dispose();
        _tcp.Dispose();
    }
}