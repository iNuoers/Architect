using System;
using System.IO;
using System.Web;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using APP.CommonLib.Log;
using APP.CommonLib.Utils;
using APP.CommonLib.XService;

namespace APP.CommonLib.XHttp
{
    /// <summary>
    /// 自定义httpHandler
    /// </summary>
    public class XHttpHandler : IHttpHandler
    {
        /// <summary>
        /// 指示其他请求是否可以使用 IHttpHandler 实例
        /// </summary>
        public bool IsReusable
        {
            get { return true; }
        }

        /// <summary>
        /// HTML编码以及SQL特殊字符转译
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        string TranslateReq(string text)
        {
            text = Regex.Replace(text, "^<$", "", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "^>$", "", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "^select$", "", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "^insert$", "", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "^update$", "", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "^create$", "", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "^delete from$", "", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "^count''$", "", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "^drop table$", "", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "^truncate$", "", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "^asc$", "", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "^mid$", "", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "^char$", "", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "^xp_cmdshell$", "", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "^exec master$", "", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "^net localgroup administrators$", "", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "^and$", "", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "^net user$", "", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "^or$", "", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "^Exec$", "", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "^Execute$", "", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "^net$", "", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "^script$", "", RegexOptions.IgnoreCase);

            return text;
        }

        public void ProcessRequest(HttpContext context)
        {
            var sw = Stopwatch.StartNew();
            var guid = string.Empty;

            try
            {
                var path = context.Request.Url.AbsolutePath;
                if (path != "/api")
                    throw new Exception("path not support");
                if (context.Request.InputStream == null)
                    throw new ArgumentException("input not support");

                string jsonReq;
                using (var stream = new StreamReader(context.Request.InputStream, Encoding.UTF8))
                {
                    jsonReq = stream.ReadToEnd();
                }

                if (string.IsNullOrEmpty(jsonReq))
                    throw new ArgumentException("input not support");

                // HTML编码以及SQL特殊字符串转义
                jsonReq = TranslateReq(jsonReq);

                XHttpRequest request = JsonHelper.JsonDeserialize<XHttpRequest>(jsonReq);
                if (request == null)
                    throw new ArgumentException("input not support");
                request.TS = DateTime.UtcNow.Ticks;
                guid = request.G;
                HttpContext.Current.Items.Add("jsonReq", jsonReq);

                XHttpResponse response = ServiceEngine.Handler(request, context);

                sw.Stop();
                response.Tt = sw.ElapsedMilliseconds;
                response.G = guid;
                context.Response.Write(JsonHelper.JsonSerializer(response));
            }
            catch (MethodAccessException ex)
            {
                sw.Stop();
                context.Response.Write(JsonHelper.JsonSerializer(XHttpResponse.Exception(sw.ElapsedMilliseconds, guid, (int)ServiceResultStatus.InvalidParameter, ex.Message)));
                Logger.Error("{3}:{0},{1} {2}", guid, ex.Message, ex.StackTrace, WebHelper.GetIP());
            }
            catch (ArgumentException ex)
            {
                sw.Stop();
                context.Response.Write(JsonHelper.JsonSerializer(XHttpResponse.Exception(sw.ElapsedMilliseconds, guid, (int)ServiceResultStatus.InvalidParameter, ex.Message)));
                Logger.Error("{3}:{0},{1} {2}", guid, ex.Message, ex.StackTrace, WebHelper.GetIP());
            }
            catch (Exception ex)
            {
                sw.Stop();
                context.Response.Write(JsonHelper.JsonSerializer(XHttpResponse.Exception(sw.ElapsedMilliseconds, guid, (int)ServiceResultStatus.Error, ex.Message)));
                Logger.Error("{2}:{0},{1}", guid, ex.Message, "");
            }
        }

    }
}
