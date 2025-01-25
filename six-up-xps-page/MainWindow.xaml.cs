using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ZXing;

namespace six_up_xps_page
{
    public partial class MainWindow : Window
    {
        public MainWindow() => InitializeComponent();
        private void OnPageUp(object sender, RoutedEventArgs e)
        {

        }

        private void OnPageDown(object sender, RoutedEventArgs e)
        {

        }
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

        private static string GenerateName(string prefix, int length) =>
            $"{prefix}{
                Guid
                .NewGuid()
                .ToString()
                .ToUpper()
                .Replace("-", string.Empty
                    ).Substring(0,length)}";
        
        private static ImageSource GenerateQRCode(string qrText)
        {
            var writer = new BarcodeWriter<WriteableBitmap>
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new ZXing.Common.EncodingOptions
                {
                    Height = 150,
                    Width = 150,
                    Margin = 0
                },
            };
            WriteableBitmap qrCodeBitmap = writer.Write(qrText);
            return qrCodeBitmap;
        }
    }
}