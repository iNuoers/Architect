using System;
using System.IO;
using System.Net;
using System.Text;

using CommonLib.Utils;

namespace CommonLib.xHttp
{
    /// <summary>
    /// FTP帮助类
    /// </summary>
    public class xFtpHelper
    {
        #region 上传文件
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="localFile">要上传到FTP服务器的文件</param>
        /// <param name="ftpPath"></param>
        public static void UpLoadFile(byte[] filestream, string filename, string ftpPath, string ftpUser, string ftpPassword)
        {
            if (string.IsNullOrEmpty(filename))
                throw new ArgumentNullException("filename is empty");

            if (ftpUser == null)
            {
                ftpUser = "";
            }
            if (ftpPassword == null)
            {
                ftpPassword = "";
            }

            FtpWebRequest ftpWebRequest = null;
            MemoryStream localFileStream = null;
            Stream requestStream = null;
            try
            {
                ftpWebRequest = (FtpWebRequest)FtpWebRequest.Create(PathHelper.MergeUrl(ftpPath, filename));
                ftpWebRequest.Credentials = new NetworkCredential(ftpUser, ftpPassword);
                ftpWebRequest.UseBinary = true;
                ftpWebRequest.KeepAlive = false;
                ftpWebRequest.Method = WebRequestMethods.Ftp.UploadFile;
                ftpWebRequest.ContentLength = filestream.Length;
                int buffLength = 4096;
                byte[] buff = new byte[buffLength];
                int contentLen;
                //localFileStream = new FileInfo(localFile).OpenRead();
                localFileStream = new MemoryStream(filestream);
                requestStream = ftpWebRequest.GetRequestStream();
                contentLen = localFileStream.Read(buff, 0, buffLength);
                while (contentLen != 0)
                {
                    requestStream.Write(buff, 0, contentLen);
                    contentLen = localFileStream.Read(buff, 0, buffLength);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("ftp upload failed", ex);
                throw ex;
                //MyLog.ShowMessage(ex.Message, "FileUpLoad0001");
            }
            finally
            {
                if (requestStream != null)
                {
                    requestStream.Close();
                }
                if (localFileStream != null)
                {
                    localFileStream.Close();
                }
            }
        }
        #endregion

        #region 文件夹管理

        /// <summary>
        /// 新建目录
        /// </summary>
        /// <param name="ftpPath"></param>
        /// <param name="dirName"></param>
        public static void MakeDir(string ftpPath, string dirName, string ftpUser, string ftpPassword)
        {
            try
            {
                //实例化FTP
                FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(PathHelper.MergeUrl(ftpPath, dirName));

                request.Credentials = new NetworkCredential(ftpUser, ftpPassword);
                //指定FTP操作类型为创建目录
                request.Method = WebRequestMethods.Ftp.MakeDirectory;
                //获取FTP服务器的响应
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                response.Close();
            }
            catch (Exception ex)
            {
                //Logger.Error("ftp upload failed", ex);
                //throw ex;
                //MyLog.ShowMessage(ex.Message, "MakeDir");
            }
        }

        /// <summary>
        /// 检查目录是否存在
        /// </summary>
        /// <param name="ftpPath">要检查的目录的上一级目录</param>
        /// <param name="dirName">要检查的目录名</param>
        /// <returns>存在返回true，否则false</returns>
        public static bool CheckDirectoryExist(string ftpPath, string dirName, string ftpUser, string ftpPassword)
        {
            bool result = false;
            try
            {
                //实例化FTP连接
                FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpPath));
                request.Credentials = new NetworkCredential(ftpUser, ftpPassword);
                //指定FTP操作类型为创建目录
                request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                //获取FTP服务器的响应
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.Default);

                StringBuilder str = new StringBuilder();
                string line = sr.ReadLine();
                while (line != null)
                {
                    str.Append(line);
                    str.Append("|");
                    line = sr.ReadLine();
                }
                string[] datas = str.ToString().Split('|');

                for (int i = 0; i < datas.Length; i++)
                {
                    if (datas[i].Contains("<DIR>"))
                    {
                        int index = datas[i].IndexOf("<DIR>");
                        string name = datas[i].Substring(index + 5).Trim();
                        if (name == dirName)
                        {
                            result = true;
                            break;
                        }
                    }
                }

                sr.Close();
                sr.Dispose();
                response.Close();
            }
            catch (Exception ex)
            {
                Logger.Error("ftp upload failed", ex);
                throw ex;
                //MyLog.ShowMessage(ex.Message, "MakeDir");
            }
            return result;
        }

        #endregion
    }
}
