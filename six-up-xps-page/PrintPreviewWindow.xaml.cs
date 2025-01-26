using Microsoft.Win32;
using System.Collections;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Xps.Packaging;
using System.Windows.Xps;

namespace six_up_xps_page
{
    public partial class PrintPreviewWindow : Window
    {
        public PrintPreviewWindow(IList items)
        {
            InitializeComponent();
            Tiles = [Tile1, Tile2, Tile3, Tile4, Tile5, Tile6];
            Items = items;
            ITEM_COUNT = items.Count;
            PAGE_COUNT = ITEM_COUNT / 6;
            if ((ITEM_COUNT % 6) != 0) PAGE_COUNT++;
            LoadPage();
        }
        ContentPresenter[] Tiles { get; }

        private void LoadPage()
        {
            foreach (var tile in Tiles)
            {
                tile.Content = null;
            }
            int
                currentIndex = PageIndex * PAGE_SIZE,
                endIndex = Math.Min(ITEM_COUNT, currentIndex + PAGE_SIZE);

            while (currentIndex < endIndex)
            {
                var tileIndex = currentIndex % 6;
                Tiles[tileIndex].Content = Items[currentIndex];
                currentIndex++;
            }
        }

        IList Items { get; }
        private readonly int ITEM_COUNT;
        private readonly int PAGE_COUNT;
        const int PAGE_SIZE = 6;

        private void OnPageUpClick(object sender, RoutedEventArgs e) => PageIndex++;

        private void OnPageDownClick(object sender, RoutedEventArgs e) => PageIndex--;
        public int PageIndex
        {
            get => _pageIndex;
            set
            {
                value = Math.Max(value, 0);
                value = Math.Min(value, PAGE_COUNT - 1);
                if (!Equals(_pageIndex, value))
                {
                    _pageIndex = value;
                    LoadPage();
                }
            }
        }
        int _pageIndex = 0;

        private async void OnPrintClick(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "XPS Document (*.xps)|*.xps",
                DefaultExt = "xps",
                FileName = "PrintPreview.xps"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    var parent = (Panel)PreviewGrid.Parent;
                    int childIndex = parent.Children.IndexOf(PreviewGrid);
                    parent.Children.Remove(PreviewGrid);
                    await Dispatcher.InvokeAsync(() =>
                    {
                        FixedDocument document = new FixedDocument();
                        FixedPage page = new FixedPage
                        {
                            Width = 850,
                            Height = 1100
                        };

                        PreviewGrid.Measure(new Size(850, 1100));
                        PreviewGrid.Arrange(new Rect(new Size(850, 1100)));
                        page.Children.Add(PreviewGrid);

                        PageContent pageContent = new PageContent { Child = page };
                        document.Pages.Add(pageContent);

                        using (XpsDocument xpsDoc = new XpsDocument(saveFileDialog.FileName, FileAccess.Write))
                        {
                            XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(xpsDoc);
                            writer.Write(document);
                        }
                        page.Children.Remove(PreviewGrid);
                        parent.Children.Insert(childIndex, PreviewGrid);
                        MessageBox.Show("XPS file saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving XPS file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
