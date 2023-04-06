namespace DataGateway.Abstractions;

public sealed class Variable
{
    private int _decimals;
    internal double _value;
    public NodeId NodeId { get; }
    public string Name { get; }
    public double Value { get => GetValue();  internal set => _value = value; }
    public bool IsReadOnly { get; }
    public long Timestamp { get; internal set; }
    public VariableMetadataCollection Metadata { get; }
    public int Decimals { get => _decimals; set => _decimals = value is >= 0 and <= 15 ? value : 0; }
    public double? Max { get; set; }
    public double? Min { get; set; }
    public double? Mult { get; set; }
    public double? Incr { get; set; }
    public VariableStatus Status { get; internal set; }
    public string? Message { get; internal set; }
    public IClient? Client { get; private set; }

    public event VariableChangedDelegate? Changed;
    public event VariableValueChangedDelegate? ValueChanged;
    public event VariableStatusChangedDelegate? StatusChanged;
    
    public Variable(NodeId nodeId, string name, double value, bool isReadOnly, VariableMetadataCollection metadata)
    {
        Name = name;
        Value = value;
        IsReadOnly = isReadOnly;
        Metadata = metadata;
        NodeId = nodeId;
    }
    
    public VariableSetter Bind(IClient client)
    {
        Client = client;
        return new VariableSetter(this);
    }

    internal void OnChanged()
    {
        Changed?.Invoke(this);
    }
    
    internal void OnValueChanged(double oldValue, double newValue)
    {
        ValueChanged?.Invoke(this, new VariableValueChangedArgs(oldValue, newValue));
    }
    
    internal void OnStatusChanged(VariableStatus oldStatus, VariableStatus newStatus)
    {
        StatusChanged?.Invoke(this, new VariableStatusChangedArgs(oldStatus, newStatus));
    }
    
    private double GetValue()
    {
        var value = _value;
        if (Mult.HasValue)
        {
            value *= Mult.Value;
        }
        if (Incr.HasValue)
        {
            value += Incr.Value;
        }
        if (value > Max)
        {
            value = Max.Value;
        }
        else if (value < Min)
        {
            value = Min.Value;
        }
        return Math.Round(value, _decimals);
    }
}

public delegate void VariableChangedDelegate(Variable variable);
public delegate void VariableStatusChangedDelegate(Variable variable, VariableStatusChangedArgs args);
public delegate void VariableValueChangedDelegate(Variable variable, VariableValueChangedArgs args);
public readonly record struct VariableStatusChangedArgs(VariableStatus OldStatus, VariableStatus NewStatus);
public readonly record struct VariableValueChangedArgs(double OldValue, double NewValue);