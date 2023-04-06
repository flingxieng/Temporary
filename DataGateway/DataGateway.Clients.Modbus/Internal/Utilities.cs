using System.Buffers.Binary;

namespace DataGateway.Clients.Modbus.Internal;

internal static class Utilities
{
    public static double ReadValue(ReadOnlySpan<byte> source, ValueType valueType, byte? bitIndex)
    {
        switch (valueType)
        {
            case ValueType.Bool:
            {
                if (!bitIndex.HasValue) return source[0] == 0 ? 0 : 1;
                var value = BinaryPrimitives.ReadInt16BigEndian(source);
                return (double)GetBitValue(value, bitIndex.Value);
            }
            case ValueType.Short:
            {
                return BinaryPrimitives.ReadInt16BigEndian(source);
            }
            case ValueType.UShort:
                return BinaryPrimitives.ReadUInt16BigEndian(source);
            case ValueType.Int:
                return BinaryPrimitives.ReadInt32BigEndian(source);
            case ValueType.UInt:
                return BinaryPrimitives.ReadUInt32BigEndian(source);
            case ValueType.Float:
                return BinaryPrimitives.ReadSingleBigEndian(source);
            case ValueType.Long:
                return BinaryPrimitives.ReadInt64BigEndian(source);
            case ValueType.ULong:
                return BinaryPrimitives.ReadUInt64BigEndian(source);
            case ValueType.Double:
                return BinaryPrimitives.ReadDoubleBigEndian(source);
            default:
                throw new ArgumentOutOfRangeException(nameof(valueType), valueType, null);
        }
    }
    
    private static decimal GetBitValue(short value, byte bitIndex)
    {
        return (value >> bitIndex) | 1;
    }
    
    public static ushort GetMaxAddressLength(ModbusRegister register)
    {
        return register switch
        {
            ModbusRegister.Coils or ModbusRegister.Inputs => 2000,
            ModbusRegister.HoldingRegisters or ModbusRegister.InputRegisters => 120,
            _ => throw new ArgumentOutOfRangeException(nameof(register), register, null)
        };
    }
}