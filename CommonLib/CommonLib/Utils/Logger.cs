using System;
using System.IO;
using System.Web;
using System.Collections.Generic;

namespace CommonLib.Utils
{
    /// <summary>
    /// 日志级别
    /// </summary>
    public enum LogLevel
    {
        Info = 1,
        Error = 2,
        Warn = 3,
        Debug = 4,
    }

    public class Logger
    {
        static Queue<LogData> m_Queue = new Queue<LogData>();
        //static bool m_Running = false;

        static bool bIsInited = false;


        //public static void WriteHashsLog(string key, List<string> fields)
        //{
        //    WriteLog("key:" + key);
        //    foreach (string field in fields)
        //    {
        //        WriteLog(field);
        //    }
        //    WriteLog("----------------------------------------------------------------");
        //}

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="info">内容</param>
        /// <param name="level">等级</param>
        /// <param name="ex">异常</param>
        public static void WriteLog(string info, LogLevel level, Exception ex)
        {
            LogData data = new LogData();
            data.Info = info;
            data.Level = level;
            data.Exception = ex;
            data.CreateDateTime = DateTime.Now;

            lock (m_Queue)
            {
                m_Queue.Enqueue(data);
                if (!bIsInited)
                {
                    bIsInited = true;
                    ThreadBox.CreateQueueConsumerThreadBox("LoggerThreadBox", 1, Persistence, m_Queue, 100, true);
                }
            }
        }

        ///// <summary>
        ///// 写日志
        ///// </summary>
        ///// <param name="info">内容</param>
        //public static void WriteLog(string info)
        //{
        //    WriteLog(info, LogLevel.Info, null);
        //}

        ///// <summary>
        ///// 写日志
        ///// </summary>
        ///// <param name="format">格式</param>
        ///// <param name="arg0">参数</param>
        //public static void WriteLog(string format, object arg0)
        //{
        //    WriteLog(string.Format(format, arg0));
        //}

        ///// <summary>
        ///// 写日志
        ///// </summary>
        ///// <param name="format">格式</param>
        ///// <param name="args">参数</param>
        //public static void WriteLog(string format, params object[] args)
        //{
        //    WriteLog(string.Format(format, args));
        //}

        ///// <summary>
        ///// 写异常日志
        ///// </summary>
        ///// <param name="ex">异常对象</param>
        //public static void WriteLog(Exception ex)
        //{
        //    WriteLog("Error", LogLevel.Error, ex);
        //}

        /// <summary>
        /// 写日志函数
        /// </summary>
        //private static void PersistenceWorker()
        //{
        //    try
        //    {
        //        while (true)
        //        {
        //            try
        //            {
        //                LogData data = null;
        //                lock (m_Queue)
        //                {
        //                    if (m_Queue.Count <= 0)
        //                        break;
        //                    data = m_Queue.Dequeue();
        //                }
        //                if (data == null)
        //                    break;
        //                using (StreamWriter writer = new StreamWriter(GetWriteLogPath(), true))
        //                {
        //                    writer.WriteLine("\r\nDatetime:{0}", data.CreateDateTime.ToString("yyyyMMdd-HH:mm:ss"));
        //                    writer.WriteLine("Info:{0}", data.Info);
        //                    writer.WriteLine("LogLevel:{0}", data.Level);
        //                    writer.WriteLine("Exception:\r\n{0}", data.Exception.ToString());
        //                    if (data.Exception.InnerException != null)
        //                    {
        //                        writer.WriteLine("InnerException:\r\n{0}\r\n", data.Exception.InnerException.ToString());
        //                    }
        //                }
        //            }
        //            catch
        //            {
        //            }
        //        }
        //    }
        //    catch
        //    {
        //    }
        //    finally
        //    {
        //        lock (m_Queue)
        //            m_Running = false;
        //    }
        //}

        /// <summary>
        /// 写日志函数
        /// </summary>
        private static void Persistence(LogData data)
        {
            using (StreamWriter writer = new StreamWriter(GetWriteLogPath(), true))
            {
                writer.WriteLine(String.Format("[{2}|{0}|{1}]", data.Level, data.Info, data.CreateDateTime.ToString("yyyyMMdd-HH:mm:ss")));
                if (data.Exception != null)
                {
                    writer.WriteLine("Exception:\r\n{0}", data.Exception.ToString());
                    if (data.Exception.InnerException != null)
                    {
                        writer.WriteLine("InnerException:\r\n{0}\r\n", data.Exception.InnerException.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// 获得日志文件路径
        /// </summary>
        /// <returns></returns>
        private static string GetWriteLogPath()
        {
            string path = string.Empty;
            if (HttpContext.Current != null)
            {
                path = HttpContext.Current.Server.MapPath(".") + @"\log\";
            }
            else
            {
                path = System.Threading.Thread.GetDomain().BaseDirectory + string.Format(@"\log\{0}\{1}\{2}\", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            }
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            path = path + string.Format("Log-{0}.log", DateTime.Now.ToString("HH"));

            return path;
        }


        public static void Error(string p)
        {
            WriteLog(p, LogLevel.Error, null);
        }

        public static void Error(string p, Exception e)
        {
            WriteLog(p, LogLevel.Error, e);
        }

        public static void Error(string p, params object[] args)
        {
            WriteLog(string.Format(p, args), LogLevel.Error, null);
        }

        public static void Info(string p)
        {
            WriteLog(p, LogLevel.Info, null);
        }

        public static void Info(string p, params object[] args)
        {
            WriteLog(string.Format(p, args), LogLevel.Info, null);
        }

        public static void Warn(string p, Exception e)
        {
            WriteLog(p, LogLevel.Warn, e);
        }

        public static void Warn(string p)
        {
            WriteLog(p, LogLevel.Warn, null);
        }
        public static void Warn(string p, params object[] args)
        {
            WriteLog(string.Format(p, args), LogLevel.Warn, null);
        }


        public static void Debug(string p, params object[] args)
        {
            WriteLog(string.Format(p, args), LogLevel.Debug, null);
        }
    }

    /// <summary>
    /// 日志对象
    /// </summary>
    class LogData
    {
        public string Info
        {
            get;
            set;
        }

        public LogLevel Level
        {
            get;
            set;
        }

        public Exception Exception
        {
            get;
            set;
        }

        public DateTime CreateDateTime
        {
            get;
            set;
        }
    }
}
