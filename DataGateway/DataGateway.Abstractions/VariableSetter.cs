namespace DataGateway.Abstractions;

public readonly struct VariableSetter
{
    public Variable Variable { get; }
    
    internal VariableSetter(Variable variable)
    {
        Variable = variable;
    }
    
    public void SetValue(double value)
    {
        Variable.Value = value;
        Variable.Status = VariableStatus.Normal;
    }
    
    public void SetWarning(string message)
    {
        Variable.Message = message;
        Variable.Status = VariableStatus.Warning;
    }
    
    public void SetError(string message)
    {
        Variable.Message = message;
        Variable.Status = VariableStatus.Error;
    }
}