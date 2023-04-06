namespace Ots.Client;

public static class OtsEvents
{
    /// <summary>
    /// 心跳
    /// </summary>
    public const short Heartbeat = 1;
    public const short ReadValues = 101;
    public const short Write = 102;
    public const short WriteAndSave = 103;
    public const short WriteDebug = 104;
    public const short CancelDebug = 105;
    public const short AlarmData = 201;
    public const short ReadSerialMonitor = 301;
    public const short ReadNetMonitor = 302;
    public const short ReadPropChildValues = 401;
    public const short ReadPropChildStrings = 402;
    public const short ReadClassChildValues = 403;
    public const short ReadFlowChart = 501;
    public const short ReadDevice = 502;

    /// <summary>
    /// 读取需要保存的数据
    /// </summary>
    public const short ReadSaveData = 601;

    /// <summary>
    /// 刷新控制系统配置文件
    /// </summary>
    public const short RefreshSaveData = 602;


    /// <summary>
    /// 注册属性
    /// </summary>
    public const short Register = 701;

    /// <summary>
    /// 取消注册属性
    /// </summary>
    public const short Unregister = 702;

    /// <summary>
    /// 注册属性模板
    /// </summary>
    public const short RegisterTemp = 703;

    /// <summary>
    /// 取消注册属性模板
    /// </summary>
    public const short UnregisterTemp = 704;

    /// <summary>
    /// 读取已经注册的属性 key
    /// </summary>
    public const short ReadRegistered = 721;

    /// <summary>
    /// 读取已经注册的属性模板 key
    /// </summary>
    public const short ReadRegisteredTemp = 722;

    /// <summary>
    /// 读取已经注册的属性值
    /// </summary>
    public const short ReadRegisterData = 751;

    /// <summary>
    /// 读取已经注册的属性模板值
    /// </summary>
    public const short ReadRegisterTempData = 752;

    /// <summary>
    /// 验证控制系统
    /// </summary>
    public const short Verify = 1001;
}