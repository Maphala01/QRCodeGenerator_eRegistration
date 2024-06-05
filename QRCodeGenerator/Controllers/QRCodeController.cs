//using System;
//using System.Drawing;
//using System.Drawing.Imaging;
//using System.IO;
//using System.Web.Mvc;
//using QRCoder;
//using QRCodeGenerator.Models;

//namespace QRCodeGenerator.Controllers
//{
//    public class QRCodeController : Controller
//    {
//        public ActionResult Index()
//        {
//            var model = new QRCodeModel();
//            var qrCodeContent = model.GetDailyQRCodeContent();

//            using (var qrGenerator = new QRCoder.QRCodeGenerator())
//            using (var qrCodeData = qrGenerator.CreateQrCode(qrCodeContent, QRCoder.QRCodeGenerator.ECCLevel.Q))
//            using (var qrCode = new QRCoder.QRCode(qrCodeData))
//            using (var bitmap = qrCode.GetGraphic(20))
//            {
//                // Load the logo image
//                var logoPath = Server.MapPath("~/Content/download.jpg");
//                using (var logo = Image.FromFile(logoPath))
//                {
//                    var combinedImage = AddLogoToQRCode(bitmap, logo);

//                    using (var stream = new MemoryStream())
//                    {
//                        combinedImage.Save(stream, ImageFormat.Png);
//                        var qrCodeBase64 = Convert.ToBase64String(stream.ToArray());
//                        model.QRCodeImageUrl = $"data:image/png;base64,{qrCodeBase64}";
//                    }
//                }
//            }

//            return View(model);
//        }

//        private Bitmap AddLogoToQRCode(Bitmap qrCodeImage, Image logo)
//        {
//            int logoSize = qrCodeImage.Width / 5; // Adjust size as needed
//            var logoPosition = new Point((qrCodeImage.Width - logoSize) / 2, (qrCodeImage.Height - logoSize) / 2);

//            var combinedImage = new Bitmap(qrCodeImage.Width, qrCodeImage.Height, PixelFormat.Format32bppArgb);
//            using (var graphics = Graphics.FromImage(combinedImage))
//            {
//                graphics.DrawImage(qrCodeImage, new Point(0, 0));
//                graphics.DrawImage(logo, new Rectangle(logoPosition, new Size(logoSize, logoSize)));
//            }

//            return combinedImage;
//        }
//    }
//}

//using System;
//using System.Drawing;
//using System.Drawing.Imaging;
//using System.IO;
//using System.Web.Mvc;
//using QRCoder; // Ensure QRCoder is referenced
//using QRCodeGenerator.Models; // Ensure this namespace is correct and needed

//namespace QRCodeGenerator.Controllers
//{
//    public class QRCodeController : Controller
//    {
//        public ActionResult Index()
//        {
//            var model = new QRCodeModel();
//            var qrCodeContent = model.GetQRCodeContent();

//            // Check if the QR code is expired
//            var expirationTimestamp = Request.QueryString["timestamp"];
//            if (!string.IsNullOrEmpty(expirationTimestamp))
//            {
//                var expirationTime = DateTime.ParseExact(expirationTimestamp, "yyyy-MM-dd-HH-mm-ss", null);
//                if (DateTime.UtcNow > expirationTime)
//                {
//                    // QR code expired, return error view or handle as needed
//                    ViewBag.QRCodeExpired = true;
//                    return View(model);
//                }
//            }

//            using (var qrGenerator = new QRCodeGenerator())
//            using (var qrCodeData = qrGenerator.CreateQrCode(qrCodeContent, QRCoder.QRCodeGenerator.ECCLevel.Q)) // Ensure QRCoder.QRCodeGenerator.ECCLevel is correct
//            using (var qrCode = new QRCode(qrCodeData)) // Ensure QRCode is properly recognized
//            using (var bitmap = qrCode.GetGraphic(20))
//            {
//                // Load the logo image
//                var logoPath = Server.MapPath("~/Content/download.jpg");
//                using (var logo = Image.FromFile(logoPath))
//                {
//                    var combinedImage = AddLogoToQRCode(bitmap, logo);

//                    using (var stream = new MemoryStream())
//                    {
//                        combinedImage.Save(stream, ImageFormat.Png);
//                        var qrCodeBase64 = Convert.ToBase64String(stream.ToArray());
//                        model.QRCodeImageUrl = $"data:image/png;base64,{qrCodeBase64}";
//                    }
//                }
//            }

//            return View(model);
//        }

//        private Bitmap AddLogoToQRCode(Bitmap qrCodeImage, Image logo)
//        {
//            int logoSize = qrCodeImage.Width / 5; // Adjust size as needed
//            var logoPosition = new Point((qrCodeImage.Width - logoSize) / 2, (qrCodeImage.Height - logoSize) / 2);

//            var combinedImage = new Bitmap(qrCodeImage.Width, qrCodeImage.Height, PixelFormat.Format32bppArgb);
//            using (var graphics = Graphics.FromImage(combinedImage))
//            {
//                graphics.DrawImage(qrCodeImage, new Point(0, 0));
//                graphics.DrawImage(logo, new Rectangle(logoPosition, new Size(logoSize, logoSize)));
//            }

//            return combinedImage;
//        }
//    }
//}
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web.Mvc;
using QRCoder;
using QRCodeGenerator.Models;

namespace QRCodeGenerator.Controllers
{
    public class QRCodeController : Controller
    {
        public ActionResult Index()
        {
            var model = new QRCodeModel();
            var qrCodeContent = model.GetQRCodeContent();

            // Check if the QR code is expired or TOTP is invalid
            var expirationTimestamp = Request.QueryString["timestamp"];
            var otp = Request.QueryString["otp"];
            if (!string.IsNullOrEmpty(expirationTimestamp) && !string.IsNullOrEmpty(otp))
            {
                var expirationTime = DateTime.ParseExact(expirationTimestamp, "yyyy-MM-dd-HH-mm-ss", null);
                if (DateTime.UtcNow > expirationTime || otp != model.TOTP)
                {
                    // QR code expired or invalid OTP, return error view or handle as needed
                    ViewBag.QRCodeExpired = true;
                    return View(model);
                }
            }

            using (var qrGenerator = new QRCoder.QRCodeGenerator()) // Corrected the usage of QRCodeGenerator here
            using (var qrCodeData = qrGenerator.CreateQrCode(qrCodeContent, QRCoder.QRCodeGenerator.ECCLevel.Q))
            using (var qrCode = new QRCoder.QRCode(qrCodeData))
            using (var bitmap = qrCode.GetGraphic(20))
            {
                // Load the logo image
                var logoPath = Server.MapPath("~/Content/download.jpg");
                using (var logo = Image.FromFile(logoPath))
                {
                    var combinedImage = AddLogoToQRCode(bitmap, logo);

                    using (var stream = new MemoryStream())
                    {
                        combinedImage.Save(stream, ImageFormat.Png);
                        var qrCodeBase64 = Convert.ToBase64String(stream.ToArray());
                        model.QRCodeImageUrl = $"data:image/png;base64,{qrCodeBase64}";
                    }
                }
            }

            return View(model);
        }

        private Bitmap AddLogoToQRCode(Bitmap qrCodeImage, Image logo)
        {
            int logoSize = qrCodeImage.Width / 5; // Adjust size as needed
            var logoPosition = new Point((qrCodeImage.Width - logoSize) / 2, (qrCodeImage.Height - logoSize) / 2);

            var combinedImage = new Bitmap(qrCodeImage.Width, qrCodeImage.Height, PixelFormat.Format32bppArgb);
            using (var graphics = Graphics.FromImage(combinedImage))
            {
                graphics.DrawImage(qrCodeImage, new Point(0, 0));
                graphics.DrawImage(logo, new Rectangle(logoPosition, new Size(logoSize, logoSize)));
            }

            return combinedImage;
        }

    
        
    }
}

