using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ots.Client;

public static class OtsClientExtensions
{
    /// <summary>
    /// 读取属性值
    /// </summary>
    /// <param name="client">ots 客户端</param>
    /// <param name="longName">属性长名称</param>
    /// <param name="cancellationToken">任务取消令牌</param>
    /// <returns>属性值</returns>
    public static async Task<double> ReadPropValue(this IOtsClient client, string longName, CancellationToken cancellationToken = default)
    {
        var message = new OtsMessage(OtsEvents.ReadValues, longName);
        var response = await client.RequestAsync(message, cancellationToken);
        return MemoryMarshal.Cast<byte, double>(response.Data)[0];
    }

    /// <summary>
    /// 读取多个属性值
    /// </summary>
    /// <param name="client">ots 客户端</param>
    /// <param name="longNames">属性长名称</param>
    /// <param name="cancellationToken">任务取消令牌</param>
    /// <returns>属性值数组</returns>
    public static async Task<ByteBox<double>> ReadPropValues(this IOtsClient client, IEnumerable<string> longNames, CancellationToken cancellationToken = default)
    {
        var body = new StringBuilder().AppendJoin(',', longNames).ToString();
        var message = new OtsMessage(OtsEvents.ReadValues, body);
        var response = await client.RequestAsync(message, cancellationToken);
        return new ByteBox<double>(response.Data);
    }

    /// <summary>
    /// 写入属性值
    /// </summary>
    /// <param name="client">ots 客户端<</param>
    /// <param name="longName">属性长名称</param>
    /// <param name="value">写入值</param>
    /// <param name="cancellationToken">任务取消令牌</param>
    /// <returns>写入任务</returns>
    public static async Task WritePropValue(this IOtsClient client, string longName, double value, CancellationToken cancellationToken = default)
    {
        var message = new OtsMessage(OtsEvents.Write, $"{longName}:{value}");
        _ = await client.RequestAsync(message, cancellationToken);
    }
}

public readonly struct ByteBox<T> where T : struct
{
    private readonly byte[] _buffer;
    public ByteBox(byte[] buffer)
    {
        _buffer = buffer;
    }
    public Span<T> Values()
    {
        return MemoryMarshal.Cast<byte, T>(_buffer).ToArray();
    }
}