using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Ots.Client;

public sealed class OtsClient : IOtsClient
{
    private Task? _receiveTask;
    private UdpClient? _client;
    private CancellationTokenSource? _cts;
    private bool _disposed;

    private readonly object _lock = new();
    private readonly OtsClientOptions _options;
    private readonly ConcurrentDictionary<long, TaskCompletionSource<OtsMessage>> _requestTasks = new();

    public OtsClient(OtsClientOptions options)
    {
        _options = options;
    }

    public async Task<OtsMessage> RequestAsync(OtsMessage request, CancellationToken cancellationToken = default)
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(GetType().Name);
        }
        var client = _client ?? CreateClient();
        using var buffer = request.GetBuffer();
        var task = CreateRequestTask(buffer.Id, cancellationToken);
        await client.SendAsync(buffer.Memory, _options.RemoteIp, _options.RemotePort, cancellationToken);
        return await task;
    }

    private async Task ReceiveAsync(UdpClient client, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var result = await client.ReceiveAsync(cancellationToken);
                var message = OtsMessage.Parse(result.Buffer);
                if (_requestTasks.TryRemove(message.Id, out var tcs))
                    tcs.TrySetResult(message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    private Task<OtsMessage> CreateRequestTask(long id, CancellationToken cancellationToken)
    {
        var tcs = new TaskCompletionSource<OtsMessage>();
        if (!_requestTasks.TryAdd(id, tcs))
        {
            throw new InvalidOperationException($"OTS请求Id重复");
        }

        Task.Delay(_options.Timeout, cancellationToken).GetAwaiter().OnCompleted(() =>
        {
            if (_requestTasks.TryRemove(id, out var tcs))
            {
                tcs.SetException(new TimeoutException("OTS请求超时"));
            }
        });

        return tcs.Task;
    }

    private UdpClient CreateClient()
    {
        lock (_lock)
        {
            if (_client != null)
                return _client;
            var localEndPoint = new IPEndPoint(IPAddress.Parse(_options.LocalIp), _options.LocalPort);
            _client = new UdpClient(localEndPoint);
            _cts = new CancellationTokenSource();
            _receiveTask = Task.Run(() => ReceiveAsync(_client, _cts.Token));
            return _client;
        }
    }

    public void Dispose()
    {
        _cts?.Cancel();
        _client?.Dispose();
        _cts = null;
        _receiveTask = null;
        _client = null;
        _disposed = true;
    }
}
