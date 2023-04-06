namespace DataGateway.Abstractions;

public readonly struct NodeId : IEquatable<NodeId>
{
    public const char Separator = '.';
    private readonly string _nodeId;
    public NodeId(string nodeId)
    {
        ArgumentException.ThrowIfNullOrEmpty(nodeId);
        _nodeId = nodeId;
    }
    public NodeId Child(string name)
    {
        return new NodeId($"{_nodeId}{Separator}{name}");
    }
    public NodeId Parent()
    {
        if (string.IsNullOrEmpty(_nodeId) || _nodeId.IndexOf(Separator) < 0)
        {
            return new NodeId(string.Empty);
        }
        var index = _nodeId.LastIndexOf(Separator);
        return new NodeId(_nodeId[..(index + 1)]);
    }
    public override string ToString()
    {
        return _nodeId;
    }
    public override bool Equals(object? obj)
    {
        return obj is NodeId other && Equals(other);
    }
    public override int GetHashCode()
    {
        return _nodeId.GetHashCode();
    }
    public bool Equals(NodeId other)
    {
        return _nodeId == other._nodeId;
    }
    public static bool operator ==(NodeId left, NodeId right)
    {
        return left.Equals(right);
    }
    public static bool operator !=(NodeId left, NodeId right)
    {
        return !(left == right);
    }
}