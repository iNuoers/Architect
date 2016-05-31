using System;
using System.Drawing;
using System.Collections.Generic;

using ZXing;
using ZXing.QrCode;
using ZXing.QrCode.Internal;

namespace CommonLib.Qr
{
    /// <summary>
    /// 二维码图片管理类
    /// </summary>
    public class QrHelper
    {
        /// <summary>
        /// 生成二维码图片
        /// </summary>
        /// <param name="contents">要生成二维码包含的信息</param>
        /// <param name="width">生成的二维码宽度（默认300像素）</param>
        /// <param name="height">生成的二维码高度（默认300像素）</param>
        /// <returns>二维码图片</returns>
        public static Bitmap GeneratorQrImage(string contents, int width = 300, int height = 300)
        {
            if (string.IsNullOrEmpty(contents))
            {
                return null;
            }

            var options = new QrCodeEncodingOptions
            {
                DisableECI = true,
                CharacterSet = "UTF-8",
                Width = width,
                Height = height,
                ErrorCorrection = ErrorCorrectionLevel.H
            };
            var writer = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = options
            };

            var bitmap = writer.Write(contents);
            return bitmap;
        }

        /// <summary>
        /// 生成中间带有图片的二维码图片
        /// </summary>
        /// <param name="contents">要生成二维码包含的信息</param>
        /// <param name="middleImg">要生成到二维码中间的图片</param>
        /// <param name="width">生成的二维码宽度（默认300像素）</param>
        /// <param name="height">生成的二维码高度（默认300像素）</param>
        /// <returns>中间带有图片的二维码</returns>
        public static Bitmap GeneratorQrImage(string contents, Image middleImg, int width = 300, int height = 300)
        {
            if (string.IsNullOrEmpty(contents))
            {
                return null;
            }
            if (middleImg == null)
            {
                return GeneratorQrImage(contents);
            }
            //构造二维码写码器
            var mutiWriter = new MultiFormatWriter();
            var hint = new Dictionary<EncodeHintType, object>
            {
                {
                    EncodeHintType.CHARACTER_SET, "UTF-8"
                },
                {
                    EncodeHintType.ERROR_CORRECTION, ErrorCorrectionLevel.H
                }
            };

            //生成二维码
            var bm = mutiWriter.encode(contents, BarcodeFormat.QR_CODE, width, height, hint);
            var barcodeWriter = new BarcodeWriter();
            var bitmap = barcodeWriter.Write(bm);

            //获取二维码实际尺寸（去掉二维码两边空白后的实际尺寸）
            var rectangle = bm.getEnclosingRectangle();

            //计算插入图片的大小和位置
            var middleImgW = Math.Min((int)(rectangle[2] / 3.5), middleImg.Width);
            var middleImgH = Math.Min((int)(rectangle[3] / 3.5), middleImg.Height);
            var middleImgL = (bitmap.Width - middleImgW) / 2;
            var middleImgT = (bitmap.Height - middleImgH) / 2;

            //将img转换成bmp格式，否则后面无法创建 Graphics对象
            var bmpimg = new Bitmap(bitmap.Width, bitmap.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using (var g = Graphics.FromImage(bmpimg))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(bitmap, 0, 0);
            }

            //在二维码中插入图片
            var myGraphic = Graphics.FromImage(bmpimg);
            //白底
            myGraphic.FillRectangle(Brushes.White, middleImgL, middleImgT, middleImgW, middleImgH);
            myGraphic.DrawImage(middleImg, middleImgL, middleImgT, middleImgW, middleImgH);
            return bmpimg;
        }
    }
}