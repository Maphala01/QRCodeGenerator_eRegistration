using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using QRCodeGenerator.Models;

namespace QRCodeGenerator.Controllers
{
    public class CheckInController : Controller
    {
        public ActionResult Index(string timestamp, string otp)
        {
            var model = new CheckInModel();
            ViewBag.Timestamp = timestamp;
            ViewBag.OTP = otp;
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(CheckInModel model, string timestamp, string otp)
        {
            if (ModelState.IsValid)
            {
                if (ValidateOTP(otp, timestamp))
                {
                    SaveCheckInData(model);
                    ViewBag.Message = "Check-in successful!";
                }
                else
                {
                    ViewBag.Message = "Invalid or expired OTP.";
                }
            }
            return View(model);
        }

        private bool ValidateOTP(string otp, string timestamp)
        {
            DateTime expirationTime = DateTime.ParseExact(timestamp, "yyyy-MM-dd-HH-mm-ss", null);
            return otp == GenerateExpectedOTP(expirationTime);
        }

        private string GenerateExpectedOTP(DateTime timestamp)
        {
            using (var hmac = new HMACSHA1(Encoding.UTF8.GetBytes("your-very-secret-key")))
            {
                var timeStep = new DateTimeOffset(timestamp).ToUnixTimeSeconds() / 30;
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

        private void SaveCheckInData(CheckInModel model)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["YourConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO CheckInData (EmployeeName, EmployeeID, OTP, CheckInTime) VALUES (@EmployeeName, @EmployeeID, @OTP, @CheckInTime)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@EmployeeName", model.EmployeeName);
                command.Parameters.AddWithValue("@EmployeeID", model.EmployeeID);
                command.Parameters.AddWithValue("@OTP", model.OTP);
                command.Parameters.AddWithValue("@CheckInTime", DateTime.UtcNow);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}
