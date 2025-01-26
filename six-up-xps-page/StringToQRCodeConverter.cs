using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using ZXing;

namespace six_up_xps_page
{
    class StringToQRCodeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string qrText)
            {
                var bitMatrix = new ZXing.QrCode.QRCodeWriter().encode(qrText, BarcodeFormat.QR_CODE, 150, 150);
                int width = bitMatrix.Width;
                int height = bitMatrix.Height;
                int stride = (width + 7) / 8;
                byte[] pixels = new byte[width * height];
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        pixels[y * width + x] = bitMatrix[x, y] ? (byte)0 : (byte)255;
                    }
                }
                BitmapSource bitmapSource = BitmapSource.Create(
                    width,
                    height,
                    96,
                    96,
                    PixelFormats.Gray8,
                    null,
                    pixels,
                    width
                );
                return bitmapSource;
            }
            else return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}
