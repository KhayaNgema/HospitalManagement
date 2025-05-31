
using QRCoder;
using System;

namespace HospitalManagement.Services
{
    public class QrCodeService
    {
        public byte[] GenerateQrCode(string data)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new PngByteQRCode(qrCodeData);
            return qrCode.GetGraphic(20);
        }
    }
}

