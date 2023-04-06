using System.Runtime.InteropServices;
using Ots.Client;

namespace DataGateway.Clients.Ots.Internal;

internal class OtsVariableGroup
{
    private readonly IReadOnlyList<OtsVariable> _variables;
    public OtsMessage Request { get; }

    public OtsVariableGroup(string key, IReadOnlyList<OtsVariable> variables)
    {
        Request = new OtsMessage(OtsEvents.ReadFlowChart, key);
        _variables = variables;
    }

    public void SetError(string error)
    {
        foreach (var variable in _variables)
        {
            variable.Setter.SetError(error);
        }
    }

    public void SetValue(byte[] buffer)
    {
        var values = MemoryMarshal.Cast<byte, double>(buffer);
        foreach (var variable in _variables)
        {
            var index = variable.Address.GroupIndex;
            variable.Setter.SetValue(values[index]);
        }
    }
}