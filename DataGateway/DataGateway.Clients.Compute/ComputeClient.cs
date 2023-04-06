using DataGateway.Abstractions;
using DataGateway.Clients.Compute.Internal;

namespace DataGateway.Clients.Compute;

public sealed class ComputeClient : IClient
{
    private ComputeEntry[] _entries = Array.Empty<ComputeEntry>();
    public string Name { get; }

    public ComputeClient(string name)
    {
        Name = name;
    }

    public void ProvideValues(string expression, IEnumerable<ComputeVariable> variables)
    {
        var calculator = ExpressionFactory.Compile(expression);
        if (calculator == null)
        {
            return;
        }
        _entries = variables.Select(x => new ComputeEntry(calculator, x)).ToArray();
    }

    public double Read(Variable variable)
    {
        return variable.Value;
    }
    
    public Task<double> ReadAsync(Variable variable, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(variable.Value);
    }
    
    public void Write(Variable variable, double value)
    {
        throw new NotImplementedException();
    }
    
    public Task WriteAsync(Variable variable, double value, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
    
    public void Dispose()
    {
        foreach (var entry in _entries)
        {
            entry.Dispose();
        }
    }
}

public class ComputeVariable
{
    public VariableSetter Setter { get; }
    public Variable[] Params { get; }

    public ComputeVariable(VariableSetter setter, Variable[]? @params = default)
    {
        Setter = setter;
        Params = @params ?? Array.Empty<Variable>();
    }
}

internal sealed class ComputeEntry : IDisposable
{
    private readonly Func<double, double[]?, double> _calculator;
    private readonly ComputeVariable _variable;
    private readonly double[] _params;

    public ComputeEntry(Func<double, double[]?, double> calculator, ComputeVariable variable)
    {
        _calculator = calculator;
        _variable = variable;
        _params = new double[variable.Params.Length];
        foreach (var item in variable.Params)
        {
            item.ValueChanged += VariableOnValueChanged;
        }
        Calculate();
    }
    
    public void Dispose()
    {
        foreach (var item in _variable.Params)
        {
            item.ValueChanged -= VariableOnValueChanged;
        }
    }
    
    private void VariableOnValueChanged(Variable variable, VariableValueChangedArgs args)
    {
        Calculate();
    }

    private void Calculate()
    {
        var self = _variable.Setter.Variable.Value;
        for (var i = 0; i < _variable.Params.Length; i++)
        {
            var variable = _variable.Params[i];
            _params[i] = variable.Value;
        }
        try
        {
            var value = _calculator.Invoke(self, _params);
            _variable.Setter.SetValue(value);
        }
        catch
        {
            _variable.Setter.SetError("计算错误");
        }
    }
}