using System;
using APP.CommonLib.Cache;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommonLib.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            //CacheManager.PutWebCache("Test.one", "test", "test_value");
            //string value = CacheManager.GetWebCache("Test.one", "test");
            //string value1 = CacheManager.GetWebCache("Test.one1", "test");
            //string value2 = CacheManager.GetWebCache("Test.one", "test1");
            //var obj = CacheManager.RemoveWebCache("Test.one1", "test");
            //string value3 = CacheManager.GetWebCache("Test.one", "test");
            //var obj1 = CacheManager.RemoveWebCache("Test.one", "test");
            //string value4 = CacheManager.GetWebCache("Test.one", "test");

            CacheManager.PutRedisCache("App", "Loing", "123456", -1);
            string rv = CacheManager.GetRedisCache("App", "Loing");
            CacheManager.DelRedisCache("App", "Login");


            CacheManager.PutRedisCache("App1", "Loing1", "123456", 300);
            string rv1 = CacheManager.GetRedisCache("App1", "Loing1");
            CacheManager.DelRedisCache("App1", "Login1");
            string rv2 = CacheManager.GetRedisCache("App1", "Loing1");
        }
    }
}