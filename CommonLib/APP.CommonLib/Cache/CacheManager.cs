using System;
using System.Web;
using System.Linq;
using Newtonsoft.Json;
using ServiceStack.Redis;
using System.Collections.Generic;

using APP.CommonLib.Redis;
using APP.CommonLib.Log;

namespace APP.CommonLib.Cache
{
    /// <summary>
    /// 缓存相关操作
    /// </summary>
    public class CacheManager
    {
        /// <summary>
        /// Formatting
        /// </summary>
        const string formatting = "[{0}]:[{1}]";
        /// <summary>
        /// FormattingV
        /// </summary>
        const string formattingV = "[{0}]:[{1}]V";
        /// <summary>
        /// FormattingPage
        /// </summary>
        const string FormattingPage = "[{0}]:[{1}]:[{2}]";

        /// <summary>
        /// 插入web环境缓存
        /// </summary>
        /// <param name="namespaces">命名空间</param>
        /// <param name="key">缓存key</param>
        /// <param name="value">要插入的数据</param>
        /// <param name="ttl">过期时间</param>
        public static void PutWebCache(string namespaces, string key, string value, int ttl = 10)
        {
            try
            {
                if (string.IsNullOrEmpty(namespaces)) throw new ArgumentNullException();
                if (string.IsNullOrEmpty(key)) throw new ArgumentNullException();
                if (string.IsNullOrEmpty(value)) throw new ArgumentNullException();
                key = string.Format(formatting, namespaces, key);
                HttpRuntime.Cache.Insert(key, value, null, System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(0, 0, ttl));
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("web环境获取缓存失败，原因：{0} {1}", ex.Message, ex.StackTrace));
            }
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="namespaces">命名空间</param>
        /// <param name="key">key</param>
        /// <returns>缓存数据</returns>
        public static string GetWebCache(string namespaces, string key)
        {
            try
            {
                if (string.IsNullOrEmpty(namespaces)) throw new ArgumentNullException();
                if (string.IsNullOrEmpty(key)) throw new ArgumentNullException();
                key = string.Format(formatting, namespaces, key);
                return (string)HttpRuntime.Cache.Get(key);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("web环境获取缓存失败，原因：{0} {1}", ex.Message, ex.StackTrace));
            }
        }

        /// <summary>
        /// 插入缓存到Redis服务器
        /// </summary>
        /// <param name="namespaces">命名空间</param>
        /// <param name="key">缓存Key</param>
        /// <param name="value">要插入的数据</param>
        /// <param name="ttl">过期时间</param>
        public static void PutRedisCache(string namespaces, string key, string value, int ttl = 10)
        {
            try
            {
                if (string.IsNullOrEmpty(namespaces)) throw new ArgumentNullException();
                if (string.IsNullOrEmpty(key)) throw new ArgumentNullException();
                if (string.IsNullOrEmpty(value)) throw new ArgumentNullException();
                key = string.Format(formatting, namespaces, key);
                if (ttl > 0)
                    RedisHelper.SetEntry(key, value, new TimeSpan(0, 0, ttl));
                else
                    RedisHelper.SetEntry(key, value);
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("插入缓存到Redis服务器失败，原因：{0} {1}", ex.Message, ex.StackTrace));
            }
        }

        /// <summary>
        /// 获取Redis中的缓存数据
        /// </summary>
        /// <param name="namespaces">命名空间</param>
        /// <param name="key">缓存Key</param>
        /// <returns>缓存数据</returns>
        public static string GetRedisCache(string namespaces, string key)
        {
            try
            {
                if (string.IsNullOrEmpty(namespaces)) throw new ArgumentNullException();
                if (string.IsNullOrEmpty(key)) throw new ArgumentNullException();
                key = string.Format(formatting, namespaces, key);
                return RedisHelper.GetValue(key);
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("获取Redis中的缓存数据失败，原因：{0} {1}", ex.Message, ex.StackTrace));
                return "";
            }
        }

        /// <summary>
        /// 插入缓存流到Redis中
        /// </summary>
        /// <param name="namespaces">命名空间</param>
        /// <param name="key">缓存Key</param>
        /// <param name="stream">要插入的数据流</param>
        /// <param name="ttl">过期时间</param>
        public static void PutRedisStreamCache(string namespaces, string key, byte[] stream, int ttl = 10)
        {
            try
            {
                if (string.IsNullOrEmpty(namespaces)) throw new ArgumentNullException();
                if (string.IsNullOrEmpty(key)) throw new ArgumentNullException();
                if (stream.Length <= 0) throw new ArgumentNullException();
                key = string.Format(formattingV, namespaces, key);
                using (RedisClient client = RedisHelper.GetClient())
                {
                    client.Set(key, stream, new TimeSpan(0, 0, ttl));
                }
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("插入缓存流到Redis服务器失败，原因：{0} {1}", ex.Message, ex.StackTrace));
            }
        }

        /// <summary>
        /// 获取Redis中的缓存流
        /// </summary>
        /// <param name="namespaces">命名空间</param>
        /// <param name="key">缓存Key</param>
        /// <returns>缓存流</returns>
        public static byte[] GetRedisStreamCache(string namespaces, string key)
        {
            try
            {
                if (string.IsNullOrEmpty(namespaces)) throw new ArgumentNullException();
                if (string.IsNullOrEmpty(key)) throw new ArgumentNullException();
                key = string.Format(formattingV, namespaces, key);
                using (RedisClient client = RedisHelper.GetClient())
                {
                    return client.Get(key);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("获取Redis中的缓存流数据失败，原因：{0} {1}", ex.Message, ex.StackTrace));
                return null;
            }
        }

        /// <summary>
        /// 删除服务器缓存
        /// </summary>
        /// <param name="namespaces"></param>
        /// <param name="key"></param>
        public static void DelWebCache(string namespaces, string key)
        {
            try
            {
                if (string.IsNullOrEmpty(namespaces)) throw new ArgumentNullException();
                if (string.IsNullOrEmpty(key)) throw new ArgumentNullException();
                key = string.Format(formatting, namespaces, key);
                HttpRuntime.Cache.Remove(key);
            }
            catch (Exception ex)
            {
                throw new Exception("web环境删除缓存失败，原因：" + ex.Message);
            }
        }

        /// <summary>
        /// 删除Redis缓存
        /// </summary>
        /// <param name="namespaces"></param>
        /// <param name="key"></param>
        public static void DelRedisCache(string namespaces, string key)
        {
            try
            {
                if (string.IsNullOrEmpty(namespaces)) throw new ArgumentNullException();
                if (string.IsNullOrEmpty(key)) throw new ArgumentNullException();
                key = string.Format(formatting, namespaces, key);
                using (RedisClient client = RedisHelper.GetClient())
                {
                    var f = client.Remove(key);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("删除Redis中的缓存数据失败，原因：" + ex.Message);
            }
        }

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="namespaces"></param>
        /// <param name="key"></param>
        public static void DelRedisStreamCache(string namespaces, string key)
        {
            try
            {
                if (string.IsNullOrEmpty(namespaces)) throw new ArgumentNullException();
                if (string.IsNullOrEmpty(key)) throw new ArgumentNullException();
                key = string.Format(formattingV, namespaces, key);
                RedisHelper.Remove(key);
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("删除Redis中的缓存流失败，原因：{0} {1}", ex.Message, ex.StackTrace));
            }
        }
    }
}