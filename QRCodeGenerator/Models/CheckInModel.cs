using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QRCodeGenerator.Models
{
    public class CheckInModel
    {
        public string EmployeeName { get; set; }
        public string EmployeeID { get; set; }
        public string OTP { get; set; }
        public DateTime CheckInTime { get; set; }
    }
}