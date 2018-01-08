using Newtonsoft.Json;
using System;

namespace APP.CommonLib.Utils
{
    /// <summary>
    /// Json格式化枚举
    /// </summary>
    public enum JsonFormatting
    {
        /// <summary>
        /// 无格式化
        /// </summary>
        None = 0,
        /// <summary>
        /// 格式化输出
        /// </summary>
        Indented = 1
    }

    /// <summary>
    /// Json帮助类
    /// </summary>
    public class JsonHelper
    {
        #region JsonSerializer
        /// <summary>
        /// 实体转换为JSON字符串
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="t">实体对象</param>
        /// <returns>Json字符串</returns>
        public static string JsonSerializer<T>(T t)
        {
            return JsonConvert.SerializeObject(t);
        }

        /// <summary>
        /// 实体转换为JSON字符串，指定是否格式化
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="t">实体对象</param>
        /// <param name="formatting">指定是否格式化</param>
        /// <returns>Json字符串</returns>
        public static string JsonSerializer<T>(T t, JsonFormatting formatting)
        {
            return JsonConvert.SerializeObject(t, (Formatting)formatting);
        }

        /// <summary>
        /// 实体转换为JSON字符串，指定是否格式化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="formatting"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static string JsonSerializer<T>(T t, JsonFormatting formatting, SerializerSettings settings)
        {
            return JsonConvert.SerializeObject(t, (Formatting)formatting, settings);
        }

        public static string JsonSerializer<T>(T t, JsonFormatting formatting, params Converter[] converters)
        {
            return JsonConvert.SerializeObject(t, (Formatting)formatting, converters);
        }

        public static string JsonSerializer<T>(T t, SerializerSettings settings)
        {
            return JsonConvert.SerializeObject(t, settings);
        }

        public static string JsonSerializer<T>(T t, params Converter[] converters)
        {
            return JsonConvert.SerializeObject(t, converters);
        }

        public static string JsonSerializer<T>(T t, Type type, SerializerSettings settings)
        {
            return JsonConvert.SerializeObject(t, type, settings);
        }

        public static string JsonSerializer<T>(T t, Type type, JsonFormatting formatting, SerializerSettings settings)
        {
            return JsonConvert.SerializeObject(t, type, (Formatting)formatting, settings);
        }
        #endregion

        #region JsonDeserialize

        /// <summary>
        /// json字符串转换为实体对象
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="json">json字符串</param>
        /// <returns>实体对象</returns>
        public static T JsonDeserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static T JsonDeserialize<T>(string json, SerializerSettings settings)
        {
            return JsonConvert.DeserializeObject<T>(json, settings);
        }

        public static T JsonDeserialize<T>(string json, params Converter[] converters)
        {
            return JsonConvert.DeserializeObject<T>(json, converters);
        }
        #endregion

    }

    public class SerializerSettings : JsonSerializerSettings
    {
    }

    public class Converter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
        }
    }
}
