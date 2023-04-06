using Ots.Client;

namespace DataGateway.Clients.Ots.Internal;

internal class DataCollector : IDisposable
{
    private readonly IOtsClient _client;
    private readonly IReadOnlyList<OtsVariableGroup> _groups;
    private readonly int _updateRate;
    private readonly CancellationTokenSource _cts;
    private readonly Task _executeTask;

    public DataCollector(IOtsClient client, IReadOnlyList<OtsVariableGroup> groups, int updateRate)
    {
        _client = client;
        _groups = groups;
        _updateRate = updateRate;
        _cts = new CancellationTokenSource();
        var token = _cts.Token;
        _executeTask = Task.Run(() => LoopCollectAsync(token));
    }

    private async Task LoopCollectAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            foreach (var group in _groups)
            {
                var response = await _client.RequestAsync(group.Request, cancellationToken);
                group.SetValue(response.Data);
            }
            await Task.Delay(_updateRate, cancellationToken);
        }
    }
    
    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();
    }
}