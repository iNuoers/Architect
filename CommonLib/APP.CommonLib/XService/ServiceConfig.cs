using System;
using System.Collections.Generic;

using APP.CommonLib.Config;

namespace APP.CommonLib.XService
{
    #region 调用接口的业务方法配置实体
    /// <summary>
    /// 调用接口的业务方法信息
    /// </summary>
    class MethodNode
    {
        /// <summary>
        /// 
        /// </summary>
        [Node]
        public string MethodName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Node]
        public string EntryPoint { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    class MethodConfig
    {
        /// <summary>
        /// 
        /// </summary>
        [Node("Methods/Method", NodeAttribute.NodeType.List)]
        public List<MethodNode> Method { get; set; }
    }
    #endregion

    #region 参数校验方法配置实体
    /// <summary>
    /// 参数校验方法配置信息
    /// </summary>
    class ValidationNode
    {
        /// <summary>
        /// 
        /// </summary>
        [Node]
        public string ValName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Node]
        public string Type { get; set; }
    }

    class ServiceValidation
    {
        [Node]
        public string ValName { get; set; }

        private string _param = "";
        [Node]
        public string Param
        {
            get { return _param; }
            set { _param = value; }
        }
    }
    #endregion

    #region 业务方法配置实体
    /// <summary>
    /// 业务方法配置信息
    /// </summary>
    class ServiceNode
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        [Node("methodName")]
        public string MethodName { get; set; }

        /// <summary>
        /// 服务类型
        /// </summary>
        [Node("type")]
        public string Type { get; set; }
    }

    class ServiceConfig
    {
        /// <summary>
        /// 
        /// </summary>
        [Node("services/service", NodeAttribute.NodeType.List)]
        public List<ServiceNode> Services { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Node("Validations/Validation", NodeAttribute.NodeType.List)]
        public List<ValidationNode> Validations { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Node("ReqStartList/ReqStart", NodeAttribute.NodeType.List)]
        public List<string> ReqStartList { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Node("ReqEndList/ReqEnd", NodeAttribute.NodeType.List)]
        public List<string> ReqEndList { get; set; }
    }
    #endregion
}
