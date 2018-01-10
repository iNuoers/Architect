using APP.CommonLib.Utils;
using APP.CommonLib.XHttp;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace APP.CommonLib.XService
{
    #region 业务DLL接口
    /// <summary>
    /// 业务DLL接口
    /// </summary>
    public interface IServiceHandler
    {
        ServiceResult Invoke(ServiceRequest req);
    }
    #endregion

    #region 参数校验DLL接口
    /// <summary>
    /// 参数校验DLL接口
    /// </summary>
    public interface IValidationHandler
    {
        int Invoke(XHttpRequest req, string param, out string errMsg);
    }
    #endregion

    #region 基础服务接口
    /// <summary>
    /// 参数校验DLL接口
    /// </summary>
    public interface IBaseService
    {
        void Invoke();
    }
    #endregion

    #region 请求开始/结束要执行的操作接口
    /// <summary>
    /// 请求开始要执行的操作接口
    /// </summary>
    public interface IOnRequestStart
    {
        void Invoke(XHttpRequest req, HttpContext context);
    }

    /// <summary>
    /// 请求结束要执行的操作接口
    /// </summary>
    public interface IOnRequestEnd
    {
        void Invoke(XHttpRequest req, ref XHttpResponse response);
    }
    #endregion

    #region 业务实体
    /// <summary>
    /// 业务方法信息及方法handler
    /// </summary>
    class ServiceInfo
    {
        /// <summary>
        /// 业务方法配置信息
        /// </summary>
        public ServiceNode node { get; set; }

        /// <summary>
        /// 业务方法handler
        /// </summary>
        public IServiceHandler handler { get; set; }
    }

    /// <summary>
    /// 业务请求实体
    /// </summary>
    public class ServiceRequest
    {
        /// <summary>
        /// 业务数据
        /// </summary>
        public string data { get; set; }
        /// <summary>
        /// 设备ID
        /// </summary>
        public string deviceid { get; set; }
        /// <summary>
        /// Token对应的memberID
        /// </summary>
        public long mid { get; set; }
        /// <summary>
        /// 请求随机数
        /// </summary>
        public string guid { get; set; }
        /// <summary>
        /// 客户端ip
        /// </summary>
        public string remoteip { get; set; }
        /// <summary>
        /// 平台标识(IOS或ANDROID或API或SERVICE)
        /// </summary>
        public int platform { get; set; }
        /// <summary>
        /// 客户端版本号
        /// </summary>
        public int version { get; set; }
        /// <summary>
        /// 备用
        /// </summary>
        public string r { get; set; }
    }

    /// <summary>
    /// 业务返回实体
    /// </summary>
    public class ServiceResult
    {
        /// <summary>
        /// 状态
        /// </summary>
        [JsonProperty("status")]
        public ServiceResultStatus Status { get; set; }

        /// <summary>
        /// 业务数据
        /// </summary>
        [JsonProperty("content")]
        public string Content { get; set; }

        /// <summary>
        /// 异常信息
        /// </summary>
        [JsonProperty("exceptionmessage")]
        public string ExceptionMessage { get; set; }

        /// <summary>
        /// 获取业务数据实体
        /// </summary>
        /// <typeparam name="T">业务实体类型</typeparam>
        /// <returns>业务实体对象</returns>
        public T GetContentObj<T>() { return JsonHelper.JsonDeserialize<T>(Content); }

        /// <summary>
        /// 成功
        /// </summary>
        /// <param name="data">业务数据</param>
        /// <returns>业务返回实体</returns>
        public static ServiceResult Success(string data = "")
        {
            return new ServiceResult { Status = ServiceResultStatus.Ok, Content = data };
        }

        /// <summary>
        /// 成功
        /// </summary>
        /// <param name="data">业务数据</param>
        /// <returns>业务返回实体</returns>
        public static ServiceResult Success<T>(T data)
        {
            return new ServiceResult { Status = ServiceResultStatus.Ok, Content = JsonHelper.JsonSerializer(data) };
        }

        /// <summary>
        /// 异常情况
        /// </summary>
        /// <param name="status">状态码</param>
        /// <param name="exceptionStr">异常信息</param>
        /// <returns></returns>
        public static ServiceResult Exception(ServiceResultStatus status, string exceptionStr)
        {
            return new ServiceResult { Status = status, ExceptionMessage = exceptionStr };
        }

        /// <summary>
        /// 异常情况，且返回相关的业务数据
        /// </summary>
        /// <param name="exceptionStr"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static ServiceResult Exception(string exceptionStr, object data)
        {
            return new ServiceResult { Status = ServiceResultStatus.InvalidLogic, ExceptionMessage = exceptionStr, Content = JsonHelper.JsonSerializer(data) };
        }
    }

    public enum ServiceResultStatus
    {
        /// <summary>
        /// 成功，正常
        /// </summary>
        Ok = 0,

        /// <summary>
        /// 提示
        /// </summary>
        Tip = 1,

        /// <summary>
        /// 程序异常
        /// </summary>
        Error = 2,

        /// <summary>
        /// 版本有更新
        /// </summary>
        VersionUpdate = 3,

        /// <summary>
        /// 无效的参数
        /// </summary>
        InvalidParameter = 4,

        /// <summary>
        /// 错误操作，且返回业务有关的数据
        /// </summary>
        InvalidLogic = 5,

        /// <summary>
        /// 无效的Token，请重新登录
        /// </summary>
        InvalidToken = 101,

    }

    #endregion
}
