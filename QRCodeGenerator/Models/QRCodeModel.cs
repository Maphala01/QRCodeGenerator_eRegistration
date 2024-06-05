//using System;

//namespace QRCodeGenerator.Models
//{
//    public class QRCodeModel
//    {
//        public string QRCodeImageUrl { get; set; }

//        public string GetDailyQRCodeContent()
//        {
//            // Generates a unique URL for each day
//            var date = DateTime.UtcNow.Date.ToString("yyyy-MM-dd");
//            var url = $"https://news24.com/?date={date}";
//            return url;
//        }
//    }
//}
using System;
using System.Security.Cryptography;
using System.Text;

namespace QRCodeGenerator.Models
{
    public class QRCodeModel
    {
        public string QRCodeImageUrl { get; set; }
        public string TOTP { get; private set; } // Added TOTP property

        public string GetQRCodeContent()
        {
            // Generates a unique URL with expiration time
            var expirationTime = DateTime.UtcNow.AddSeconds(30); // QR code expires in 30 seconds
            var timestamp = expirationTime.ToString("yyyy-MM-dd-HH-mm-ss");
            TOTP = GenerateTOTP(); // Generate and set the TOTP
            ////var url = $"http://172.26.160.1%3A8077?timestamp={timestamp}&otp={TOTP}";
            var url = $"https://172.20.240.1:44384/Registration/Index";
            return url;
        }

        private string GenerateTOTP()
        {
            using (var hmac = new HMACSHA1(Encoding.UTF8.GetBytes("your-very-secret-key")))
            {
                var timeStep = DateTimeOffset.UtcNow.ToUnixTimeSeconds() / 30;
                var timeStepBytes = BitConverter.GetBytes(timeStep);
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(timeStepBytes);
                }

                var hash = hmac.ComputeHash(timeStepBytes);
                var offset = hash[hash.Length - 1] & 0x0F;
                var binaryCode = (hash[offset] & 0x7F) << 24
                               | (hash[offset + 1] & 0xFF) << 16
                               | (hash[offset + 2] & 0xFF) << 8
                               | (hash[offset + 3] & 0xFF);

                var totp = binaryCode % 100000;
                return totp.ToString("D5");
            }
        }
    }
}


