using System;
using System.IO;
using System.Web;
using System.Collections.Generic;

using APP.CommonLib.Utils;

namespace APP.CommonLib.Log
{
    /// <summary>
    /// 日志级别枚举
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// 信息
        /// </summary>
        Info = 1,
        /// <summary>
        /// 错误
        /// </summary>
        Error = 2,
        /// <summary>
        /// 警告
        /// </summary>
        Warn = 3,
        /// <summary>
        /// Debug
        /// </summary>
        Debug = 4,
    }

    public class Logger
    {
        /// <summary>
        /// 日志队列
        /// </summary>
        static Queue<LogData> m_Queue = new Queue<LogData>();
        /// <summary>
        /// 
        /// </summary>
        static bool bIsInited = false;

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

        /// <summary>
        /// 记录错误日志
        /// </summary>
        /// <param name="p">错误信息</param>
        public static void Error(string p)
        {
            WriteLog(p, LogLevel.Error, null);
        }

        /// <summary>
        /// 记录错误日志
        /// </summary>
        /// <param name="p">错误信息</param>
        /// <param name="e">异常信息</param>
        public static void Error(string p, Exception e)
        {
            WriteLog(p, LogLevel.Error, e);
        }

        /// <summary>
        /// 记录错误日志
        /// </summary>
        /// <param name="p">错误信息</param>
        /// <param name="args">格式化参数</param>
        public static void Error(string p, params object[] args)
        {
            WriteLog(string.Format(p, args), LogLevel.Error, null);
        }

        /// <summary>
        /// 记录信息日志
        /// </summary>
        /// <param name="p">信息</param>
        public static void Info(string p)
        {
            WriteLog(p, LogLevel.Info, null);
        }

        /// <summary>
        /// 记录信息日志
        /// </summary>
        /// <param name="p">信息</param>
        /// <param name="args">格式化数据</param>
        public static void Info(string p, params object[] args)
        {
            WriteLog(string.Format(p, args), LogLevel.Info, null);
        }

        /// <summary>
        /// 记录警告日志
        /// </summary>
        /// <param name="p">警告信息</param>
        public static void Warn(string p)
        {
            WriteLog(p, LogLevel.Warn, null);
        }

        /// <summary>
        /// 记录警告日志
        /// </summary>
        /// <param name="p">警告信息</param>
        /// <param name="e">异常信息</param>
        public static void Warn(string p, Exception e)
        {
            WriteLog(p, LogLevel.Warn, e);
        }

        /// <summary>
        /// 记录警告日志
        /// </summary>
        /// <param name="p">警告信息</param>
        /// <param name="args">格式化数据</param>
        public static void Warn(string p, params object[] args)
        {
            WriteLog(string.Format(p, args), LogLevel.Warn, null);
        }

        /// <summary>
        /// 记录Debug信息
        /// </summary>
        /// <param name="p">Debug信息</param>
        public static void Debug(string p)
        {
            WriteLog(p, LogLevel.Debug, null);
        }

        /// <summary>
        /// 记录Debug信息
        /// </summary>
        /// <param name="p">Debug信息</param>
        /// <param name="args">格式化数据</param>
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
        /// <summary>
        /// 日志信息
        /// </summary>
        public string Info
        {
            get;
            set;
        }

        /// <summary>
        /// 日志级别
        /// </summary>
        public LogLevel Level
        {
            get;
            set;
        }

        /// <summary>
        /// 异常信息
        /// </summary>
        public Exception Exception
        {
            get;
            set;
        }

        /// <summary>
        /// 日志记录时间
        /// </summary>
        public DateTime CreateDateTime
        {
            get;
            set;
        }
    }
}
