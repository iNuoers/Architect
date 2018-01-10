
namespace APP.CommonLib.Encrypt
{
    /// <summary>
    /// 加解密版本枚举
    /// </summary>
    public enum EncryptVersion
    {
        /// <summary>
        /// 第一版加密
        /// </summary>
        ONE = 1,

        /// <summary>
        /// AES加密
        /// </summary>
        AES = 2
    }

    /// <summary>
    /// 加解密类型接口
    /// </summary>
    public interface IEncryptManager
    {
        /// <summary>
        /// 加密算法
        /// </summary>
        /// <param name="data">要加密的数据</param>
        /// <returns>加密后的数据</returns>
        string EncryptData(string data);

        /// <summary>
        /// 解密算法
        /// </summary>
        /// <param name="data">要解密的数据</param>
        /// <returns>解密后的数据</returns>
        string DecryptData(string data);

    }
}