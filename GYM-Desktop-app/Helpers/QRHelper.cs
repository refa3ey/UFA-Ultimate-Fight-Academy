using System;
using System.Drawing;
using QRCoder;

namespace GYM_Desktop_app.Helpers
{
    public static class QRHelper
    {
        public static Bitmap GenerateQR(string content, int pixelsPerModule = 10)
        {
            using (var gen  = new QRCodeGenerator())
            using (var data = gen.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q))
            {
                var qr = new QRCode(data);
                return qr.GetGraphic(pixelsPerModule);
            }
        }

        public static Bitmap GenerateMemberQR(int memberID, int pixelsPerModule = 10)
            => GenerateQR($"MBR-{memberID}", pixelsPerModule);

        public static string ParseQRContent(string scanned)
        {
            if (string.IsNullOrWhiteSpace(scanned)) return scanned;
            if (scanned.StartsWith("MBR-", StringComparison.OrdinalIgnoreCase))
                return scanned.Substring(4);
            return scanned;
        }
    }
}
