using Microsoft.Win32;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Grafika_Komputerowa.Pages
{
    /// <summary>
    /// Logika interakcji dla klasy HistogramPage.xaml
    /// </summary>
    public partial class HistogramPage : Page
    {
        private BitmapImage bitmapImage;
        private byte[] Pixels;
        private byte[] BasePixels;
        private int ImageWidth;
        private int ImageHeight;
        private double[,] Histogram;
        private int Value;

        public HistogramPage()
        {
            InitializeComponent();
        }
        private void LoadFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Plik JPEG |*.jpg";
            if (openFileDialog.ShowDialog() == true)
            {
                using (var file = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read))
                {
                    bitmapImage = new BitmapImage(new Uri(openFileDialog.FileName));
                    ImageWidth = bitmapImage.PixelWidth;
                    ImageHeight = bitmapImage.PixelHeight;

                    Pixels = new byte[ImageWidth * ImageHeight];
                    BasePixels = new byte[ImageWidth * ImageHeight];
                    bitmapImage.CopyPixels(Pixels, ImageWidth, 0);
                    Pixels.CopyTo(BasePixels, 0);

                    CreateHistogram();
                    image.Source = bitmapImage;

                    T1.IsEnabled = true;
                    B1.IsEnabled = true;
                }
            }
        }
        private void CreateHistogram()
        {
            Histogram = new double[256, 3];

            // ilość wystąpień danego pixela
            for (int i = 0; i < Pixels.Length; i++)
                Histogram[(int)Pixels[i], 0]++;

            // częstotliwość wystąpień danego pixela
            for (int i = 0; i < 256; i++)
                Histogram[i, 1] = Histogram[i, 0] / Pixels.Length;

            // suma czestotliwości wystąpień
            Histogram[0, 2] = Histogram[0, 1];
            for (int i = 1; i < 256; i++)
                Histogram[i, 2] = Histogram[i - 1, 2] + Histogram[i, 1];

            FillHistogramCanvas();
        }

        private void AlignHistogramClick(object sender, RoutedEventArgs e)
        {
            int Value = Convert.ToInt32(T1.Text);
            for (int i = 0; i < Pixels.Length; i++) Pixels[i] = (byte)(Histogram[BasePixels[i], 2] * Value);
            image.Source = BitmapSource.Create(ImageWidth, ImageHeight, 96, 96, PixelFormats.Gray8, null, Pixels, ImageWidth);
            CreateHistogram();
        }
        private void ExtendedHistogramClick(object sender, RoutedEventArgs e)
        {
            byte[] Pixels2 = new byte[Pixels.Length];
            int max = 0, min = 255;
            for (int i = 0; i < Pixels.Length; i++)
            {
                if (Pixels[i] > max) max = Pixels[i];
                if (Pixels[i] < min) min = Pixels[i];
            }

            for (int i = 0; i < Pixels.Length; i++)
                Pixels2[i] = max != min ? (byte)(255 * (Pixels[i] - min) / (max - min)) : (byte)0;

            Pixels = Pixels2;
            CreateHistogram();

            image.Source = BitmapSource.Create(ImageWidth, ImageHeight, 96, 96, PixelFormats.Gray8, null, Pixels2, ImageWidth);
        }
        private void FillHistogramCanvas()
        {
            C1.Children.Clear();
            double max = 0;
            for (int i = 0; i < 256; i++) if (max < Histogram[i, 0]) max = Histogram[i, 0];
            double div = max / 100;
            for (int i = 0; i < 256; i++)
            {
                Rectangle rect = new Rectangle()
                {
                    Width = 1,
                    Fill = Brushes.Gray,
                    Height = Histogram[i, 0] / div
                };
                C1.Children.Add(rect);
                Canvas.SetLeft(rect,i);
                Canvas.SetBottom(rect,0);
            }
        }
        
        private void BinaryClick(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < Pixels.Length; i++) Pixels[i] = Pixels[i] > (byte)Value ? (byte)255 : (byte)0;
            CreateHistogram();
            image.Source = BitmapSource.Create(ImageWidth, ImageHeight, 96, 96, PixelFormats.Gray8, null, Pixels, ImageWidth);
            BasePixels.CopyTo(Pixels, 0);
        }
        private void PercentBlackSelectionClick(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            BasePixels.CopyTo(Pixels, 0);
            CreateHistogram();

            double _value = S1.Value;
            double Sum = 0;
            int index;
            for (index = 0; index < 256; index++)
            {
                Sum += Histogram[index, 0];
                if (Sum / Pixels.Length >= _value / 100) break; 
            }

            for (int i = 0; i < Pixels.Length; i++) Pixels[i] = Pixels[i] > (byte)index ? (byte)255 : (byte)0;
            image.Source = BitmapSource.Create(ImageWidth, ImageHeight, 96, 96, PixelFormats.Gray8, null, Pixels, ImageWidth);
        }
        private void MeanIterativeSelectionClick(object sender, RoutedEventArgs e)
        {
            BasePixels.CopyTo(Pixels, 0);
            CreateHistogram();
            int TOB = 0, TB = 0, T1, T2 = -1;
            int[] corrners = new int[] { 0, ImageWidth - 1, ImageHeight * (ImageWidth - 1), ImageWidth * ImageHeight - 1 };
            foreach (int corrner in corrners) TB += Pixels[corrner];
            for (int i = 0; i < Pixels.Length; i++) TOB += Pixels[i];
            T1 = ((TOB - TB) / (Pixels.Length - 4) + TB / 4) / 2;
            while (T1 != T2)
            {
                T2 = T1;
                int TOBC = 0, TBC = 0, TOBS = 0, TBS = 0;
                for (int i = 0; i < Pixels.Length; i++)
                {
                    if (Pixels[i] < T1) { TBS += Pixels[i]; TBC++; }
                    else { TOBS += Pixels[i]; TOBC++; }
                }
                T1 = (TBS / TBC + TOBS / TOBC) / 2;
            }

            for (int i = 0; i < Pixels.Length; i++) Pixels[i] = Pixels[i] > (byte)T1 ? (byte)255 : (byte)0;
            image.Source = BitmapSource.Create(ImageWidth, ImageHeight, 96, 96, PixelFormats.Gray8, null, Pixels, ImageWidth);
        }

        private void PrevTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox TB = (TextBox)sender;
            if (!IsNumeric(TB.Text + e.Text)) e.Handled = false;
            else { e.Handled = true; TB.Text = "0"; TB.Focus(); }
        }
        private void LostFocusTextBox(object sender, RoutedEventArgs e)
        {
            Value = Convert.ToInt32(T1.Text);
        }
        private bool IsNumeric(string text)
        {
            Regex reg = new Regex("[^0-9]");
            return reg.IsMatch(text);
        }
        private void RestoreClick(object sender, RoutedEventArgs e)
        {
            BasePixels.CopyTo(Pixels,0);
            image.Source = BitmapSource.Create(ImageWidth, ImageHeight, 96, 96, PixelFormats.Gray8, null, BasePixels, ImageWidth);
            CreateHistogram();
        }
    }
}
