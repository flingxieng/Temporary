namespace DataGateway.Abstractions;

/// <summary>
/// 变量状态
/// </summary>
public enum VariableStatus
{
    /// <summary>
    /// 未绑定
    /// </summary>
    Unbound,
    
    /// <summary>
    /// 未确定
    /// </summary>
    Unknown,
    
    /// <summary>
    /// 正常
    /// </summary>
    Normal,
    
    /// <summary>
    /// 警告
    /// </summary>
    Warning,
    
    /// <summary>
    /// 错误
    /// </summary>
    Error
}