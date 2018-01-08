using System;
using ServiceStack.Redis;
using System.Collections.Generic;

using APP.CommonLib.IO;
using APP.CommonLib.Log;
using APP.CommonLib.Config;

namespace APP.CommonLib.Redis
{
    /// <summary>
    /// Redis帮助类
    /// </summary>
    public class RedisHelper
    {
        /// <summary>
        /// （主）连接池
        /// </summary>
        private static PooledRedisClientManager MainRedisClientManager = null;

        #region 构造函数

        /// <summary>
        /// 初始化连接池
        /// </summary>
        static RedisHelper()
        {
            string redisConfFile = PathHelper.MergePathName(PathHelper.GetConfigPath(), "Redis.config");
            RedisConfig config = ConfigManager.GetObjectConfig<RedisConfig>(redisConfFile);
            if (MainRedisClientManager == null)
            {
                try
                {
                    IEnumerable<string> readWriteHosts = config.MainRedisServer;
                    MainRedisClientManager = new PooledRedisClientManager(readWriteHosts, null,
                        new RedisClientManagerConfig()
                        {
                            MaxReadPoolSize = config.MaxReadPoolSize,
                            MaxWritePoolSize = config.MaxWritePoolSize,
                            AutoStart = config.AutoStart,
                            DefaultDb = config.DefaultDB,
                        })
                    {
                        PoolTimeout = config.PoolTimeout,
                        ConnectTimeout = config.ConnectTimeout,
                        SocketReceiveTimeout = config.SocketReceiveTimeout,
                        SocketSendTimeout = config.SocketSendTimeout
                    };
                }
                catch (Exception exc)
                {
                    Logger.Error("Redis线程池初始化失败", exc);
                }
            }
        }
        #endregion

        #region 获取Redis客户端
        /// <summary>
        /// 获取一个可用的RedisClient
        /// </summary>
        /// <returns>RedisClient</returns>
        public static RedisClient GetClient(long db = 0)
        {
            RedisClient client = null;
            try
            {
                client = MainRedisClientManager.GetClient() as RedisClient;
                if (db != 0)
                    client.ChangeDb(db);
            }
            catch (Exception exc)
            {
                Logger.Error("获取Redis连接池连接失败", exc);
            }
            return client;
        }
        #endregion

        #region Redis相关操作
        public static bool Add<T>(string key, T value)
        {
            using (RedisClient client = GetClient())
            {
                return client.Add(key, value);
            }
        }

        public static bool Add<T>(string key, T value, DateTime dt)
        {
            using (RedisClient client = GetClient())
            {
                return client.Add(key, value, dt);
            }
        }

        public static bool Add<T>(string key, T value, TimeSpan ts)
        {
            using (RedisClient client = GetClient())
            {
                return client.Add(key, value, ts);
            }
        }

        public static void AddItemToList(string listId, string value)
        {
            using (RedisClient client = GetClient())
            {
                client.AddItemToList(listId, value);
            }
        }
        public static void AddItemToSet(string setId, string item)
        {
            using (RedisClient client = GetClient())
            {
                client.AddItemToSet(setId, item);
            }
        }
        public static bool AddItemToSortedSet(string setId, string value)
        {
            using (RedisClient client = GetClient())
            {
                return client.AddItemToSortedSet(setId, value);
            }
        }
        public static bool AddItemToSortedSet(string setId, string value, double score)
        {
            using (RedisClient client = GetClient())
            {
                return client.AddItemToSortedSet(setId, value, score);
            }
        }
        public static bool AddItemToSortedSet(string setId, string value, long score)
        {
            using (RedisClient client = GetClient())
            {
                return client.AddItemToSortedSet(setId, value, score);
            }
        }
        public static void AddRangeToList(string listId, List<string> values)
        {
            using (RedisClient client = GetClient())
            {
                client.AddRangeToList(listId, values);
            }
        }
        public static void AddRangeToSet(string setId, List<string> items)
        {
            using (RedisClient client = GetClient())
            {
                client.AddRangeToSet(setId, items);
            }
        }
        public static bool AddRangeToSortedSet(string setId, List<string> values, double score)
        {
            using (RedisClient client = GetClient())
            {
                return client.AddRangeToSortedSet(setId, values, score);
            }
        }
        public static bool AddRangeToSortedSet(string setId, List<string> values, long score)
        {
            using (RedisClient client = GetClient())
            {
                return client.AddRangeToSortedSet(setId, values, score);
            }
        }
        public static long AppendToValue(string key, string value)
        {
            using (RedisClient client = GetClient())
            {
                return client.AppendToValue(key, value);
            }
        }

        public static string BlockingDequeueItemFromList(string listId, TimeSpan? timeOut)
        {
            using (RedisClient client = GetClient())
            {
                return client.BlockingDequeueItemFromList(listId, timeOut);
            }
        }
        public static ItemRef BlockingDequeueItemFromLists(string[] listIds, TimeSpan? timeOut)
        {
            using (RedisClient client = GetClient())
            {
                return client.BlockingDequeueItemFromLists(listIds, timeOut);
            }
        }
        public static string BlockingPopAndPushItemBetweenLists(string fromListId, string toListId, TimeSpan? timeOut)
        {
            using (RedisClient client = GetClient())
            {
                return client.BlockingPopAndPushItemBetweenLists(fromListId, toListId, timeOut);
            }
        }
        public static string BlockingPopItemFromList(string listId, TimeSpan? timeOut)
        {
            using (RedisClient client = GetClient())
            {
                return client.BlockingPopItemFromList(listId, timeOut);
            }
        }
        public static ItemRef BlockingPopItemFromLists(string[] listIds, TimeSpan? timeOut)
        {
            using (RedisClient client = GetClient())
            {
                return client.BlockingPopItemFromLists(listIds, timeOut);
            }
        }
        public static string BlockingRemoveStartFromList(string listId, TimeSpan? timeOut)
        {
            using (RedisClient client = GetClient())
            {
                return client.BlockingRemoveStartFromList(listId, timeOut);
            }
        }
        public static ItemRef BlockingRemoveStartFromLists(string[] listIds, TimeSpan? timeOut)
        {
            using (RedisClient client = GetClient())
            {
                return client.BlockingRemoveStartFromLists(listIds, timeOut);
            }
        }
        public static void ChangeDb(long db)
        {
            using (RedisClient client = GetClient())
            {
                client.ChangeDb(db);
            }
        }
        public static RedisClient CloneClient()
        {
            using (RedisClient client = GetClient())
            {
                return client.CloneClient();
            }
        }
        public static bool ContainsKey(string key)
        {
            using (RedisClient client = GetClient())
            {
                return client.ContainsKey(key);
            }
        }
        public static long Decrement(string key, uint amount)
        {
            using (RedisClient client = GetClient())
            {
                return client.Decrement(key, amount);
            }
        }
        public static long DecrementValue(string key)
        {
            using (RedisClient client = GetClient())
            {
                return client.DecrementValue(key);
            }
        }
        public static long DecrementValueBy(string key, int count)
        {
            using (RedisClient client = GetClient())
            {
                return client.DecrementValueBy(key, count);
            }
        }

        public static string DequeueItemFromList(string listId)
        {
            using (RedisClient client = GetClient())
            {
                return client.DequeueItemFromList(listId);
            }
        }
        public static void EnqueueItemOnList(string listId, string value)
        {
            using (RedisClient client = GetClient())
            {
                client.EnqueueItemOnList(listId, value);
            }
        }
        public static long ExecLuaAsInt(string body, params string[] args)
        {
            using (RedisClient client = GetClient())
            {
                return client.ExecLuaAsInt(body, args);
            }
        }
        public static long ExecLuaAsInt(string luaBody, string[] keys, string[] args)
        {
            using (RedisClient client = GetClient())
            {
                return client.ExecLuaAsInt(luaBody, keys, args);
            }
        }
        public static bool ExpireEntryAt(string key, DateTime expireAt)
        {
            using (RedisClient client = GetClient())
            {
                return client.ExpireEntryAt(key, expireAt);
            }
        }
        public static bool ExpireEntryIn(string key, TimeSpan expireIn)
        {
            using (RedisClient client = GetClient())
            {
                return client.ExpireEntryIn(key, expireIn);
            }
        }
        public static T Get<T>(string key)
        {
            using (RedisClient client = GetClient())
            {
                return client.Get<T>(key);
            }
        }
        public static IList<T> GetAll<T>() where T : class, new()
        {
            using (RedisClient client = GetClient())
            {
                return client.GetAll<T>();
            }
        }
        public static IDictionary<string, T> GetAll<T>(IEnumerable<string> keys)
        {
            using (RedisClient client = GetClient())
            {
                return client.GetAll<T>(keys);
            }
        }
        public static Dictionary<string, string> GetAllEntriesFromHash(string hashId)
        {
            using (RedisClient client = GetClient())
            {
                return client.GetAllEntriesFromHash(hashId);
            }
        }
        public static List<string> GetAllItemsFromList(string listId)
        {
            using (RedisClient client = GetClient())
            {
                return client.GetAllItemsFromList(listId);
            }
        }
        public static HashSet<string> GetAllItemsFromSet(string setId)
        {
            using (RedisClient client = GetClient())
            {
                return client.GetAllItemsFromSet(setId);
            }
        }
        public static List<string> GetAllItemsFromSortedSet(string setId)
        {
            using (RedisClient client = GetClient())
            {
                return client.GetAllItemsFromSortedSet(setId);
            }
        }
        public static List<string> GetAllItemsFromSortedSetDesc(string setId)
        {
            using (RedisClient client = GetClient())
            {
                return client.GetAllItemsFromSortedSetDesc(setId);
            }
        }
        public static List<string> GetAllKeys()
        {
            using (RedisClient client = GetClient())
            {
                return client.GetAllKeys();
            }
        }
        public static IDictionary<string, double> GetAllWithScoresFromSortedSet(string setId)
        {
            using (RedisClient client = GetClient())
            {
                return client.GetAllWithScoresFromSortedSet(setId);
            }
        }
        public static string GetAndSetEntry(string key, string value)
        {
            using (RedisClient client = GetClient())
            {
                return client.GetAndSetValue(key, value);
            }
        }
        public static byte[] GetClientList()
        {
            using (RedisClient client = GetClient())
            {
                return client.ClientList();
            }
        }
        public static string GetConfig(string configItem)
        {
            using (RedisClient client = GetClient())
            {
                return client.GetConfig(configItem);
            }
        }
        public static HashSet<string> GetDifferencesFromSet(string fromSetId, params string[] withSetIds)
        {
            using (RedisClient client = GetClient())
            {
                return client.GetDifferencesFromSet(fromSetId, withSetIds);
            }
        }
        public static T GetFromHash<T>(object id)
        {
            using (RedisClient client = GetClient())
            {
                return client.GetFromHash<T>(id);
            }
        }
        public static long GetHashCount(string hashId)
        {
            using (RedisClient client = GetClient())
            {
                return client.GetHashCount(hashId);
            }
        }
        public static List<string> GetHashKeys(string hashId)
        {
            using (RedisClient client = GetClient())
            {
                return client.GetHashKeys(hashId);
            }
        }
        public static List<string> GetHashValues(string hashId)
        {
            using (RedisClient client = GetClient())
            {
                return client.GetHashValues(hashId);
            }
        }
        public static HashSet<string> GetIntersectFromSets(params string[] setIds)
        {
            using (RedisClient client = GetClient())
            {
                return client.GetIntersectFromSets(setIds);
            }
        }
        public static string GetItemFromList(string listId, int listIndex)
        {
            using (RedisClient client = GetClient())
            {
                return client.GetItemFromList(listId, listIndex);
            }
        }
        public static long GetItemIndexInSortedSet(string setId, string value)
        {
            using (RedisClient client = GetClient())
            {
                return client.GetItemIndexInSortedSet(setId, value);
            }
        }
        public static long GetItemIndexInSortedSetDesc(string setId, string value)
        {
            using (RedisClient client = GetClient())
            {
                return client.GetItemIndexInSortedSetDesc(setId, value);
            }
        }
        public static double GetItemScoreInSortedSet(string setId, string value)
        {
            using (RedisClient client = GetClient())
            {
                return client.GetItemScoreInSortedSet(setId, value);
            }
        }
        public static long GetListCount(string listId)
        {
            using (RedisClient client = GetClient())
            {
                return client.GetListCount(listId);
            }
        }
        public static string GetRandomItemFromSet(string setId)
        {
            using (RedisClient client = GetClient())
            {
                return client.GetRandomItemFromSet(setId);
            }
        }
        public static string GetRandomKey()
        {
            using (RedisClient client = GetClient())
            {
                return client.GetRandomKey();
            }
        }
        public static List<string> GetRangeFromList(string listId, int startingFrom, int endingAt)
        {
            using (RedisClient client = GetClient())
            {
                return client.GetRangeFromList(listId, startingFrom, endingAt);
            }
        }
        public static List<string> GetRangeFromSortedList(string listId, int startingFrom, int endingAt)
        {
            using (RedisClient client = GetClient())
            {
                return client.GetRangeFromSortedList(listId, startingFrom, endingAt);
            }
        }
        public static List<string> GetRangeFromSortedSet(string setId, int fromRank, int toRank)
        {
            using (RedisClient client = GetClient())
            {
                return client.GetRangeFromSortedSet(setId, fromRank, toRank);
            }
        }
        public static List<string> GetRangeFromSortedSetByHighestScore(string setId, double fromScore, double toScore)
        {
            using (RedisClient client = GetClient())
            {
                return client.GetRangeFromSortedSetByHighestScore(setId, fromScore, toScore);
            }
        }
        public static List<string> GetRangeFromSortedSetByHighestScore(string setId, long fromScore, long toScore)
        {
            using (RedisClient client = GetClient())
            {
                return client.GetRangeFromSortedSetByHighestScore(setId, fromScore, toScore);
            }
        }
        public static List<string> GetRangeFromSortedSetByHighestScore(string setId, string fromStringScore, string toStringScore)
        {
            using (RedisClient client = GetClient())
            {
                return client.GetRangeFromSortedSetByHighestScore(setId, fromStringScore, toStringScore);
            }
        }
        public static List<string> GetRangeFromSortedSetByLowestScore(string setId, double fromScore, double toScore)
        {
            using (RedisClient client = GetClient())
            {
                return client.GetRangeFromSortedSetByLowestScore(setId, fromScore, toScore);
            }
        }
        public static List<string> GetRangeFromSortedSetDesc(string setId, int fromRank, int toRank)
        {
            using (RedisClient client = GetClient())
            {
                return client.GetRangeFromSortedSetDesc(setId, fromRank, toRank);
            }
        }
        public static IDictionary<string, double> GetRangeWithScoresFromSortedSet(string setId, int fromRank, int toRank)
        {
            using (RedisClient client = GetClient())
            {
                return client.GetRangeWithScoresFromSortedSet(setId, fromRank, toRank);
            }
        }
        public static long GetSetCount(string setId)
        {
            using (RedisClient client = GetClient())
            {
                return client.GetSetCount(setId);
            }
        }
        public static List<string> GetSortedEntryValues(string setId, int startingFrom, int endingAt)
        {
            using (RedisClient client = GetClient())
            {
                return client.GetSortedEntryValues(setId, startingFrom, endingAt);
            }
        }
        public static List<string> GetSortedItemsFromList(string listId, SortOptions sortOptions)
        {
            using (RedisClient client = GetClient())
            {
                return client.GetSortedItemsFromList(listId, sortOptions);
            }
        }
        public static long GetSortedSetCount(string setId)
        {
            using (RedisClient client = GetClient())
            {
                return client.GetSortedSetCount(setId);
            }
        }
        public static long GetSortedSetCount(string setId, double fromScore, double toScore)
        {
            using (RedisClient client = GetClient())
            {
                return client.GetSortedSetCount(setId, fromScore, toScore);
            }
        }
        public static long GetSortedSetCount(string setId, long fromScore, long toScore)
        {
            using (RedisClient client = GetClient())
            {
                return client.GetSortedSetCount(setId, fromScore, toScore);
            }
        }
        public static long GetSortedSetCount(string setId, string fromStringScore, string toStringScore)
        {
            using (RedisClient client = GetClient())
            {
                return client.GetSortedSetCount(setId, fromStringScore, toStringScore);
            }
        }
        public static TimeSpan? GetTimeToLive(string key)
        {
            using (RedisClient client = GetClient())
            {
                return client.GetTimeToLive(key);
            }
        }
        public static HashSet<string> GetUnionFromSets(params string[] setIds)
        {
            using (RedisClient client = GetClient())
            {
                return client.GetUnionFromSets(setIds);
            }
        }
        public static string GetValue(string key)
        {
            using (RedisClient client = GetClient())
            {
                return client.GetValue(key);
            }
        }
        public static string GetValueFromHash(string hashId, string key)
        {
            using (RedisClient client = GetClient())
            {
                return client.GetValueFromHash(hashId, key);
            }
        }
        public static List<string> GetValues(List<string> keys)
        {
            using (RedisClient client = GetClient())
            {
                return client.GetValues(keys);
            }
        }
        public static List<T> GetValues<T>(List<string> keys)
        {
            using (RedisClient client = GetClient())
            {
                return client.GetValues<T>(keys);
            }
        }
        public static List<string> GetValuesFromHash(string hashId, params string[] keys)
        {
            using (RedisClient client = GetClient())
            {
                return client.GetValuesFromHash(hashId, keys);
            }
        }
        public static long Increment(string key, uint amount)
        {
            using (RedisClient client = GetClient())
            {
                return client.Increment(key, amount);
            }
        }
        public static double IncrementItemInSortedSet(string setId, string value, double incrementBy)
        {
            using (RedisClient client = GetClient())
            {
                return client.IncrementItemInSortedSet(setId, value, incrementBy);
            }
        }
        public static double IncrementItemInSortedSet(string setId, string value, long incrementBy)
        {
            using (RedisClient client = GetClient())
            {
                return client.IncrementItemInSortedSet(setId, value, incrementBy);
            }
        }
        public static long IncrementValue(string key)
        {
            using (RedisClient client = GetClient())
            {
                return client.IncrementValue(key);
            }
        }
        public static long IncrementValueBy(string key, int count)
        {
            using (RedisClient client = GetClient())
            {
                return client.IncrementValueBy(key, count);
            }
        }
        public static long IncrementValueInHash(string hashId, string key, int incrementBy)
        {
            using (RedisClient client = GetClient())
            {
                return client.IncrementValueInHash(hashId, key, incrementBy);
            }
        }
        public static long IncrementValueInHash(string hashId, string key, long incrementBy)
        {
            using (RedisClient client = GetClient())
            {
                return client.IncrementValueInHash(hashId, key, incrementBy);
            }
        }
        public static string PopAndPushItemBetweenLists(string fromListId, string toListId)
        {
            using (RedisClient client = GetClient())
            {
                return client.PopAndPushItemBetweenLists(fromListId, toListId);
            }
        }
        public static string PopItemFromList(string listId)
        {
            using (RedisClient client = GetClient())
            {
                return client.PopItemFromList(listId);
            }
        }
        public static string PopItemFromSet(string setId)
        {
            using (RedisClient client = GetClient())
            {
                return client.PopItemFromSet(setId);
            }
        }
        public static string PopItemWithHighestScoreFromSortedSet(string setId)
        {
            using (RedisClient client = GetClient())
            {
                return client.PopItemWithHighestScoreFromSortedSet(setId);
            }
        }
        public static string PopItemWithLowestScoreFromSortedSet(string setId)
        {
            using (RedisClient client = GetClient())
            {
                return client.PopItemWithLowestScoreFromSortedSet(setId);
            }
        }
        public static void PrependItemToList(string listId, string value)
        {
            using (RedisClient client = GetClient())
            {
                client.PrependItemToList(listId, value);
            }
        }
        public static void PrependRangeToList(string listId, List<string> values)
        {
            using (RedisClient client = GetClient())
            {
                client.PrependRangeToList(listId, values);
            }
        }
        public static void PushItemToList(string listId, string value)
        {
            using (RedisClient client = GetClient())
            {
                client.PushItemToList(listId, value);
            }
        }
        public static bool Remove(string key)
        {
            using (RedisClient client = GetClient())
            {
                return client.Remove(key);
            }
        }
        public static void RemoveAll(IEnumerable<string> keys)
        {
            using (RedisClient client = GetClient())
            {
                client.RemoveAll(keys);
            }
        }
        public static void RemoveAllFromList(string listId)
        {
            using (RedisClient client = GetClient())
            {
                client.RemoveAllFromList(listId);
            }
        }
        public static string RemoveEndFromList(string listId)
        {
            using (RedisClient client = GetClient())
            {
                return client.RemoveEndFromList(listId);
            }
        }
        public static bool RemoveEntry(params string[] keys)
        {
            using (RedisClient client = GetClient())
            {
                return client.RemoveEntry(keys);
            }
        }
        public static bool RemoveEntryFromHash(string hashId, string key)
        {
            using (RedisClient client = GetClient())
            {
                return client.RemoveEntryFromHash(hashId, key);
            }
        }
        public static long RemoveItemFromList(string listId, string value)
        {
            using (RedisClient client = GetClient())
            {
                return client.RemoveItemFromList(listId, value);
            }
        }
        public static long RemoveItemFromList(string listId, string value, int noOfMatches)
        {
            using (RedisClient client = GetClient())
            {
                return client.RemoveItemFromList(listId, value, noOfMatches);
            }
        }
        public static void RemoveItemFromSet(string setId, string item)
        {
            using (RedisClient client = GetClient())
            {
                client.RemoveItemFromSet(setId, item);
            }
        }
        public static bool RemoveItemFromSortedSet(string setId, string value)
        {
            using (RedisClient client = GetClient())
            {
                return client.RemoveItemFromSortedSet(setId, value);
            }
        }
        public static long RemoveRangeFromSortedSet(string setId, int minRank, int maxRank)
        {
            using (RedisClient client = GetClient())
            {
                return client.RemoveRangeFromSortedSet(setId, minRank, maxRank);
            }
        }
        public static long RemoveRangeFromSortedSetByScore(string setId, double fromScore, double toScore)
        {
            using (RedisClient client = GetClient())
            {
                return client.RemoveRangeFromSortedSetByScore(setId, fromScore, toScore);
            }
        }
        public static long RemoveRangeFromSortedSetByScore(string setId, long fromScore, long toScore)
        {
            using (RedisClient client = GetClient())
            {
                return client.RemoveRangeFromSortedSetByScore(setId, fromScore, toScore);
            }
        }
        public static string RemoveStartFromList(string listId)
        {
            using (RedisClient client = GetClient())
            {
                return client.RemoveStartFromList(listId);
            }
        }
        public static void RenameKey(string fromName, string toName)
        {
            using (RedisClient client = GetClient())
            {
                client.RenameKey(fromName, toName);
            }
        }
        public static bool Replace<T>(string key, T value)
        {
            using (RedisClient client = GetClient())
            {
                return client.Replace<T>(key, value);
            }
        }
        public static bool Replace<T>(string key, T value, DateTime expiresAt)
        {
            using (RedisClient client = GetClient())
            {
                return client.Replace<T>(key, value, expiresAt);
            }
        }
        public static bool Replace<T>(string key, T value, TimeSpan expiresIn)
        {
            using (RedisClient client = GetClient())
            {
                return client.Replace<T>(key, value, expiresIn);
            }
        }
        public static List<string> SearchKeys(string pattern)
        {
            using (RedisClient client = GetClient())
            {
                return client.SearchKeys(pattern);
            }
        }
        public static bool Set<T>(string key, T value)
        {
            using (RedisClient client = GetClient())
            {
                return client.Set<T>(key, value);
            }
        }
        public static bool Set<T>(string key, T value, DateTime expiresAt)
        {
            using (RedisClient client = GetClient())
            {
                return client.Set<T>(key, value, expiresAt);
            }
        }
        public static bool Set<T>(string key, T value, TimeSpan expiresIn)
        {
            using (RedisClient client = GetClient())
            {
                return client.Set<T>(key, value, expiresIn);
            }
        }
        public static void SetAll(Dictionary<string, string> map)
        {
            using (RedisClient client = GetClient())
            {
                client.SetAll(map);
            }
        }
        public static void SetAll<T>(IDictionary<string, T> values)
        {
            using (RedisClient client = GetClient())
            {
                client.SetAll<T>(values);
            }
        }
        public static void SetAll(IEnumerable<string> keys, IEnumerable<string> values)
        {
            using (RedisClient client = GetClient())
            {
                client.SetAll(keys, values);
            }
        }
        protected void SetConfig(string configItem, string value)
        {
            using (RedisClient client = GetClient())
            {
                client.SetConfig(configItem, value);
            }
        }
        public static bool SetContainsItem(string setId, string item)
        {
            using (RedisClient client = GetClient())
            {
                return client.SetContainsItem(setId, item);
            }
        }
        public static void SetEntry(string key, string value)
        {
            using (RedisClient client = GetClient())
            {
                client.SetValue(key, value);
            }
        }
        public static void SetEntry(string key, string value, TimeSpan expireIn)
        {
            using (RedisClient client = GetClient())
            {
                client.SetValue(key, value, expireIn);
            }
        }
        public static bool SetEntryInHash(string hashId, string key, string value)
        {
            using (RedisClient client = GetClient())
            {
                return client.SetEntryInHash(hashId, key, value);
            }
        }
        public static bool SetEntryInHashIfNotExists(string hashId, string key, string value)
        {
            using (RedisClient client = GetClient())
            {
                return client.SetEntryInHashIfNotExists(hashId, key, value);
            }
        }
        public static void SetItemInList(string listId, int listIndex, string value)
        {
            using (RedisClient client = GetClient())
            {
                client.SetItemInList(listId, listIndex, value);
            }
        }
        public static void SetRangeInHash(string hashId, IEnumerable<KeyValuePair<string, string>> keyValuePairs)
        {
            using (RedisClient client = GetClient())
            {
                client.SetRangeInHash(hashId, keyValuePairs);
            }
        }
        public static bool SortedSetContainsItem(string setId, string value)
        {
            using (RedisClient client = GetClient())
            {
                return client.SortedSetContainsItem(setId, value);
            }
        }
        public static T Store<T>(T entity) where T : class, new()
        {
            using (RedisClient client = GetClient())
            {
                return client.Store<T>(entity);
            }
        }
        public static void StoreAll<TEntity>(IEnumerable<TEntity> entities) where TEntity : class, new()
        {
            using (RedisClient client = GetClient())
            {
                client.StoreAll<TEntity>(entities);
            }
        }
        public static void StoreAsHash<T>(T entity)
        {
            using (RedisClient client = GetClient())
            {
                client.StoreAsHash<T>(entity);
            }
        }
        public static void StoreDifferencesFromSet(string intoSetId, string fromSetId, params string[] withSetIds)
        {
            using (RedisClient client = GetClient())
            {
                client.StoreDifferencesFromSet(intoSetId, fromSetId, withSetIds);
            }
        }
        public static void StoreIntersectFromSets(string intoSetId, params string[] setIds)
        {
            using (RedisClient client = GetClient())
            {
                client.StoreIntersectFromSets(intoSetId, setIds);
            }
        }
        public static long StoreIntersectFromSortedSets(string intoSetId, params string[] setIds)
        {
            using (RedisClient client = GetClient())
            {
                return client.StoreIntersectFromSortedSets(intoSetId, setIds);
            }
        }
        public static object StoreObject(object entity)
        {
            using (RedisClient client = GetClient())
            {
                return client.StoreObject(entity);
            }
        }
        public static void StoreUnionFromSets(string intoSetId, params string[] setIds)
        {
            using (RedisClient client = GetClient())
            {
                client.StoreUnionFromSets(intoSetId, setIds);
            }
        }
        public static long StoreUnionFromSortedSets(string intoSetId, params string[] setIds)
        {
            using (RedisClient client = GetClient())
            {
                return client.StoreUnionFromSortedSets(intoSetId, setIds);
            }
        }
        public static void TrimList(string listId, int keepStartingFrom, int keepEndingAt)
        {
            using (RedisClient client = GetClient())
            {
                client.TrimList(listId, keepStartingFrom, keepEndingAt);
            }
        }
        public static bool Expire(string key, int seconds)
        {
            using (RedisClient client = GetClient())
            {
                return client.Expire(key, seconds);
            }
        }
        public static bool ExpireAt(string key, long unixTime)
        {
            using (RedisClient client = GetClient())
            {
                return client.ExpireAt(key, unixTime);
            }
        }
        protected void FlushAll()
        {
            using (RedisClient client = GetClient())
            {
                client.FlushAll();
            }
        }
        protected void FlushDb()
        {
            using (RedisClient client = GetClient())
            {
                client.FlushDb();
            }
        }
        public static byte[] Get(string key)
        {
            using (RedisClient client = GetClient())
            {
                return client.Get(key);
            }
        }
        #endregion
    }
}