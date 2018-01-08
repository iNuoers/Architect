using System;
using System.Linq;
using ServiceStack.Text;
using ServiceStack.Redis;
using System.Collections.Generic;

using APP.CommonLib.Log;

namespace APP.CommonLib.Redis
{
    /// <summary>
    /// Redis分布式锁帮助类
    /// </summary>
    public class RedisLockHelper
    {
        #region 单例
        private static RedisLockHelper _instance;
        private static readonly object LockObj = new object();

        public static RedisLockHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (LockObj)
                    {
                        if (_instance == null)
                        {
                            _instance = new RedisLockHelper();
                        }
                    }
                }
                return _instance;
            }
        }
        #endregion

        /// <summary>
        /// 单对象获取锁
        /// </summary>
        /// <param name="req">锁对象</param>
        /// <param name="waitLock">是否一直等待，直到加锁成功</param>
        /// <param name="waitTime">waitLock为false时的等待时间</param>
        /// <returns></returns>
        public string GetRedisLock(LockItem req, bool waitLock = true, long waitTime = 0)
        {
            string result = string.Empty;
            if (string.IsNullOrWhiteSpace(req.Id) || req.ItemType == null)
                throw new ArgumentNullException("ID or ItemType can not be empty.");

            string key = string.Format("RedisLock:{0}_{1}", req.ItemType.FullName, req.Id);
            try
            {
                using (RedisClient redisClient = RedisHelper.GetClient())
                {
                    if (GetLock(redisClient, key, waitTime, waitLock))
                        result = key;
                }
            }
            catch (Exception exc)
            {
                Logger.Error("Redis Lock Exception. ", exc);
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="redisClient"></param>
        /// <param name="key"></param>
        /// <param name="waitTime"></param>
        /// <param name="waitLock"></param>
        /// <returns></returns>
        private bool GetLock(RedisClient redisClient, string key, long waitTime, bool waitLock)
        {
            bool result = false;
            if (waitLock)
            {
                do
                {
                    if (LockLogic(redisClient, key))
                        return true;
                } while (true);
            }

            TimeSpan ts = TimeSpan.FromMilliseconds(waitTime);
            long expireTime = DateTime.UtcNow.Add(ts).ToUnixTimeMs();
            do
            {
                if (LockLogic(redisClient, key))
                    return true;
            } while (DateTime.UtcNow.ToUnixTimeMs() < expireTime);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="redisClient"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private bool LockLogic(RedisClient redisClient, string key)
        {
            bool result = false;

            try
            {
                //利用SETNX命令设置锁的值，返回true则表示获得锁并且将值设置成过期时间
                var nx = redisClient.SetValueIfNotExists(key, key);
                if (nx)
                {
                    redisClient.Expire(key, 600);// 设置redis key过期时间10分钟
                    result = true;
                }
            }
            catch (Exception exc)
            {
                Logger.Error("RedisLock exception. ", exc);
                throw new RedisResponseException(string.Format("{0} {1}", exc.Message, exc.StackTrace));
            }

            return result;
        }

        /// <summary>
        /// 获取多对象锁
        /// </summary>
        /// <param name="reqList">对象列表</param>
        /// <param name="waitLock">是否一直等待，直到加锁成功</param>
        /// <param name="waitTime">waitLock为false时的等待时间</param>
        /// <returns></returns>
        public List<string> GetRedisLock(List<LockItem> reqList, bool waitLock = true, long waitTime = 0)
        {
            List<string> keyList = new List<string>();
            foreach (var req in reqList)
            {
                if (string.IsNullOrWhiteSpace(req.Id) || req.ItemType == null)
                    throw new ArgumentNullException("ID or ItemType can not be empty.");

                keyList.Add(string.Format("RedisLock:{0}_{1}", req.ItemType.FullName, req.Id));
            }

            try
            {
                using (RedisNativeClient redisClient = RedisHelper.GetClient())
                {
                    if (waitLock)
                    {
                        do
                        {
                            if (LockItemArr(redisClient, keyList))
                                return keyList;
                        } while (true);
                    }
                    TimeSpan ts = TimeSpan.FromMilliseconds(waitTime);
                    long expireTime = DateTime.UtcNow.Add(ts).ToUnixTimeMs();
                    do
                    {
                        if (LockItemArr(redisClient, keyList))
                            return keyList;
                    } while (DateTime.UtcNow.ToUnixTimeMs() < expireTime);
                }
            }
            catch (Exception exc)
            {
                Logger.Error("Redis Lock Exception. ", exc);
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="redisClient"></param>
        /// <param name="keyList"></param>
        /// <returns></returns>
        private bool LockItemArr(RedisNativeClient redisClient, List<string> keyList)
        {
            bool result = false;

            try
            {
                //利用MSetNx命令设置锁的值，返回true则表示获得锁并且将值设置成过期时间
                byte[][] value = keyList.Select(x => System.Text.Encoding.UTF8.GetBytes(x)).ToArray<byte[]>();
                var nx = redisClient.MSetNx(keyList.ToArray(), value);
                if (nx)
                {
                    foreach (var key in keyList)
                        redisClient.Expire(key, 600);// 设置redis key过期时间10分钟
                    result = true;
                }
            }
            catch (Exception exc)
            {
                Logger.Error("RedisLock exception. ", exc);
                throw new RedisResponseException(string.Format("{0} {1}", exc.Message, exc.StackTrace));
            }

            return result;
        }

        /// <summary>
        /// 释放锁
        /// </summary>
        /// <param name="key">锁key</param>
        public void ReleaseLock(string key)
        {
            try
            {
                using (RedisClient redisClient = RedisHelper.GetClient())
                {
                    TimeSpan ts = (TimeSpan)redisClient.GetTimeToLive(key);

                    if (ts.TotalSeconds > 1)
                        redisClient.Remove(key);
                }
            }
            catch (Exception exc)
            {
                Logger.Error("Redis unLock exception. ", exc);
                throw new RedisResponseException(string.Format("{0} {1}", exc.Message, exc.StackTrace));
            }
        }

        /// <summary>
        /// 批量释放锁
        /// </summary>
        /// <param name="keyList">锁key列表</param>
        public void ReleaseLock(List<string> keyList)
        {
            try
            {
                using (RedisClient redisClient = RedisHelper.GetClient())
                {
                    foreach (var key in keyList)
                    {
                        TimeSpan ts = (TimeSpan)redisClient.GetTimeToLive(key);
                        if (ts.TotalSeconds > 1)
                            redisClient.Remove(key);
                    }
                }
            }
            catch (Exception exc)
            {
                Logger.Error("Redis unLock exception ：{0} {1}", exc.Message, exc.StackTrace);
                throw new RedisResponseException(string.Format("{0} {1}", exc.Message, exc.StackTrace));
            }
        }
    }

    /// <summary>
    /// Redis锁请求对象
    /// </summary>
    public class LockItem
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        public LockItem(string id, Type type)
        {
            Id = id;
            ItemType = type;
        }

        /// <summary>
        /// 对象ID(用于生成锁key)
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 对象类型(用于生成锁key)
        /// </summary>
        public Type ItemType { get; set; }
    }
}