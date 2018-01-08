using System;
using System.Collections.Generic;

namespace APP.CommonLib.Cache
{
    internal class CacheItem : ICache
    {
        string ns = "";

        public CacheItem(string namespaces)
        {
            ns = namespaces;
        }

        /// <summary>
        /// 设置服务器缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="value">缓存数据</param>
        /// <param name="ttl">过期时间</param>
        public void PutWebCache(string key, string value, int ttl = 10)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException();
            if (string.IsNullOrEmpty(value)) throw new ArgumentNullException();
            CacheManager.PutWebCache(ns, key, value, ttl);
        }

        /// <summary>
        /// 读取服务器缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <returns></returns>
        public string GetWebCache(string key)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException();
            return CacheManager.GetWebCache(ns, key);
        }

        /// <summary>
        /// 设置Redis缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="value">缓存数据</param>
        /// <param name="ttl">过期时间</param>
        public void PutRedisCache(string key, string value, int ttl = 10)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException();
            if (string.IsNullOrEmpty(value)) throw new ArgumentNullException();
            CacheManager.PutRedisCache(ns, key, value, ttl);
        }

        /// <summary>
        /// 读取Redis缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <returns></returns>
        public string GetRedisCache(string key)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException();
            return CacheManager.GetRedisCache(ns, key);
        }

        /// <summary>
        /// 删除服务器缓存
        /// </summary>
        /// <param name="key"></param>
        public void DelWebCache(string key)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException();
            CacheManager.DelWebCache(ns, key);
        }

        /// <summary>
        /// 删除Redis缓存
        /// </summary>
        /// <param name="key"></param>
        public void DelRedisCache(string key)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException();
            CacheManager.DelRedisCache(ns, key);
        }

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key"></param>
        public void DelRedisStreamCache(string key)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException();
            CacheManager.DelRedisStreamCache(ns, key);
        }
    }
}