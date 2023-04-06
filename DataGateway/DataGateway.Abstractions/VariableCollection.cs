using System.Collections;
using System.Collections.Immutable;

namespace DataGateway.Abstractions;

public class VariableCollection : IReadOnlyCollection<Variable>
{
    public static VariableCollection Empty { get; } = new(Array.Empty<Variable>());
    
    private readonly ImmutableDictionary<NodeId,Variable> _variables;
    
    public int Count => _variables.Count;
    
    public VariableCollection(IEnumerable<Variable> variables)
    {
        var values = variables.Select(variable => new KeyValuePair<NodeId, Variable>(variable.NodeId, variable));
        _variables = ImmutableDictionary.CreateRange(values);
    }

    public Variable? GetVariable(NodeId nodeId)
    {
        _variables.TryGetValue(nodeId, out var variable);
        return variable;
    }
    
    public Variable GetRequiredVariable(NodeId nodeId)
    {
        if (!_variables.TryGetValue(nodeId, out var variable))
        {
            throw new InvalidOperationException($"找不到扫节点id为 '{nodeId}' 的变量");
        }
        return variable;
    }
    
    public IEnumerator<Variable> GetEnumerator()
    {
        return _variables.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _variables.Values.GetEnumerator();
    }
}