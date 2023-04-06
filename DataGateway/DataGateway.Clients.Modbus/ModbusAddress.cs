namespace DataGateway.Clients.Modbus;

public class ModbusAddress
{
    public byte SlaveAddress { get; set; }
    public ModbusRegister Register { get; set; }
    public ushort Address { get; set; }
    public ValueType ValueType { get; set; }
    public byte? BitIndex { get; set; }
    public ModbusByteOrder ByteOrder { get; set; }
    
    public ushort AddressLength => GetAddressLength();
    public int ByteSize => GetByteSize();
    
    private ushort GetAddressLength()
    {
        if (Register is ModbusRegister.Coils or ModbusRegister.Inputs)
            return 1;
        var length = GetByteSize() / 2;
        return (ushort) (length == 0 ? 1 : length);
    }

    private int GetByteSize()
    {
        if (Register is ModbusRegister.Coils or ModbusRegister.Inputs)
            return 1;
        return ValueType switch
        {
            ValueType.Bool => 2,
            ValueType.Short => 2,
            ValueType.UShort => 2,
            ValueType.Int => 4,
            ValueType.UInt => 4,
            ValueType.Float => 4,
            ValueType.Long => 8,
            ValueType.ULong => 8,
            ValueType.Double => 8,
            _ => 2
        };
    }
}

public enum ModbusRegister
{
    Coils,
    Inputs,
    HoldingRegisters,
    InputRegisters
}

/// <summary>
/// Modbus 字节顺序
/// </summary>
public enum ModbusByteOrder : byte
{
    /// <summary>
    /// 大端
    /// </summary>
    BigEndian,

    /// <summary>
    /// 小端
    /// </summary>
    LittleEndian,

    /// <summary>
    /// 双字节大端
    /// </summary>
    DoubleByteBigEndian,

    /// <summary>
    /// 双字节小端
    /// </summary>
    DoubleByteLittleEndian,
}

public enum ValueType
{
    Bool,
    Short,
    UShort,
    Int,
    UInt,
    Float,
    Long,
    ULong,
    Double
}