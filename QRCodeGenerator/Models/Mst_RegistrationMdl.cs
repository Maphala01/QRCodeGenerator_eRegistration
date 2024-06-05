using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QRCodeGenerator.Models
{
    public class Mst_RegistrationMdl
    {
        public string qrCodeImgUrl { get; set; }
        public int QRCodeTotp { get; set; }
        public string empName { get; set; }
        public string empDepartment { get; set; }
    }
}