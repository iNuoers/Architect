using System;
using System.IO;
using System.Web;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using APP.CommonLib.IO;
using APP.CommonLib.Log;
using APP.CommonLib.XHttp;
using APP.CommonLib.Config;
using APP.CommonLib.Extension;

namespace APP.CommonLib.XService
{
    public class ServiceEngine
    {
        const string methodCfgFile = "method.config";
        const string serviceCfgFile = "service.config";
        /// <summary>
        /// 业务方法字典
        /// </summary>
        private static readonly Dictionary<string, ServiceInfo> Dic = new Dictionary<string, ServiceInfo>();

        /// <summary>
        /// 单例
        /// </summary>
        private static ServiceEngine _instance;

        #region 装载服务组件
        /// <summary>
        /// 加载项目目录中的服务组件列表
        /// </summary>
        /// <returns></returns>
        private static List<Assembly> GetAssemblies()
        {
            // 项目根目录
            var mapPath = PathHelper.MapPath;
            var path = PathHelper.isWeb ? mapPath + "/bin/" : mapPath;
            var filePaths = Directory.GetFiles(path, "app.*")
                .Where(s => s.EndsWith(".dll") || s.EndsWith(".exe"))
                .ToList();
            var list = new List<Assembly>();
            filePaths.ForEach(_ => { list.Add(Assembly.LoadFrom(_)); });
            return list;
        }

        /// <summary>
        /// 初始化服务组件
        /// </summary>
        /// <param name="assemblies">组件集合</param>
        private static void Initialization(List<Assembly> assemblies)
        {
            if (_instance != null)
                return;
            _instance = new ServiceEngine();
            if (assemblies == null)
                return;

            var serviceCfg = GetServiceCfg();
            ServiceHandler(serviceCfg);

            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes().Where(IsServicesType))
                {

                }
            }
        }

        /// <summary>
        /// 加载业务方法配置项
        /// </summary>
        /// <returns></returns>
        private static ServiceConfig GetServiceCfg()
        {
            return ConfigManager.GetObjectConfig<ServiceConfig>(PathHelper.MergePathName(PathHelper.GetConfigPath(), serviceCfgFile));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceConfig"></param>
        private static void ServiceHandler(ServiceConfig serviceConfig)
        {
            if (serviceConfig == null)
                return;

            foreach (var node in serviceConfig.Services)
            {
                if (Dic.ContainsKey(node.MethodName))
                    continue;

                var type = Type.GetType(node.Type);
                if (type == null || !IsServicesType(type))
                    continue;

                var handler = Activator.CreateInstance(type) as IServiceHandler;
                var info = new ServiceInfo
                {
                    node = node,
                    handler = handler
                };
                Logger.Info("Dic Add:" + node.MethodName + " by config");
                Dic.Add(node.MethodName, info);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool IsServicesType(Type type)
        {
            return type.IsClass && !type.IsAbstract && type.GetInterfaces().Contains(typeof(IServiceHandler));
        }

        /// <summary>
        /// 加载服务配置项
        /// </summary>
        /// <param name="serviceConfig"></param>
        private static void LoadServiceHandler(ServiceConfig serviceConfig)
        {
            if (serviceConfig == null)
                return;

            foreach (var node in serviceConfig.Services)
            {
                if (Dic.ContainsKey(node.MethodName))
                    continue;

                var type = Type.GetType(node.Type);
                if (type == null || !IsServicesType(type))
                    continue;

                var handler = Activator.CreateInstance(type) as IServiceHandler;
                var info = new ServiceInfo
                {
                    node = node,
                    handler = handler
                };
                Logger.Info("Dic Add:" + node.MethodName + " by config");
                Dic.Add(node.MethodName, info);
            }
        }

        /// <summary>
        /// 加载方法配置
        /// </summary>
        private static void LoadMethodHandlerg(ServiceConfig serviceConfig)
        {

        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        private ServiceEngine() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        static ServiceEngine()
        {
            try
            {
                var assemblies = GetAssemblies();
                Initialization(assemblies);
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("初始化服务组件异常，{0} -- {1}", ex.Message, ex.StackTrace));
            }
        }


        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        internal static XHttpResponse Handler(XHttpRequest request, HttpContext context)
        {
            try
            {
                var methodInfo = Dic.FirstOrDefault(t => t.Key == request.M).Value;
                if (methodInfo == null)
                {
                    Logger.Error("方法 ：" + request.M + " 未配置");
                    return XHttpResponse.Exception((int)ServiceResultStatus.Error, "服务方法 ：" + request.M + "未配置");
                }

                // 参数校验
                string errTips;

                string afterData = "";

                Logger.Info("requestData:【" + request.ToJSON() + "】");

                var req = new ServiceRequest
                {
                    data = afterData,
                    deviceid = request.DID
                };

                Logger.Info("rData:【" + req.ToJSON() + "】");
                var d = methodInfo.handler.Invoke(req);
                if (d.Status == ServiceResultStatus.Ok)
                {
                    string data;

                }
                return XHttpResponse.Exception((int)d.Status, d.ExceptionMessage, d.Content);
            }
            catch (Exception ex)
            {
                Logger.Error($"call method {request.M} failed.", ex);
                return XHttpResponse.Exception((int)ServiceResultStatus.Error, ex.Message);
            }
        }

    }
}