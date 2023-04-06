using System.IO.Ports;

namespace DataGateway.Clients.Modbus;

public class ConnectionOptions
{
    /// <summary>
    /// 读取超时时间
    /// </summary>
    public int ReadTimeout { get; set; }
    
    /// <summary>
    /// 写入超时时间
    /// </summary>
    public int WriteTimeout { get; set; }
}

public class SerialPortConnectionOptions : ConnectionOptions
{
    /// <summary>
    /// 端口名称
    /// </summary>
    public string PortName { get; init; } = string.Empty;

    /// <summary>
    /// 波特率
    /// </summary>
    public int BaudRate { get; init; }

    /// <summary>
    /// 校验位
    /// </summary>
    public Parity Parity { get; init; }

    /// <summary>
    /// 数据位
    /// </summary>
    public int DataBits { get; init; }

    /// <summary>
    /// 停止位
    /// </summary>
    public StopBits StopBits { get; init; }
}

/// <summary>
/// modbus 网络客户端连接配置
/// </summary>
public class NetworkConnectionOptions : ConnectionOptions
{
    public string Ip { get; set; } = string.Empty;
    public int Port { get; set; }
}