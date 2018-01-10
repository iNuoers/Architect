using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

using APP.CommonLib.Log;

namespace APP.CommonLib.Encrypt
{
    /// <summary>
    /// AES加密帮助类
    /// </summary>
    public class AESEncodeHelper : IEncryptManager
    {
        #region 密钥对
        /// <summary>
        /// 密钥
        /// </summary>
        string _key { get; set; }

        /// <summary>
        /// 盐
        /// </summary>
        string _vector { get; set; }

        #endregion

        #region 构造函数
        /// <summary>
        /// 无参构造 设置初始值
        /// </summary>
        public AESEncodeHelper()
        {
            _key = "mr.ben@app";
            _vector = "aes@mr.ben";
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="key">秘钥</param>
        /// <param name="vector">盐</param>
        public AESEncodeHelper(string key, string vector)
        {
            _key = key;
            _vector = vector;
        }
        #endregion

        /// <summary>
        /// 加密算法
        /// </summary>
        /// <param name="data">要加密的数据</param>
        /// <returns>加密后的数据</returns>
        public string EncryptData(string data)
        {
            byte[] cryptograph = null;
            try
            {
                byte[] plainBates = Encoding.UTF8.GetBytes(data);
                byte[] key = new byte[32];
                Array.Copy(Encoding.UTF8.GetBytes(_key.PadRight(key.Length)), key, key.Length);
                byte[] vector = new byte[16];
                Array.Copy(Encoding.UTF8.GetBytes(_vector.PadRight(vector.Length)), vector, vector.Length);
                Rijndael aes = Rijndael.Create();

                using (MemoryStream stream = new MemoryStream())
                {
                    using (CryptoStream encryptor = new CryptoStream(stream, aes.CreateEncryptor(key, vector), CryptoStreamMode.Write))
                    {
                        // 明文数据写入加密流
                        encryptor.Write(plainBates, 0, plainBates.Length);
                        encryptor.FlushFinalBlock();
                        cryptograph = stream.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("AES加密失败，原因：{0} {1}", ex.Message, ex.StackTrace));
                return "";
            }
            return Convert.ToBase64String(cryptograph);
        }

        /// <summary>
        /// 解密算法
        /// </summary>
        /// <param name="data">要解密的数据</param>
        /// <returns>解密后的数据</returns>
        public string DecryptData(string data)
        {
            byte[] original = null;
            try
            {
                byte[] encryptedBytes = Convert.FromBase64String(data);
                byte[] key = new byte[32];
                Array.Copy(Encoding.UTF8.GetBytes(_key.PadRight(key.Length)), key, key.Length);
                byte[] vectory = new byte[16];
                Array.Copy(Encoding.UTF8.GetBytes(_vector.PadRight(vectory.Length)), vectory, vectory.Length);
                Rijndael aes = Rijndael.Create();
                using (MemoryStream stream = new MemoryStream(encryptedBytes))
                {
                    using (CryptoStream decryptor = new CryptoStream(stream, aes.CreateDecryptor(key, vectory), CryptoStreamMode.Read))
                    {
                        using (MemoryStream originalStream = new MemoryStream())
                        {
                            byte[] buffer = new byte[1024];
                            int readBytes = 0;
                            while ((readBytes = decryptor.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                originalStream.Write(buffer, 0, readBytes);
                            }
                            original = originalStream.ToArray();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("解密失败，原因：{0} {1}", ex.Message, ex.StackTrace));
                return "";
            }
            return Encoding.UTF8.GetString(original);
        }

    }
}
