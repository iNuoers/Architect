using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Drawing;
using System.Drawing.Imaging;

using CommonLib.Qr;
using CommonLib.Utils;
using CommonLib.Cache;
using CommonLib.Security;
using CommonLib.Configuration;

namespace QrWeb.Controllers
{
    public class QrController : Controller
    {
        public JsonResult Query(string id)
        {
            id = EncodeHelper.EncryptString(id);
            string encode = EncodeHelper.ToRandomMd5(id, 3);
            id = encode + "|" + id;
            id = HttpUtility.UrlEncode(id);
            return Json(id, JsonRequestBehavior.AllowGet);
        }

        public FileContentResult Index(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    throw new ArgumentNullException();
                var len = id.IndexOf('|');
                var hash = id.Substring(0, len);
                var content = id.Substring(len + 1);
                if (!EncodeHelper.EqualsRandomMd5(content, hash))
                    throw new ArgumentException();
                content = EncodeHelper.DecryptString(content);

                var data = GetQrImage(content);

                return File(data, "image/jpeg");
            }
            catch (Exception ex)
            {
                Logger.Error("error", ex);
                return null;
            }
        }

        private byte[] GetQrImage(string content)
        {
            var cacheTime = ConfigManager.GetWebConfig("redisCacheTime", 0);

            if (cacheTime != 0)
            {
                var r = CacheManager.GetRedisStreamCache("Qrcode", content);
                if (r != null)
                    return r;
            }

            var middlImg = GetImage();
            var width = ConfigManager.GetWebConfig("QrWidht", 540);
            var height = ConfigManager.GetWebConfig("QrHeight", 540);

            var bmp = QrHelper.GeneratorQrImage(content, middlImg, width, height);
            var stream = new MemoryStream();
            bmp.Save(stream, ImageFormat.Jpeg);

            var data = stream.ToArray();

            if (cacheTime != 0)
                CacheManager.PutRedisStreamCache("Qrcode", content, data, cacheTime);

            return data;
        }

        private Image GetImage()
        {
            return Image.FromFile(HttpContext.Server.MapPath(ConfigManager.GetWebConfig("qrimg", "/File/watermarks/logo.png")));
        }

    }
}
