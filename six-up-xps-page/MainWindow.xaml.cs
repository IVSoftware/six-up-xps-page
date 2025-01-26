using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows;

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
        // THE SOURCE LIST - With 15 items initially populated for test.
        public IList Items { get; } =
            new ObservableCollection<UniqueNumberItem>(
                Enumerable
                .Range(1, 15)
                .Select(_ => new UniqueNumberItem
                {
                    WNR = GenerateName("WNR", 10),
                    UniqueNumber = GenerateName("UN", 7),
                    Matnr = GenerateName("MAT-", 7),
                }));

        // Utility for test name generation
        private static string GenerateName(string prefix, int length) =>
            $"{prefix}{Guid.NewGuid().ToString().Replace("-", string.Empty
                    ).Substring(0,length).ToUpper()}";
    }
    partial class UniqueNumberItem : ObservableObject
    {
        [ObservableProperty]
        string wNR = string.Empty; 

        [ObservableProperty]
        string uniqueNumber = string.Empty;

        [ObservableProperty]
        string matnr = string.Empty;
    }
}