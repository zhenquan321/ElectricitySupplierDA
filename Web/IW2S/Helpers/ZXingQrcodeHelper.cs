using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;
using ZXing.QrCode.Internal;

namespace IW2S.Helpers
{
    public class ZXingQrcodeHelper
    { 
         //生成中间带图片的二维码
        public static byte[] GetQrBitmap(string msg, string logo, int width = 300, int height = 300)
        {
            try
            {
                Image logoImage = null;
                try
                {
                    logoImage = Image.FromFile(logo);
                }
                catch
                {

                }

                Bitmap image = null;

                if (logoImage == null)
                {
                    //将字符串生成二维码图片
                    image = GetQrBitmap(msg);
                }
                else
                {
                    //将字符串生成中间带图片的二维码图片
                    image = GetQrBitmapWithMiddleImg(msg, logoImage);
                }


                //保存为PNG到内存流
                MemoryStream ms = new MemoryStream();
                image.Save(ms, ImageFormat.Png);
                return ms.GetBuffer();
            }
            catch
            {
                return null;
            }
        }

        //生成普通二维码（不带图片）
        static Bitmap GetQrBitmap(string msg, int width = 300, int height = 300)
        {
            //构造二维码写码器
            QRCodeWriter qrwriter = new QRCodeWriter();
            IDictionary<EncodeHintType, object> hints = new Dictionary<EncodeHintType, object>();
            hints.Add(EncodeHintType.CHARACTER_SET, "UTF-8");
            hints.Add(EncodeHintType.ERROR_CORRECTION, ErrorCorrectionLevel.H);//容错级别
            hints.Add(EncodeHintType.MARGIN, 1);//二维码留白边距

            BarcodeWriter bw = new BarcodeWriter();
            BitMatrix bm = qrwriter.encode(msg, BarcodeFormat.QR_CODE, width, height, hints);
            Bitmap img = bw.Write(bm);

            return img;
        }

        //生成中间带图片的二维码
        static Bitmap GetQrBitmapWithMiddleImg(string msg, Image logoImage, int width = 300, int height = 300)
        {

            Bitmap img = GetQrBitmap(msg);

            //计算插入图片的大小和位置，并计算logo占整个二维码图片的比例
            //根据实际需求可以自定义logo所占比例
            int logoImageW = Math.Min((int)(img.Size.Width / 4), logoImage.Width);
            int logoImageH = Math.Min((int)(img.Size.Height / 4), logoImage.Height);

            int logoImageX = (img.Width - logoImageW) / 2;
            int logoImageY = (img.Height - logoImageH) / 2;

            //将img转换成bmp格式，并创建Graphics对象
            Bitmap bmpimg = new Bitmap(img.Width, img.Height, PixelFormat.Format32bppArgb);

            using (Graphics g = Graphics.FromImage(bmpimg))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.DrawImage(img, 0, 0);
            }

            //在二维码中插入图片
            Graphics myGraphic = Graphics.FromImage(bmpimg);
            //白底
            myGraphic.FillRectangle(Brushes.White, logoImageX, logoImageY, logoImageW, logoImageH);
            myGraphic.DrawImage(logoImage, logoImageX, logoImageY, logoImageW, logoImageH);

            return bmpimg;
        }

    }
}