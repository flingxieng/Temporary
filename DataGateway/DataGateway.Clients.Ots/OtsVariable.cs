using DataGateway.Abstractions;

namespace DataGateway.Clients.Ots;

public struct OtsVariable
{
    public VariableSetter Setter { get; set; }
    public OtsAddress Address { get; set; }
}