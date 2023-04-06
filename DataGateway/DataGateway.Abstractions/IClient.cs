namespace DataGateway.Abstractions;

public interface IClient : IDisposable
{
    string Name { get; }
    double Read(Variable variable);
    Task<double> ReadAsync(Variable variable, CancellationToken cancellationToken = default);
    void Write(Variable variable, double value);
    Task WriteAsync(Variable variable, double value, CancellationToken cancellationToken = default);
}