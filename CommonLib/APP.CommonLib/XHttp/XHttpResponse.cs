using APP.CommonLib.Utils;
using Newtonsoft.Json;
using System;

namespace APP.CommonLib.XHttp
{
    /// <summary>
    /// http响应实体
    /// </summary>
    public class XHttpResponse
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public XHttpResponse()
        {
            V = 0;
            S = 0;
            D = "";
            Tt = 0;
            G = "";
            E = "";
            Ts = "";
            Es = "";
            Idv = "";
            Adv = "";
            Ie = false;
        }

        /// <summary>
        /// 数据
        /// Data
        /// </summary>
        [JsonProperty("d")]
        public string D { get; set; }

        /// <summary>
        /// 耗时（TimeTaken）
        /// </summary>
        [JsonProperty("tt")]
        public long Tt { get; set; }

        /// <summary>
        /// APP版本号（APPVersion）
        /// </summary>
        [JsonProperty("v")]
        public int V { get; set; }

        /// <summary>
        /// Guid
        /// </summary>
        [JsonProperty("g")]
        public string G { get; set; }

        /// <summary>
        /// 是否加密
        /// </summary>
        [JsonProperty("ie")]
        public bool Ie { get; set; }

        /// <summary>
        /// 加密版本号
        /// </summary>
        [JsonProperty("e")]
        public string E { get; set; }

        /// <summary>
        /// 状态
        /// Status
        /// </summary>
        [JsonProperty("s")]
        public int S { get; set; }

        /// <summary>
        /// 异常消息
        /// </summary>
        [JsonProperty("es")]
        public string Es { get; set; }

        /// <summary>
        /// 返回时间戳（TimeSpan）
        /// </summary>
        [JsonProperty("ts")]
        public string Ts { get; set; }

        /// <summary>
        /// 低频数据缓存版本号IOS用（IOSDataVersion）
        /// </summary>
        [JsonProperty("idv")]
        public string Idv { get; set; }

        /// <summary>
        /// 低频数据缓存版本号Android用（AndroidDataVersion）
        /// </summary>
        [JsonProperty("adv")]
        public string Adv { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timetaken"></param>
        /// <param name="guid"></param>
        /// <param name="status"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static XHttpResponse Exception(long timetaken, string guid = "", int status = 0, string exception = "")
        {
            return new XHttpResponse
            {
                G = guid,
                S = status,
                Tt = timetaken,
                Es = exception,
                Ts = DateTime.Now.ToString(TimeFormat.YMDHMS)
            };
        }

        /// <summary>
        /// 异常情况
        /// </summary>
        /// <param name="status">状态</param>
        /// <param name="exception">异常信息</param>
        /// <param name="data">数据</param>
        /// <returns>http响应实体</returns>
        public static XHttpResponse Exception(int status, string exception = "", string data = "")
        {
            return new XHttpResponse
            {
                D = data,
                S = status,
                Es = exception,
                Ts = DateTime.Now.ToString(TimeFormat.YMDHMS)
            };
        }

        /// <summary>
        /// 异常情况
        /// </summary>
        /// <param name="status">状态</param>
        /// <param name="exception">异常信息</param>
        /// <returns></returns>
        public static XHttpResponse Exception(int status, string exception = "")
        {
            return new XHttpResponse
            {
                S = status,
                Es = exception,
                Ts = DateTime.Now.ToString(TimeFormat.YMDHMS)
            };
        }
    }
}
