using System;
using System.Threading.Tasks;
using System.Threading;

namespace Ots.Client;

/// <summary>
/// ots 客户端
/// </summary>
public interface IOtsClient : IDisposable
{
    /// <summary>
    /// 请求 OTS 数据
    /// </summary>
    /// <param name="request">请求消息</param>
    /// <param name="cancellationToken">请求取消令牌</param>
    /// <returns>OTS 响应信息</returns>
    Task<OtsMessage> RequestAsync(OtsMessage request, CancellationToken cancellationToken = default);
}