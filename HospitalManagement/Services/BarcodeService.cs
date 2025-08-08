using System.Drawing;
using System.Drawing.Imaging;
using ZXing;
using ZXing.Common;

public class BarcodeService
{
    private readonly string _barcodeFolder;
    private readonly IWebHostEnvironment _env;

    public BarcodeService(IWebHostEnvironment env)
    {
        _env = env;
        _barcodeFolder = Path.Combine(_env.WebRootPath, "barcodes");

        if (!Directory.Exists(_barcodeFolder))
            Directory.CreateDirectory(_barcodeFolder);
    }

    public async Task<string> GenerateAndSaveBarcodeAsync(string barcodeValue, string fileName)
    {
        var writer = new BarcodeWriterPixelData
        {
            Format = BarcodeFormat.CODE_128,
            Options = new EncodingOptions
            {
                Height = 80,
                Width = 300,
                Margin = 10
            }
        };

        var pixelData = writer.Write(barcodeValue);

        using var barcodeBitmap = new Bitmap(pixelData.Width, pixelData.Height, PixelFormat.Format32bppRgb);
        var bitmapData = barcodeBitmap.LockBits(new Rectangle(0, 0, pixelData.Width, pixelData.Height),
            ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);
        System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
        barcodeBitmap.UnlockBits(bitmapData);

        int labelHeight = 30;
        int totalHeight = barcodeBitmap.Height + labelHeight;
        using var finalBitmap = new Bitmap(barcodeBitmap.Width, totalHeight);

        using var graphics = Graphics.FromImage(finalBitmap);
        graphics.Clear(Color.White);

        graphics.DrawImage(barcodeBitmap, 0, 0);

        using var font = new Font("Arial", 14, FontStyle.Regular);
        var textSize = graphics.MeasureString(barcodeValue, font);
        float xCenter = (finalBitmap.Width - textSize.Width) / 2;
        float yText = barcodeBitmap.Height + 5;

        using var brush = new SolidBrush(Color.Black);
        graphics.DrawString(barcodeValue, font, brush, xCenter, yText);

        var fullPath = Path.Combine(_barcodeFolder, fileName);
        await Task.Run(() => finalBitmap.Save(fullPath, ImageFormat.Png));

        return $"/barcodes/{fileName}";
    }
}
