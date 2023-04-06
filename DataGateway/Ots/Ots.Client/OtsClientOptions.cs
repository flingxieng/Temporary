namespace Ots.Client
{
    /// <summary>
    /// ots client 配置
    /// </summary>
    public class OtsClientOptions
    {
        private const int DefaultTimeout = 5000;
        private int _timeout = DefaultTimeout;

        /// <summary>
        /// 本地 ip 地址
        /// </summary>
        public string LocalIp { get; set; } = "127.0.0.1";

        /// <summary>
        /// 本地端口号
        /// </summary>
        public int LocalPort { get; set; } = 9700;

        /// <summary>
        /// Ots ip 地址
        /// </summary>
        public string RemoteIp { get; set; } = "127.0.0.1";

        /// <summary>
        /// Ots 端口号
        /// </summary>
        public int RemotePort { get; set; } = 11111;

        /// <summary>
        /// 超时时间(毫秒)
        /// </summary>
        public int Timeout
        {
            get => _timeout;
            set
            {
                if (value > 0)
                {
                    _timeout = value;
                }
            }
        }
    }
}
