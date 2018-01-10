using System;
using System.Linq;
using System.Collections.Generic;

namespace APP.CommonLib.Encrypt
{
    public class EncryptFactory
    {
        /// <summary>
        /// 加解密版本配置字典
        /// </summary>
        static Dictionary<EncryptVersion, string> encryptVerDic = new Dictionary<EncryptVersion, string>()
        {
            { EncryptVersion.ONE, "APP.CommonLib.Encrypt.EncryptV1, APP.CommonLib" },
            { EncryptVersion.AES, "APP.CommonLib.Encrypt.AESEncodeHelper, APP.CommonLib" }
        };

        /// <summary>
        /// 创建加解密版本类型对象
        /// </summary>
        /// <param name="ver">加解密版本</param>
        /// <param name="args">加解密类型构造函数所需要的参数数组</param>
        /// <returns>加解密类型对象</returns>
        public static IEncryptManager CreateEncryptManager(EncryptVersion ver, params object[] args)
        {
            if (encryptVerDic.ContainsKey(ver))
            {
                Type type = Type.GetType(encryptVerDic[ver]);
                if (type.IsInterface || !type.GetInterfaces().Contains(typeof(IEncryptManager)))
                    return null;

                return Activator.CreateInstance(type, args) as IEncryptManager;
            }
            return null;
        }
    }
}
