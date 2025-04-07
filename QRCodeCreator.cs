using System;
using System.IO;
using System.Text;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace LCSIQRCodePIX
{
    [ComVisible(true)]
    [Guid("D4A7E5A6-FA5C-4F2C-85C5-B2B563A9F903")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IQRCodeCreator
    {
        string GenerateQRCode(string chavePix, string nomeRecebedor, string cidadeRecebedor, string valor, string txid, string filePath, string pointOfInitiation);
        string GeneratePixPayload(string chavePix, string nomeRecebedor, string cidadeRecebedor, string valor, string txid, string pointOfInitiation);
    }

    [ComVisible(true)]
    [Guid("A1B5A7C7-3D4E-4F5D-943E-8B7D4F5A7C8D")]
    [ClassInterface(ClassInterfaceType.None)]
    public class QRCodeCreator : IQRCodeCreator
    {
        public string GenerateQRCode(string chavePix, string nomeRecebedor, string cidadeRecebedor, string valor, string txid, string filePath, string pointOfInitiation)
        {
            try
            {
                string payload = GeneratePixPayload(chavePix, nomeRecebedor, cidadeRecebedor, valor, txid, pointOfInitiation);
                GenerateQRCodeImage(payload, filePath);
                return "QR Code LCSI gerado com sucesso!";
            }
            catch (Exception ex)
            {
                return $"Erro ao gerar o QR Code LCSI: {ex.Message}";
            }
        }

        public string GeneratePixPayload(string chavePix, string nomeRecebedor, string cidadeRecebedor, string valor, string txid, string pointOfInitiation)
        {
            // Validação do parâmetro pointOfInitiation
            if (pointOfInitiation != "11" && pointOfInitiation != "12")
            {
                throw new ArgumentException("O valor de pointOfInitiation deve ser '11' ou '12'.");
            }

            StringBuilder sb = new StringBuilder();
            AppendField(sb, "00", "01");
            AppendField(sb, "01", pointOfInitiation);

            StringBuilder pixInfo = new StringBuilder();
            AppendField(pixInfo, "00", "BR.GOV.BCB.PIX");
            AppendField(pixInfo, "01", chavePix);
            AppendField(sb, "26", pixInfo.ToString());

            AppendField(sb, "52", "0000");
            AppendField(sb, "53", "986");
            if (!string.IsNullOrEmpty(valor)) AppendField(sb, "54", valor);
            AppendField(sb, "58", "BR");
            AppendField(sb, "59", nomeRecebedor);
            AppendField(sb, "60", cidadeRecebedor);


            StringBuilder additionalInfo = new StringBuilder();
            if (!string.IsNullOrEmpty(txid)) AppendField(additionalInfo, "05", txid);
            if (additionalInfo.Length > 0) AppendField(sb, "62", additionalInfo.ToString());


            string baseStr = sb.ToString() + "6304";
            string crc = CalculateCRC16(baseStr);
            return baseStr + crc;
        }

        private void AppendField(StringBuilder sb, string id, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                sb.Append(id);
                sb.Append(value.Length.ToString("D2"));
                sb.Append(value);
            }
        }

        private void GenerateQRCodeImage(string payload, string filePath)
        {

            var writer = new BarcodeWriterPixelData
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new QrCodeEncodingOptions
                {
                    Width = 500,
                    Height = 500,
                    Margin = 2,
                    ErrorCorrection = ZXing.QrCode.Internal.ErrorCorrectionLevel.M,
                    CharacterSet = "UTF-8"
                }
            };

            var pixelData = writer.Write(payload);
            using (Bitmap bitmap = new Bitmap(pixelData.Width, pixelData.Height, PixelFormat.Format32bppArgb))
            {
                var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                    ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                try
                {
                    Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
                }
                finally
                {
                    bitmap.UnlockBits(bitmapData);
                }
                bitmap.Save(filePath, ImageFormat.Png);
            }
        }

        private string CalculateCRC16(string data)
        {
            int crc = 0xFFFF;
            int polinomio = 0x1021;
            byte[] dados = Encoding.UTF8.GetBytes(data);

            foreach (byte b in dados)
            {
                crc ^= (b << 8);
                for (int i = 0; i < 8; i++)
                {
                    crc = ((crc & 0x8000) != 0) ? (crc << 1) ^ polinomio : (crc << 1);
                    crc &= 0xFFFF;
                }
            }
            return crc.ToString("X4").ToUpper();
        }
    }
}