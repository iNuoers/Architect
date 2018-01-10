using APP.CommonLib.Config;
using APP.CommonLib.Log;
using System;
using System.Text;

namespace APP.CommonLib.Encrypt
{
    public class EncryptV1 : IEncryptManager
    {
        /// <summary>
        /// 异或
        /// </summary>
        /// <param name="source">数据</param>
        /// <param name="num">异或秘钥</param>
        /// <returns>异或后的数据</returns>
        static string XORWithNum(string source, int num)
        {
            char[] result = new char[source.Length];
            int size = source.Length;
            for (int i = 0; i < size; i++)
            {
                result[i] = (char)(source[i] ^ num);
            }
            return new string(result);
        }

        /// <summary>
        /// 加密数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>加密后的数据</returns>
        public string EncryptData(string data)
        {
            string result = string.Empty;
            if (!string.IsNullOrEmpty(data))
            {
                try
                {
                    int num = ConfigManager.GetWebConfig("EncryptNum", 64);
                    string xor = XORWithNum(data, num);
                    result = Convert.ToBase64String(Encoding.UTF8.GetBytes(xor));
                }
                catch (Exception ex)
                {
                    Logger.Error(string.Format("EncryptV1 -- 加密失败，原因：{0} {1}", ex.Message, ex.StackTrace));
                }
            }
            return result;
        }

        /// <summary>
        /// 解密数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>解密后的数据</returns>
        public string DecryptData(string data)
        {
            string result = string.Empty;
            try
            {
                string s1 = string.Empty;
                s1 = Encoding.UTF8.GetString(Convert.FromBase64String(data));
                int num = ConfigManager.GetWebConfig("EncryptNum", 64);
                result = XORWithNum(s1, num);
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("EncryptV1 -- 解密失败，原因：{0} {1}", ex.Message, ex.StackTrace));
            }
            return result;
        }
    }
}