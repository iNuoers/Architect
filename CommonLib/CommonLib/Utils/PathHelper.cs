using System.Web;

namespace CommonLib.Utils
{
    public class PathHelper
    {
        public static bool IsWeb { get { return HttpContext.Current != null; } }

        public static string AppPath { get { return IsWeb ? HttpContext.Current.Request.ApplicationPath : null; } }
        public static string MapPath { get { return IsWeb ? HttpContext.Current.Server.MapPath(AppPath) : System.AppDomain.CurrentDomain.BaseDirectory; } }

        public static string MergePathName(string path, string sub)
        {
            path = path.Trim();
            sub = sub.Trim();

            if (!path.EndsWith("\\"))
            {
                path += '\\';
            }

            return path + sub;
        }

        public static string GetConfigPath()
        {
            return MergePathName(MapPath, "Config");
        }

        public static string MergeUrl(string path, string sub)
        {
            path = path.Trim();
            sub = sub.Trim();

            if (!path.EndsWith("/"))
            {
                path += '/';
            }

            return path + sub;
        }
    }
}
