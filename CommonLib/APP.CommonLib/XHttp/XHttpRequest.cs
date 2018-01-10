using System;

namespace APP.CommonLib.XHttp
{
    /// <summary>
    /// 平台标识枚举
    /// </summary>
    public enum RequestPlatForm
    {
        /// <summary>
        /// 默认
        /// </summary>
        Default = 0,

        /// <summary>
        /// 安卓
        /// </summary>
        Android = 1,

        /// <summary>
        /// 苹果
        /// </summary>
        IOS = 2,

        /// <summary>
        /// 微信公众号
        /// </summary>
        WeChat = 3,

        /// <summary>
        /// PC端
        /// </summary>
        PC = 4,

        /// <summary>
        /// 微信小程序
        /// </summary>
        WeApp = 5
    }

    /// <summary>
    /// http请求实体
    /// </summary>
    public class XHttpRequest
    {
        /// <summary>
        /// 无参构造
        /// </summary>
        public XHttpRequest()
        {
            V = 0;
            D = "";
            M = "";
            E = "";
            T = "";
            MID = 0;
            IDV = "";
            ADV = "";
            DID = "";
            IE = false;
            TS = DateTime.UtcNow.Ticks;
            G = Guid.NewGuid().ToString();
            P = (int)RequestPlatForm.Default;
        }

        /// <summary>
        /// 业务数据Json字符串（Data）
        /// </summary>
        public string D { get; set; }

        /// <summary>
        /// 低频数据缓存版本号IOS用（IOSDataVersion）
        /// </summary>
        public string IDV { get; set; }

        /// <summary>
        /// 低频数据缓存版本号Android用（AndroidDataVersion）
        /// </summary>
        public string ADV { get; set; }

        /// <summary>
        /// 业务方法名（Method）
        /// </summary>
        public string M { get; set; }

        /// <summary>
        /// APP版本号（APPVersion）
        /// </summary>
        public int V { get; set; }

        /// <summary>
        /// 设备ID（DeviceID）
        /// </summary>
        public string DID { get; set; }

        /// <summary>
        /// 请求随机数（GUID）
        /// </summary>
        public string G { get; set; }

        /// <summary>
        /// 是否加密（IsEncryption）
        /// </summary>
        public bool IE { get; set; }

        /// <summary>
        /// 加密算法版本（EncryptionVersion）
        /// </summary>
        public string E { get; set; }

        /// <summary>
        /// Token
        /// </summary>
        public string T { get; set; }

        /// <summary>
        /// 请求时间戳（TimeSpan）
        /// </summary>
        public long TS { get; set; }

        /// <summary>
        /// 平台标识(PlatForm,1:Android;2:IOS)
        /// </summary>
        public int P { get; set; }

        /// <summary>
        /// 服务间调用用MemberID
        /// </summary>
        public int MID { get; set; }
    }
}
