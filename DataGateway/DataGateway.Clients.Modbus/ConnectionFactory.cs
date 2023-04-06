using DataGateway.Clients.Modbus.Internal;

namespace DataGateway.Clients.Modbus;

public static class ConnectionFactory
{
    public static IModbusClientConnection CreateTcp(NetworkConnectionOptions options)
    {
        return new ModbusTcpClientConnection(options);
    }
    public static IModbusClientConnection CreateUdp(NetworkConnectionOptions options)
    {
        return new ModbusUdpClientConnection(options);
    }
    public static IModbusClientConnection CreateRtu(SerialPortConnectionOptions options)
    {
        return new ModbusSerialPortConnection(options);
    }
    public static IModbusClientConnection CreateAscii(SerialPortConnectionOptions options)
    {
        return new ModbusSerialPortConnection(options, true);
    }
}