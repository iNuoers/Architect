using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using APP.CommonLib.Utils;

namespace APP.CommonLib.Extension
{
    /// <summary>
    /// Object的扩展方法
    /// </summary>
    public static class ObjectExtension
    {
        #region TOJSON
        /// <summary>
        /// 扩展方法：将对象序列化为JSON
        /// </summary>
        /// <param name="pCaller">调用者</param>
        /// <returns>对象序列化为JSON的字符串</returns>
        public static string ToJSON(this object pCaller)
        {
            if (pCaller == null) { return "null"; }

            if (pCaller.GetType().Name == "DataTable" || pCaller.GetType().Name == "DataSet")
            {
                //DataTable序列化
                return JsonHelper.JsonSerializer(pCaller, JsonFormatting.Indented);
            }
            else
            {
                //其它直接序列化
                return JsonHelper.JsonSerializer(pCaller);
            }
        }

        #endregion

        #region TOINT
        /// <summary>
        /// int 类型转换的方法
        /// </summary>
        /// <param name="id">string 值</param>
        /// <returns>返回转换后的值</returns>
        public static int ToInt(this object obj, int DefaultValue = 0)
        {
            if (obj.GetType().IsEnum) //增加枚举判断
                return (int)obj;

            if (obj != null && obj != DBNull.Value)
                int.TryParse(obj.ToString(), out DefaultValue);
            return DefaultValue;
        }
        #endregion

        #region TODATETIME
        /// <summary>
        /// datetime 类型转换的方法
        /// </summary>
        /// <param name="id">string 值</param>
        /// <returns>返回转换后的值</returns>
        public static DateTime ToDateTime(this object time)
        {
            DateTime result;
            if (time != null && DateTime.TryParse(time.ToString(), out result))
            {
                return result;
            }
            return DateTime.MinValue;
        }
        #endregion

        #region TOdecimal
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="DefaultValue"></param>
        /// <returns></returns>
        public static decimal ToDecimal(this object obj, decimal DefaultValue = 0m)
        {
            if (obj != null && obj != DBNull.Value)
                decimal.TryParse(obj.ToString(), out DefaultValue);

            return DefaultValue;
        }
        #endregion

        #region ToBytes

        /// <summary>
        /// 获取可序列化对象的二进制数据
        /// </summary>
        /// <param name="pValue">可序列化对象</param>
        /// <returns></returns>
        public static byte[] GetBytes(this object pValue)
        {
            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, pValue);
                return ms.ToArray();
            }
        }
        #endregion

        #region 判断GUID是否为空GUID
        /// <summary>
        /// 判断是否为Guid.Empty
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static bool IsEmptyGuid(this Guid guid)
        {
            return guid == Guid.Empty;
        }
        #endregion
    }
}
