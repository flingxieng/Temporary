namespace DataGateway.Clients.Modbus.Internal;

internal class ModbusVariableGroup
{
    public byte SlaveAddress { get; }
    public ModbusRegister Register { get; }
    public ushort StartAddress { get; }
    public ushort AddressLength { get; }
    public IReadOnlyList<ModbusVariable> Variables { get; }
    
    private ModbusVariableGroup(byte slaveAddress, ModbusRegister register, 
        ushort startAddress, ushort addressLength, IReadOnlyList<ModbusVariable> variables)
    {
        SlaveAddress = slaveAddress;
        Register = register;
        StartAddress = startAddress;
        AddressLength = addressLength;
        Variables = variables;
    }
    
    public static IReadOnlyList<ModbusVariableGroup> GenerateGroups(IEnumerable<ModbusVariable> variables)
    {
        var groups = new List<ModbusVariableGroup>();
        foreach (var slaveIdGroups in variables.GroupBy(variable => variable.Address.SlaveAddress))
        {
            var slaveAddress = slaveIdGroups.Key;
            foreach (var registerGroups in slaveIdGroups.GroupBy(variable => variable.Address.Register))
            {
                var register = registerGroups.Key;
                var orderedVariables = registerGroups.OrderBy(variable => variable.Address.Address);
                var max = Utilities.GetMaxAddressLength(register);
                var items = GenerateGroups(slaveAddress, register, max, orderedVariables);
                groups.AddRange(items);
            }
        }
        return groups;
    }
    private static IEnumerable<ModbusVariableGroup> GenerateGroups(byte slaveId, ModbusRegister register, ushort maxAddress,
        IEnumerable<ModbusVariable> variables)
    {
        var groups = new List<ModbusVariableGroup>();
        ushort startAddress = 0;
        ushort addressLength = 0;
        ModbusAddress? prevAddress = null;
        var segments = new List<ModbusVariable>();
        foreach (var variable in variables)
        {
            var address = variable.Address;
            if (prevAddress == null)
            {
                startAddress = address.Address;
                Handle(variable);
            }
            else if (prevAddress.Address + prevAddress.AddressLength == address.Address)
            {
                Handle(variable);
            }
            else if (prevAddress.Address == address.Address)
            {
                segments.Add(variable);
            }
            else
            {
                var group = new ModbusVariableGroup(slaveId, register, startAddress, addressLength, segments);
                groups.Add(group);
                startAddress = variable.Address.Address;
                addressLength = variable.Address.AddressLength;
                segments = new List<ModbusVariable> {variable};
            }
            prevAddress = address;
        }
        if (segments.Count > 0)
        {
            var group = new ModbusVariableGroup(slaveId, register, startAddress, addressLength, segments);
            groups.Add(group);
        }

        return groups;

        void Handle(ModbusVariable variable)
        {
            var length = addressLength + variable.Address.AddressLength;
            if (length > maxAddress)
            {
                var group = new ModbusVariableGroup(slaveId, register, startAddress, addressLength, segments);
                groups.Add(group);
                startAddress = variable.Address.Address;
                addressLength = variable.Address.AddressLength;
                segments = new List<ModbusVariable>();
            }
            else
            {
                addressLength += variable.Address.AddressLength;
                segments.Add(variable);
            }
        }
    }
}