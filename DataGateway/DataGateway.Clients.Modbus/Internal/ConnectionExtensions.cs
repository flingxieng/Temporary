using System.Runtime.InteropServices;

namespace DataGateway.Clients.Modbus.Internal;

internal static class ConnectionExtensions
{
    public static async Task<ByteMemoryOwner> ReadAsync(
        this IModbusClientConnection connection,
        byte slaveAddress, 
        ModbusRegister register,
        ushort startAddress, 
        ushort addressLength)
    {
        if (!connection.Connected)
        {
            await connection.ConnectAsync();
        }
        var client = connection.Client;
        switch (register)
        {
            case ModbusRegister.Coils:
            {
                var values = await client.ReadCoilsAsync(slaveAddress, startAddress, addressLength);
                return Convert(values);
            }
            case ModbusRegister.Inputs:
            {
                var values = await client.ReadInputsAsync(slaveAddress, startAddress, addressLength);
                return Convert(values);
            }
            case ModbusRegister.HoldingRegisters:
            {
                var values = await client.ReadHoldingRegistersAsync(slaveAddress, startAddress, addressLength);
                return Convert(values);
            }
            case ModbusRegister.InputRegisters:
            {
                var values = await client.ReadHoldingRegistersAsync(slaveAddress, startAddress, addressLength);
                return Convert(values);
            }
            default:
            {
                throw new ArgumentOutOfRangeException(nameof(register), register, null);
            }
        }
    }
    
    private static ByteMemoryOwner Convert(IReadOnlyList<bool> values)
    {
        var owner = ByteMemoryOwner.Rent(values.Count);
        for (var i = 0; i < values.Count; i++)
        {
            var value = values[i];
            owner.Memory.Span[i] = value ? (byte)1 : (byte)0;
        }
        return owner;
    }
    
    private static ByteMemoryOwner Convert(ushort[] values)
    {
        var owner = ByteMemoryOwner.Rent(values.Length * 2);
        var source = MemoryMarshal.Cast<ushort, byte>(values);
        if (!BitConverter.IsLittleEndian)
        {
            source.CopyTo(owner.Memory.Span);
        }
        else
        {
            for (var i = 0; i < source.Length; i += 2)
            {
                owner.Memory.Span[i] = source[i + 1];
                owner.Memory.Span[i + 1] = source[i];
            }
        }
        return owner;
    }
}