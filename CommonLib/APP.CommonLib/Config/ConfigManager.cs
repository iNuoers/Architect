using System;
using System.IO;
using System.Xml;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

using APP.CommonLib.Log;

namespace APP.CommonLib.Config
{
    /// <summary>
    /// 配置文件帮助类
    /// </summary>
    public class ConfigManager
    {
        /// <summary>
        /// 获取web配置文件中指定Key的值
        /// </summary>
        /// <param name="key">配置key</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>配置值</returns>
        public static string GetWebConfig(string key, string defaultValue = "")
        {
            try
            {
                string value = System.Configuration.ConfigurationManager.AppSettings[key];
                if (!string.IsNullOrWhiteSpace(value))
                    return value;
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("获取web配置文件中指定Key的值错误，原因：{0}-{1}", ex.Message, ex.StackTrace));
            }
            return defaultValue;
        }

        /// <summary>
        /// 获取web配置文件中指定Key的值
        /// </summary>
        /// <param name="key">配置key</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>配置值</returns>
        public static int GetWebConfig(string key, int defaultValue = 0)
        {
            try
            {
                string value = System.Configuration.ConfigurationManager.AppSettings[key];
                if (string.IsNullOrEmpty(value))
                    return defaultValue;

                if (int.TryParse(value, out int v))
                    return v;
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("获取web配置文件中指定Key的值错误，原因：{0}-{1}", ex.Message, ex.StackTrace));
            }
            return defaultValue;
        }

        /// <summary>
        /// 获取web配置文件中指定Key的值
        /// </summary>
        /// <param name="key">配置key</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>配置值</returns>
        public static long GetWebConfig(string key, long defaultValue = 0)
        {
            try
            {
                string value = System.Configuration.ConfigurationManager.AppSettings[key];
                if (string.IsNullOrEmpty(value))
                    return defaultValue;

                if (long.TryParse(value, out long v))
                    return v;
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("获取web配置文件中指定Key的值错误，原因：{0}-{1}", ex.Message, ex.StackTrace));
            }
            return defaultValue;
        }

        /// <summary>
        /// 获取web配置文件中指定Key的值
        /// </summary>
        /// <param name="key">配置key</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>配置值</returns>
        public static bool GetWebConfig(string key, bool defaultValue = false)
        {
            try
            {
                string value = System.Configuration.ConfigurationManager.AppSettings[key];

                if (string.IsNullOrEmpty(value))
                    return defaultValue;

                if (Boolean.TryParse(value, out bool v))
                    return v;
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("获取web配置文件中指定Key的值错误，原因：{0}-{1}", ex.Message, ex.StackTrace));
            }
            return defaultValue;
        }

        /// <summary>
        /// 读取XML文件,将XML文件内容转换成指定类型的对象
        /// </summary>
        /// <typeparam name="T">配置类型</typeparam>
        /// <param name="xmlFilename">xml文件全名</param>
        /// <returns></returns>
        public static T GetObjectConfig<T>(string xmlFileName) where T : class
        {
            T ot = default(T);
            try
            {
                if (File.Exists(xmlFileName))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(xmlFileName);
                    XmlElement elememt = doc.DocumentElement;
                    if (elememt == null || !string.Equals(elememt.Name, "configuration"))
                        throw new Exception("DOM element is null or is not a configuration element.");

                    XmlNode node = elememt as XmlNode;

                    Type t = typeof(T);
                    ot = (T)SetValue(t, node);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("读取XML文件,将XML文件内容转换成指定类型的对象失败，原因：{0} {1}", ex.Message, ex.StackTrace));
            }
            return ot;
        }

        /// <summary>
        /// 内部方法，获取xml节点的属性值
        /// </summary>
        /// <param name="xnode">xml节点</param>
        /// <param name="name">属性名称</param>
        /// <returns>属性值</returns>
        private static string GetNodeAttrValue(XmlNode xnode, string name)
        {
            if (xnode == null || xnode.Attributes[name] == null) return "";
            string o = xnode.Attributes[name].Value;
            return o;
        }

        /// <summary>
        /// 内部方法，获取xml节点的属性
        /// </summary>
        /// <param name="pi">类的属性</param>
        /// <returns>xml节点属性</returns>
        private static NodeAttribute GetNodeAttr(PropertyInfo pi)
        {
            object[] attributes = pi.GetCustomAttributes(typeof(NodeAttribute), false);
            if (attributes == null || attributes.Length == 0)
                return null;

            return attributes[0] as NodeAttribute;
        }

        /// <summary>
        /// 内部方法，将XML内容映射成类属性值
        /// </summary>
        /// <param name="t">类型</param>
        /// <param name="xnode">xml节点</param>
        /// <returns>object</returns>
        private static object SetValue(Type t, XmlNode xnode)
        {
            if (xnode == null)
                return null;
            switch (t.ToString())
            {
                case "System.String":
                    return GetNodeAttrValue(xnode, "value");

                case "System.Int16":
                    if (!string.IsNullOrEmpty(GetNodeAttrValue(xnode, "value")))
                        return Int16.Parse(GetNodeAttrValue(xnode, "value"));
                    break;

                case "System.Int32":
                    if (!string.IsNullOrEmpty(GetNodeAttrValue(xnode, "value")))
                        return Int32.Parse(GetNodeAttrValue(xnode, "value"));
                    break;

                case "System.Int64":
                    if (!string.IsNullOrEmpty(GetNodeAttrValue(xnode, "value")))
                        return Int64.Parse(GetNodeAttrValue(xnode, "value"));
                    break;

                case "System.Boolean":
                    if (!string.IsNullOrEmpty(GetNodeAttrValue(xnode, "value")))
                        return Boolean.Parse(GetNodeAttrValue(xnode, "value"));
                    break;

                case "System.DateTime":
                    if (!string.IsNullOrEmpty(GetNodeAttrValue(xnode, "value")))
                        return DateTime.Parse(GetNodeAttrValue(xnode, "value"));
                    break;

                default:
                    {
                        object ot = Activator.CreateInstance(t);
                        PropertyInfo[] pis = t.GetProperties();
                        foreach (PropertyInfo pi in pis)
                        {
                            NodeAttribute node = GetNodeAttr(pi);

                            if (node == null) continue;
                            switch (node.Type)
                            {
                                case NodeAttribute.NodeType.Simple:
                                    {
                                        if (string.IsNullOrEmpty(node.NodeName))
                                            node.NodeName = pi.Name;
                                        Type tt = pi.PropertyType;
                                        if (xnode.Attributes[node.NodeName] == null) continue;
                                        string o = xnode.Attributes[node.NodeName].Value;
                                        if (string.IsNullOrEmpty(o))
                                            continue;
                                        switch (tt.ToString())
                                        {
                                            case "System.String":
                                                pi.SetValue(ot, o, null);
                                                break;

                                            case "System.Int16":
                                                pi.SetValue(ot, Int16.Parse(o), null);
                                                break;

                                            case "System.Int32":
                                                pi.SetValue(ot, Int32.Parse(o), null);
                                                break;

                                            case "System.Int64":
                                                pi.SetValue(ot, Int64.Parse(o), null);
                                                break;

                                            case "System.Boolean":
                                                pi.SetValue(ot, Boolean.Parse(o), null);
                                                break;

                                            case "System.DateTime":
                                                pi.SetValue(ot, DateTime.Parse(o), null);
                                                break;
                                        }
                                        continue;
                                    }
                                case NodeAttribute.NodeType.List:
                                    {
                                        Type tt = pi.PropertyType;

                                        Type[] subTypes = tt.GetGenericArguments();
                                        if (subTypes.Length != 1) continue;

                                        IList list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(subTypes[0]));
                                        pi.SetValue(ot, list, null);

                                        XmlNodeList nl = xnode.SelectNodes(node.NodeName);
                                        foreach (XmlNode xn in nl)
                                        {
                                            object ob = SetValue(subTypes[0], xn);
                                            list.Add(ob);
                                        }
                                    }
                                    break;
                                case NodeAttribute.NodeType.Class:
                                    {
                                        Type tt = pi.PropertyType;
                                        XmlNodeList nl = xnode.SelectNodes(node.NodeName);
                                        if (nl == null)
                                            continue;
                                        object obj = SetValue(tt, nl[0]);
                                        pi.SetValue(ot, obj, null);
                                    }
                                    break;
                            }
                        }
                        return ot;
                    }
            }
            return null;
        }
    }

    /// <summary>
    /// 自定义Node特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class NodeAttribute : Attribute
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public NodeAttribute()
        {
            Type = NodeType.Simple;
        }

        /// <summary>
        /// 构造函数重载
        /// </summary>
        /// <param name="nodename">xml节点名</param>
        /// <param name="type">节点类型</param>
        public NodeAttribute(string nodename, NodeType type = NodeType.Simple)
        {
            NodeName = nodename;
            Type = type;
        }

        /// <summary>
        /// 节点名
        /// </summary>
        public string NodeName { get; set; }

        /// <summary>
        /// 节点类型枚举
        /// </summary>
        public enum NodeType
        {
            Simple,
            Class,
            List,
        }

        /// <summary>
        /// 节点类型
        /// </summary>
        public NodeType Type { get; set; }
    }
}