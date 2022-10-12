using Grafika_Komputerowa.Pages;
using System.Windows;

namespace Grafika_Komputerowa
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static PaintPage paintPage = new PaintPage();
        public static PPMPage ppmtPage = new PPMPage();
        public static RGBPage rgbPage = new RGBPage();
        public static FiltrPage filtrPage = new FiltrPage();
        public static HistogramPage histogramPage = new HistogramPage();
        public static BézierPage bézierPage = new BézierPage();
        public static ShapePage shapePage = new ShapePage();
        public static MorfologiaPage morfologiaPage = new MorfologiaPage();
        public static AnalizaPage analizaPage = new AnalizaPage();

        public MainWindow()
        {
            InitializeComponent();
            frame.Content = analizaPage;
        }

        private void PaintPageClick(object sender, RoutedEventArgs e)
        {
            frame.Content = paintPage;
        }

        private void PPMPageClick(object sender, RoutedEventArgs e)
        {
            frame.Content = ppmtPage;
        }

        private void RGBPageClick(object sender, RoutedEventArgs e)
        {
            frame.Content = rgbPage;
        }

        private void FiltrPageClick(object sender, RoutedEventArgs e)
        {
            frame.Content = filtrPage;
        }

        private void HistogramPageClick(object sender, RoutedEventArgs e)
        {
            frame.Content = histogramPage;
        }

        private void BézierPageClick(object sender, RoutedEventArgs e)
        {
            frame.Content = bézierPage;
        }
        private void ShapePageClick(object sender, RoutedEventArgs e)
        {
            frame.Content = shapePage;
        }

        private void MorfologiaPageClick(object sender, RoutedEventArgs e)
        {
            frame.Content = morfologiaPage;
        }

        private void AnalizaPageClick(object sender, RoutedEventArgs e)
        {
            frame.Content = analizaPage;
        }
    }
}
