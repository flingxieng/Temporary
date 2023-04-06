using DataGateway.Abstractions;
using DataGateway.Clients.Modbus.Internal;

namespace DataGateway.Clients.Modbus;

public readonly struct ModbusVariable
{
    public VariableSetter Setter { get; }
    public ModbusAddress Address { get; }
    
    public ModbusVariable(VariableSetter setter, ModbusAddress address)
    {
        Setter = setter;
        Address = address;
    }
    
    internal void SetValue(Span<byte> buffer)
    {
        var byteSize = Address.ByteSize;
        Span<byte> source = stackalloc byte[byteSize];
        switch (Address.ByteOrder)
        {
            case ModbusByteOrder.BigEndian:
            {
                buffer.CopyTo(source);
                break;
            }
            case ModbusByteOrder.LittleEndian:
            {
                for (var i = 0; i < source.Length; i++)
                {
                    source[i] = buffer[byteSize - 1 - i];
                }
                break;
            }
            case ModbusByteOrder.DoubleByteBigEndian:
            {
                if (byteSize > 2)
                {
                    for (var i = 0; i < source.Length; i += 2)
                    {
                        source[i] = buffer[byteSize - 2 - i];
                        source[i + 1] = buffer[byteSize - 1 - i];
                    }
                }
                else
                {
                    buffer.CopyTo(source);
                }
                break;
            }
            case ModbusByteOrder.DoubleByteLittleEndian:
            {
                if (byteSize > 2)
                {
                    for (var i = 0; i < source.Length; i += 2)
                    {
                        source[i] = buffer[i + 1];
                        source[i + 1] = buffer[i];
                    }
                }
                else
                {
                    buffer.CopyTo(source);
                }
                break;
            }
            default:
            {
                buffer.CopyTo(source);
                break;
            }
        }
        var value = Utilities.ReadValue(source, Address.ValueType, Address.BitIndex);
        Setter.SetValue(value);
    }
}