using System.Net.Sockets;
using NModbus;

namespace DataGateway.Clients.Modbus.Internal;

internal class ModbusUdpClientConnection : IModbusClientConnection
{
    private readonly UdpClient _udp = new();
    private readonly NetworkConnectionOptions _options;
	
    public bool Connected { get; private set; }
	
    public IModbusMaster Client { get; }
	
    public ModbusUdpClientConnection(NetworkConnectionOptions options)
    {
        _options = options;
        Client = new ModbusFactory().CreateMaster(_udp);
    }
	
    public Task ConnectAsync()
    {
        _udp.Connect(_options.Ip, _options.Port);
        Connected = true;
        Client.Transport.ReadTimeout = _options.ReadTimeout;
        Client.Transport.WriteTimeout = _options.WriteTimeout;
        return Task.CompletedTask;
    }
	
    public void Dispose()
    {
        Client.Dispose();
        _udp.Dispose();
    }
}