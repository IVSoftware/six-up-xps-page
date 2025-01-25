using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ZXing;
using static six_up_xps_page.Utilities;

namespace six_up_xps_page
{
    public partial class MainWindow : Window
    {
        public MainWindow() => InitializeComponent();
        private void OnPrintPreview(object sender, RoutedEventArgs e) => 
            new PrintPreviewWindow(DataContext.Items).ShowDialog();
        new MainWindowViewModel DataContext => (MainWindowViewModel)base.DataContext;
    }
    class MainWindowViewModel
    {
        public IList Items { get; } =
            new ObservableCollection<UniqueNumberItem>(
                Enumerable
                .Range(1, 15)
                .Select(_ => new UniqueNumberItem()));
    }
    partial class UniqueNumberItem : ObservableObject
    {
        public ImageSource QRCode
        {
            get
            {
                if (qrCode is null)
                {
                    qrCode = GenerateQRCode(UniqueNumber);
                }
                return qrCode;
            }
        }
        ImageSource? qrCode = default;


        [ObservableProperty]
        string wNR = GenerateName("WNR", 10);

        [ObservableProperty]
        string uniqueNumber = GenerateName("UN", 7);

        [ObservableProperty]
        string matnr = GenerateName("MAT-", 7);

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            switch (e.PropertyName)
            {
                case nameof(UniqueNumber):
                    qrCode = GenerateQRCode(UniqueNumber);
                    OnPropertyChanged(nameof(QRCode));
                    break;
            }
        }
    }
    static class Utilities
    {
        public static string GenerateName(string prefix, int length) =>
            $"{prefix}{
                Guid
                .NewGuid()
                .ToString()
                .ToUpper()
                .Replace("-", string.Empty
                    ).Substring(0,length)}";

        public static ImageSource GenerateQRCode(string qrText)
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
    }
}