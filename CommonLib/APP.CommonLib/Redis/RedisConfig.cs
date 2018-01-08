using System.Collections.Generic;
using APP.CommonLib.Config;

namespace APP.CommonLib.Redis
{
    /// <summary>
    /// Redis配置
    /// </summary>
    public class RedisConfig
    {
        public RedisConfig()
        {
            DefaultDB = 0;
            AutoStart = true;
            MaxReadPoolSize = 100;
            MaxWritePoolSize = 100;
            PoolTimeout = 60000;
            ConnectTimeout = 60000;
            SocketReceiveTimeout = 60000;
            SocketSendTimeout = 60000;
        }

        /// <summary>
        /// 默认数据库
        /// </summary>
        [Node("DefaultDB", NodeAttribute.NodeType.Class)]
        public int DefaultDB { get; set; }
        /// <summary>
        /// 是否自启动
        /// </summary>
        public bool AutoStart { get; set; }
        /// <summary>
        /// 最大读连接池连接数
        /// </summary>
        public int MaxReadPoolSize { get; set; }
        /// <summary>
        /// 最大写连接池连接数
        /// </summary>
        public int MaxWritePoolSize { get; set; }
        /// <summary>
        /// 从连接池中获取连接的超时时间
        /// </summary>
        public int PoolTimeout { get; set; }
        /// <summary>
        /// 连接Redis的超时时间
        /// </summary>
        public int ConnectTimeout { get; set; }
        /// <summary>
        /// Scoket方式连接redis,接收的超时时间
        /// </summary>
        public int SocketReceiveTimeout { get; set; }
        /// <summary>
        /// Scoket方式连接redis,发送的超时时间
        /// </summary>
        public int SocketSendTimeout { get; set; }
        /// <summary>
        /// 主连接池Host列表
        /// </summary>
        [Node("MainRedisServers/MainRedisServer", NodeAttribute.NodeType.List)]
        public List<string> MainRedisServer { get; set; }
        /// <summary>
        /// 备用连接池Host列表
        /// </summary>
        [Node("BackupRedisServers/BackupRedisServer", NodeAttribute.NodeType.List)]
        public List<string> BackupRedisServer { get; set; }
    }
}