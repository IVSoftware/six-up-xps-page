using System.Collections;
using System.Windows;
using System.Windows.Controls;

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
            if((ITEM_COUNT % 6) != 0) PAGE_COUNT++;
            LoadPage(_pageIndex);
        }
        ContentPresenter[] Tiles { get; }

        private void LoadPage(int pageIndex)
        {
            int
                currentIndex = _pageIndex * PAGE_SIZE,
                endIndex = Math.Min(ITEM_COUNT, currentIndex + PAGE_SIZE);

            while(currentIndex<endIndex)
            {
                var tileIndex = currentIndex % 6;
                Tiles[tileIndex].Content = Items[currentIndex];
                currentIndex++;
            }
        }

        IList Items { get; }
        private int _pageIndex = 0;
        private readonly int ITEM_COUNT;
        private readonly int PAGE_COUNT;
        const int PAGE_SIZE = 6;

        private void OnPrintClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
