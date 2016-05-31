using System;
using System.Web;
using System.Collections.Generic;

using ServiceStack.Redis;
using CommonLib.Configuration;

namespace CommonLib.Cache
{
    /// <summary>
    /// 缓存管理器
    /// </summary>
    public class CacheManager
    {
        private const string formatting = "[{0}]:[{1}]";

        private const string formattingV = "[{0}]:[{1}]V";

        static Dictionary<string, System.Web.Caching.Cache> list = new Dictionary<string, System.Web.Caching.Cache>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        static string GetPath(string filename)
        {
            //如果是网站的处理
            if (HttpContext.Current != null)
            {
                return HttpContext.Current.Server.MapPath(filename);
            }
            else
            {
                //应用程序，包括Windows Application和Console程序
                return Environment.CurrentDirectory + filename;
            }
        }

        /// <summary>
        /// 读取Redis配置
        /// </summary>
        /// <returns></returns>
        static RedisClient GetRedisClient()
        {
            //Redis服务IP
            var ip = ConfigManager.GetWebConfig("redisCacheIp", "");
            if (string.IsNullOrEmpty(ip))
                throw new ArgumentException("redisIp config not exist");
            //Redis服务端口
            var port = ConfigManager.GetWebConfig("redisCachePort", 6379);
            //Redis服务密码
            var passwd = ConfigManager.GetWebConfig("redisCachePasswd", null);
            long db = ConfigManager.GetWebConfig("redisCacheDB", 0);

            return new RedisClient(ip, port, passwd, db);
        }

        /// <summary>
        /// 设置系统缓存
        /// </summary>
        /// <param name="namespaces">命名空间</param>
        /// <param name="key">K</param>
        /// <param name="value">V</param>
        /// <param name="ttl">失效时间（秒）</param>
        private static void putWebCache(string namespaces, string key, string value, int ttl = 10)
        {
            try
            {
                if (string.IsNullOrEmpty(namespaces)) throw new ArgumentNullException();
                if (string.IsNullOrEmpty(key)) throw new ArgumentNullException();
                if (string.IsNullOrEmpty(value)) throw new ArgumentNullException();
                if (ttl > 86400) ttl = 86400;
                key = string.Format(formatting, namespaces, key);
                HttpRuntime.Cache.Insert(key, value, null, System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(0, 0, ttl));
            }
            catch (Exception)
            {
                // ignored
            }
        }

        /// <summary>
        /// 读取系统缓存
        /// </summary>
        /// <param name="namespaces">命名空间</param>
        /// <param name="key">K</param>
        /// <returns></returns>
        static string getWebCache(string namespaces, string key)
        {
            try
            {
                if (string.IsNullOrEmpty(namespaces)) throw new ArgumentNullException();
                if (string.IsNullOrEmpty(key)) throw new ArgumentNullException();
                key = string.Format(formatting, namespaces, key);
                return (string)HttpRuntime.Cache.Get(key);
            }
            catch (Exception)
            { return ""; }
        }

        /// <summary>
        /// 设置Redis缓存
        /// </summary>
        /// <param name="namespaces"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="ttl"></param>
        static void putRedisCache(string namespaces, string key, string value, int ttl = 10)
        {
            try
            {
                if (string.IsNullOrEmpty(namespaces)) throw new ArgumentNullException();
                if (string.IsNullOrEmpty(key)) throw new ArgumentNullException();
                if (string.IsNullOrEmpty(value)) throw new ArgumentNullException();
                if (ttl > 1800) ttl = 1800;
                key = string.Format(formatting, namespaces, key);
                using (RedisClient client = GetRedisClient())
                {
                    client.SetEntry(key, value, new TimeSpan(0, 0, ttl));
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        /// <summary>
        /// 读取Redis缓存
        /// </summary>
        /// <param name="namespaces"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private static string getRedisCache(string namespaces, string key)
        {
            try
            {
                if (string.IsNullOrEmpty(namespaces)) throw new ArgumentNullException();
                if (string.IsNullOrEmpty(key)) throw new ArgumentNullException();
                key = string.Format(formatting, namespaces, key);
                using (var client = GetRedisClient())
                {
                    return client.GetValue(key);
                }
            }
            catch (Exception)
            { return ""; }
        }

        /// <summary>
        /// 设置系统缓存
        /// </summary>
        /// <param name="namespaces">命名空间</param>
        /// <param name="key">K</param>
        /// <param name="value">V</param>
        /// <param name="ttl">失效时间（秒）</param>
        public static void PutWebCache(string namespaces, string key, string value, int ttl = 10)
        {
            putWebCache(namespaces, key, value, ttl);
        }

        /// <summary>
        /// 读取系统缓存
        /// </summary>
        /// <param name="namespaces">命名空间</param>
        /// <param name="key">K</param>
        /// <returns></returns>
        public static string GetWebCache(string namespaces, string key)
        {
            return getWebCache(namespaces, key);
        }

        /// <summary>
        /// 设置Redis缓存
        /// </summary>
        /// <param name="namespaces"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="ttl"></param>
        public static void PutRedisCache(string namespaces, string key, string value, int ttl = 10)
        {
            putRedisCache(namespaces, key, value, ttl);
        }

        /// <summary>
        /// 读取Redis缓存
        /// </summary>
        /// <param name="namespaces"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetRedisCache(string namespaces, string key)
        {
            return getRedisCache(namespaces, key);
        }

        public static void PutRedisStreamCache(string namespaces, string key, byte[] stream, int ttl = 10)
        {
            try
            {
                if (string.IsNullOrEmpty(namespaces)) throw new ArgumentNullException();
                if (string.IsNullOrEmpty(key)) throw new ArgumentNullException();
                if (stream.Length <= 0) throw new ArgumentNullException();
                if (ttl > 1800) ttl = 1800;
                key = string.Format(formattingV, namespaces, key);
                using (var client = GetRedisClient())
                {
                    client.Set(key, stream, new TimeSpan(0, 0, ttl));
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public static byte[] GetRedisStreamCache(string namespaces, string key)
        {
            try
            {
                if (string.IsNullOrEmpty(namespaces)) throw new ArgumentNullException();
                if (string.IsNullOrEmpty(key)) throw new ArgumentNullException();
                key = string.Format(formattingV, namespaces, key);
                using (RedisClient client = GetRedisClient())
                {
                    return client.Get(key);
                }
            }
            catch (Exception)
            { return null; }
        }

        public static ICache GetCache(string namespaces)
        {
            if (string.IsNullOrEmpty(namespaces)) throw new ArgumentNullException();
            return new CacheItem(namespaces);
        }
    }

    public interface ICache
    {
        /// <summary>
        /// 设置系统缓存
        /// </summary>
        /// <param name="key">K</param>
        /// <param name="value">V</param>
        /// <param name="ttl">失效时间（秒）</param>
        void PutWebCache(string key, string value, int ttl = 10);

        /// <summary>
        /// 读取系统缓存
        /// </summary>
        /// <param name="key">K</param>
        /// <returns></returns>
        string GetWebCache(string key);

        /// <summary>
        /// 设置Redis缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="ttl"></param>
        void PutRedisCache(string key, string value, int ttl = 10);

        /// <summary>
        /// 读取Redis缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        string GetRedisCache(string key);
    }

    /// <summary>
    /// 
    /// </summary>
    internal class CacheItem : ICache
    {
        private readonly string _ns;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="namespaces"></param>
        public CacheItem(string namespaces)
        {
            _ns = namespaces;
        }

        /// <summary>
        /// 设置系统缓存
        /// </summary>
        /// <param name="key">K</param>
        /// <param name="value">V</param>
        /// <param name="ttl">失效时间（秒）</param>
        public void PutWebCache(string key, string value, int ttl = 10)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException();
            if (string.IsNullOrEmpty(value)) throw new ArgumentNullException();
            CacheManager.PutWebCache(_ns, key, value, ttl);
        }

        /// <summary>
        /// 读取系统缓存
        /// </summary>
        /// <param name="key">K</param>
        /// <returns></returns>
        public string GetWebCache(string key)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException();
            return CacheManager.GetWebCache(_ns, key);
        }

        /// <summary>
        /// 设置Redis缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="ttl"></param>
        public void PutRedisCache(string key, string value, int ttl = 10)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException();
            if (string.IsNullOrEmpty(value)) throw new ArgumentNullException();
            CacheManager.PutRedisCache(_ns, key, value, ttl);
        }

        /// <summary>
        /// 读取Redis缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetRedisCache(string key)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException();
            return CacheManager.GetRedisCache(_ns, key);
        }
    }
}
