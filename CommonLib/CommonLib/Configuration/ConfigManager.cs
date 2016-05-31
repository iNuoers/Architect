using System;
using System.Xml;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CommonLib.Configuration
{
    /// <summary>
    /// Config管理器
    /// </summary>
    public class ConfigManager
    {
        /// <summary>
        /// 读取String类型的配置信息
        /// </summary>
        /// <param name="key">读取键</param>
        /// <param name="defaultValue">是否使用默认值</param>
        /// <returns></returns>
        public static string GetWebConfig(string key, string defaultValue = "")
        {
            var value = System.Configuration.ConfigurationManager.AppSettings[key];
            return string.IsNullOrEmpty(value) ? defaultValue : value;
        }

        /// <summary>
        /// 读取Int类型的配置信息
        /// </summary>
        /// <param name="key">读取键</param>
        /// <param name="defaultvalue">是否使用默认值</param>
        /// <returns></returns>
        public static int GetWebConfig(string key, int defaultvalue = 0)
        {
            var value = System.Configuration.ConfigurationManager.AppSettings[key];

            if (string.IsNullOrEmpty(value))
                return defaultvalue;

            int v;
            return int.TryParse(value, out v) ? v : defaultvalue;
        }

        /// <summary>
        /// 读取Long类型的配置信息
        /// </summary>
        /// <param name="key">读取键</param>
        /// <param name="defaultvalue">是否使用默认值</param>
        /// <returns></returns>
        public static long GetWebConfig(string key, long defaultvalue = 0)
        {
            string value = System.Configuration.ConfigurationManager.AppSettings[key];

            if (string.IsNullOrEmpty(value))
                return defaultvalue;

            long v;
            return long.TryParse(value, out v) ? v : defaultvalue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlFilename"></param>
        /// <returns></returns>
        public static T GetObjectConfig<T>(string xmlFilename) where T : class
        {
            var doc = new XmlDocument();
            doc.Load(xmlFilename);
            var element = doc.DocumentElement;
            if (element == null || !String.Equals(element.Name, "configuration"))
                throw new Exception("DOM element is null or is not a configuration element.");

            var xnode = (XmlNode) element;

            var t = typeof(T);
            var ot = (T)SetValue(t, xnode);

            return ot;
        }

        /// <summary>
        /// 设置Config节点值
        /// </summary>
        /// <param name="t"></param>
        /// <param name="xnode"></param>
        /// <returns></returns>
        private static object SetValue(Type t, XmlNode xnode)
        {
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

                default:
                    {
                        var ot = Activator.CreateInstance(t);
                        var pis = t.GetProperties();
                        foreach (var pi in pis)
                        {
                            var node = GetNodeAttr(pi);

                            if (node != null)
                            {
                                if (string.IsNullOrEmpty(node.NodeName))
                                    node.NodeName = pi.Name;
                                Type tt = pi.PropertyType;
                                if (xnode.Attributes != null && xnode.Attributes[node.NodeName] == null) continue;
                                if (xnode.Attributes != null)
                                {
                                    var o = xnode.Attributes[node.NodeName].Value;
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
                                    }
                                }
                                continue;
                            }

                            var nodelist = GetNodeListAttr(pi);
                            if (nodelist == null) continue;
                            {
                                var tt = pi.PropertyType;

                                var subTypes = tt.GetGenericArguments();
                                if (subTypes.Length != 1) continue;

                                var list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(subTypes[0]));
                                pi.SetValue(ot, list, null);

                                var nl = xnode.SelectNodes(nodelist.NodeName);
                                foreach (var ob in from XmlNode xn in nl select SetValue(subTypes[0], xn))
                                {
                                    list.Add(ob);
                                }
                            }
                        }
                        return ot;
                    }
            }
            return null;
        }

        /// <summary>
        /// 获取config节点属性值
        /// </summary>
        /// <param name="xnode"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private static string GetNodeAttrValue(XmlNode xnode, string name)
        {
            if (xnode.Attributes != null && xnode.Attributes[name] == null) return "";
            if (xnode.Attributes == null) return null;
            var o = xnode.Attributes[name].Value;
            return o;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pi"></param>
        /// <returns></returns>
        private static NodeListAttribute GetNodeListAttr(PropertyInfo pi)
        {
            var attributes = pi.GetCustomAttributes(typeof(NodeListAttribute), false);
            if (attributes.Length == 0)
                return null;

            return attributes[0] as NodeListAttribute;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pi"></param>
        /// <returns></returns>
        private static NodeAttribute GetNodeAttr(PropertyInfo pi)
        {
            var attributes = pi.GetCustomAttributes(typeof(NodeAttribute), false);
            if (attributes.Length == 0)
                return null;

            return attributes[0] as NodeAttribute;
        }

    }

    [AttributeUsage(AttributeTargets.Property)]
    public class NodeAttribute : Attribute
    {
        public NodeAttribute()
        {
        }

        public NodeAttribute(string nodename)
        {
            NodeName = nodename;
        }

        public string NodeName { get; set; }
    }


    [AttributeUsage(AttributeTargets.Property)]
    public class NodeListAttribute : Attribute
    {
        public NodeListAttribute(string nodename)
        {
            NodeName = nodename;
        }
        public string NodeName { get; set; }
    }
}