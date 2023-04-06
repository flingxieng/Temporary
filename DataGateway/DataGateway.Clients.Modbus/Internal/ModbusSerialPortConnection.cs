using System.IO.Ports;
using NModbus;
using NModbus.Serial;

namespace DataGateway.Clients.Modbus.Internal;

public sealed class ModbusSerialPortConnection : IModbusClientConnection
{
    private readonly SerialPort _serialPort = new();
    private readonly SerialPortConnectionOptions _options;
    public bool Connected => _serialPort.IsOpen;
    public IModbusMaster Client { get; }
    
    public ModbusSerialPortConnection(SerialPortConnectionOptions options, bool isAscii = false)
    {
        _options = options;
        var factory = new ModbusFactory();
        Client = isAscii ? factory.CreateAsciiMaster(_serialPort) : factory.CreateRtuMaster(_serialPort);
    }
    
    public Task ConnectAsync()
    {
        _serialPort.Open();
        Client.Transport.ReadTimeout = _options.ReadTimeout;
        Client.Transport.WriteTimeout = _options.WriteTimeout;
        _serialPort.PortName = _options.PortName;
        _serialPort.BaudRate = _options.BaudRate;
        _serialPort.Parity = _options.Parity;
        _serialPort.DataBits = _options.DataBits;
        _serialPort.StopBits = _options.StopBits;
        return Task.CompletedTask;
    }
    
    public void Dispose()
    {
        Client.Dispose();
        _serialPort.Dispose();
    }
}