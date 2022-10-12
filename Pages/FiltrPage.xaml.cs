using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Grafika_Komputerowa.Pages
{
    /// <summary>
    /// Logika interakcji dla klasy FiltrPage.xaml
    /// </summary>
    public partial class FiltrPage : Page
    {
        private BitmapImage bitmapImage;
        private byte[] Pixels;
        private byte[,] Pixels2D;
        private double[] BasePixels;
        private byte AverageR;
        private byte AverageG;
        private byte AverageB;
        private int Stride;
        private int ImageWidth;
        private int ImageHeight;
        private int ImageWidth2D;
        private int ImageHeight2D;
        private int Mask = 1;

        public FiltrPage()
        {
            InitializeComponent();
        }
        private void LoadFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Plik PNG |*.PNG| XD |*.jpg" ;
            if(openFileDialog.ShowDialog() == true)
            {
                using (var file = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read))
                {
                    bitmapImage = new BitmapImage(new Uri(openFileDialog.FileName));
                    Stride = bitmapImage.PixelWidth * 4;
                    Pixels = new byte[Stride * bitmapImage.PixelHeight];
                    bitmapImage.CopyPixels(Pixels, Stride, 0);
                    image.Source = bitmapImage;
                    ImageWidth = bitmapImage.PixelWidth;
                    ImageHeight = bitmapImage.PixelHeight;
                    BasePixels = new double[Stride * bitmapImage.PixelHeight];
                    SetBasePixels();
                    Pixels2D = GetExtendedPixels();

                    FindAverage();
                    SR.IsEnabled = true;
                    SG.IsEnabled = true;
                    SB.IsEnabled = true;
                    SV.IsEnabled = true;
                    SM.IsEnabled = true;
                    BGA.IsEnabled = true;
                    BGH.IsEnabled = true;
                    B1.IsEnabled = true;
                    B2.IsEnabled = true;
                    B3.IsEnabled = true;
                    B4.IsEnabled = true;
                    B5.IsEnabled = true;
                    B6.IsEnabled = true;
                    SR.Value = AverageR;
                    SG.Value = AverageG;
                    SB.Value = AverageB;
                    SV.Value = 20;
                }
            }
        }
        private void FindAverage()
        {
            AverageR = 0;
            AverageG = 0;
            AverageB = 0;

            double[] ColorChannels = new double[] { 0, 0, 0, 0 };

            for (int x = 0; x < Pixels.Length; x++)
                ColorChannels[x % 4] += Pixels[x];

            AverageR = (byte)(ColorChannels[2] * 4 / Pixels.Length);
            AverageG = (byte)(ColorChannels[1] * 4 / Pixels.Length);
            AverageB = (byte)(ColorChannels[0] * 4 / Pixels.Length);
        }
        private void SetBasePixels()
        {
            for (int i = 0; i < Pixels.Length; i++) BasePixels[i] = Pixels[i];
        }
        private void LoadImage()
        {
            image.Source = BitmapSource.Create(ImageWidth, ImageHeight, 96, 96, bitmapImage.Format, null, Pixels, Stride);
        }
        private void CheckTextValue(object sender, TextCompositionEventArgs e)
        {
            TextBox TB = (TextBox)sender;
            if (!IsNumeric(TB.Text + e.Text)) e.Handled = false;
            else TB.Text = "0";
        }
        private bool IsNumeric(string text)
        {
            Regex reg = new Regex("[^0-9]");
            return reg.IsMatch(text);
        }
        private void RestoreOriginal(object sender, RoutedEventArgs e)
        {
            ImageWidth = bitmapImage.PixelWidth;
            ImageHeight = bitmapImage.PixelHeight;
            Stride = bitmapImage.PixelWidth * 4;
            Pixels = new byte[ImageWidth * 4 * ImageHeight];
            for (int i = 0; i < Pixels.Length; i++) Pixels[i] = (byte)BasePixels[i];
            Pixels2D = GetExtendedPixels();
            LoadImage();

            SR.Value = AverageR;
            SG.Value = AverageG;
            SB.Value = AverageB;
            SV.Value = 20;
        }
        private byte[,] GetExtendedPixels()
        {
            ImageWidth2D = bitmapImage.PixelWidth + 2 * Mask;
            ImageHeight2D = bitmapImage.PixelHeight + 2 * Mask;
            int Stride2D = ImageWidth2D * 4;
            byte[,] ExtendedPixels = new byte[Stride2D, ImageHeight2D];
            int index = 0;

            for (int y = Mask; y < ImageHeight2D - Mask; y++)
                for (int x = Mask * 4; x < ImageWidth2D * 4 - Mask * 4; x++)
                    ExtendedPixels[x,y] = Pixels[index++];

            for (int y = 0; y < Mask; y++)
                for (int x = 0; x < bitmapImage.PixelWidth * 4; x++)
                    ExtendedPixels[x + Mask * 4, y] = Pixels[x];

            for (int y = ImageHeight2D - 1; y > ImageHeight2D - 1 - Mask; y--)
                for (int x = 0; x < bitmapImage.PixelWidth * 4; x++)
                    ExtendedPixels[x + Mask * 4, y] = Pixels[(bitmapImage.PixelHeight - 1) * bitmapImage.PixelWidth * 4 + x];

            for (int x = 0; x < Mask * 4; x++)
                for (int y = 0; y < ImageHeight2D; y++)
                    ExtendedPixels[x, y] = ExtendedPixels[x%4 + Mask * 4, y];

            for (int x = (ImageWidth2D - Mask) * 4; x < ImageWidth2D * 4; x++)
                for (int y = 0; y < ImageHeight2D; y++)
                    ExtendedPixels[x, y] = ExtendedPixels[(ImageWidth2D - Mask - 1) * 4 + x % 4, y];

            return ExtendedPixels;
        }
        private byte[] GetPixelArray()
        {
            byte[] PixelArray = new byte[ImageWidth * 4 * ImageHeight];
            int index = 0;

            for (int y = Mask; y < ImageHeight + Mask; y++)
                for (int x = Mask * 4; x < (ImageWidth + Mask)* 4; x++)
                    PixelArray[index++] = Pixels2D[x,y];

            return PixelArray;
        }
        
        
        // funkcje arytmetyczne

        private void SliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                double[] Procent = new double[] { 0, 0, 0, 1 };
                Procent[2] = SR.Value >= AverageR ? (SR.Value - AverageR) / (255 - AverageR) : -1 * ((AverageR - SR.Value) / AverageR);
                Procent[1] = SG.Value >= AverageG ? (SG.Value - AverageG) / (255 - AverageG) : -1 * ((AverageG - SG.Value) / AverageG);
                Procent[0] = SB.Value >= AverageB ? (SB.Value - AverageB) / (255 - AverageB) : -1 * ((AverageB - SB.Value) / AverageB);
                for (int x = 0; x < Pixels.Length; x++)
                    Pixels[x] = Procent[x % 4] >= 0 ? (byte)(BasePixels[x] + Procent[x % 4] * (255 - BasePixels[x])): (byte)(BasePixels[x] + Procent[x % 4] * BasePixels[x]);
                LoadImage();
            }
        }
        private void SliderBrightnessChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                double Procent = (SV.Value - 20) / 20;
                SR.Value = Procent >= 0 ? (int)(AverageR + Procent * (255 - AverageR)) : (int)(AverageR + Procent * AverageR);
                SG.Value = Procent >= 0 ? (int)(AverageG + Procent * (255 - AverageG)) : (int)(AverageG + Procent * AverageG);
                SB.Value = Procent >= 0 ? (int)(AverageB + Procent * (255 - AverageB)) : (int)(AverageB + Procent * AverageB);
                for (int x = 0; x < Pixels.Length; x++)
                    Pixels[x] = Procent >= 0 ? (byte)(BasePixels[x] + Procent * (255 - BasePixels[x])): (byte)(BasePixels[x] + Procent * BasePixels[x]);
                LoadImage();
            }
        }
        private void SliderMaskChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Mask = (int)(SM.Value + 1);
            Pixels2D = GetExtendedPixels();
        }
        private void GreyScale_Average(object sender, RoutedEventArgs e)
        {
            for (int x = 0; x < Pixels.Length; x += 4)
            {
                double Average = (Pixels[x] + Pixels[x + 1] + Pixels[x + 2]) / 3;
                Pixels[x] = (byte)Average;
                Pixels[x + 1] = (byte)Average;
                Pixels[x + 2] = (byte)Average;
            }
            Pixels2D = GetExtendedPixels();
            LoadImage();
        }
        private void GreyScale_HumanEye(object sender, RoutedEventArgs e)
        {
            for (int x = 0; x < Pixels.Length; x += 4)
            {
                double Average = Pixels[x] * 0.11 + Pixels[x + 1] * 0.59 + Pixels[x + 2] * 0.3;
                Pixels[x] = (byte)Average;
                Pixels[x + 1] = (byte)Average;
                Pixels[x + 2] = (byte)Average;
            }
            Pixels2D = GetExtendedPixels();
            LoadImage();
        }

        // filtry

        private void Smoothing_Average(object sender, RoutedEventArgs e)
        {
            double Sum;
            for (int y = Mask; y < ImageHeight2D - Mask; y++)
                for (int x = Mask * 4; x < (ImageWidth2D - Mask) * 4; x++)
                {
                    Sum = 0;
                    for (int i = 0 - Mask; i <= Mask; i++)
                        for (int j = 0 - Mask; j <= Mask; j++)
                            Sum += Pixels2D[x + i * 4, y + j];

                    Pixels2D[x, y] = (byte)(Sum / ((Mask * 2 + 1) * (Mask * 2 + 1)));
                }

            Pixels = GetPixelArray();
            LoadImage();
        }
        private void Smoothing_Median(object sender, RoutedEventArgs e)
        {
            double[] Median;
            for (int y = Mask; y < ImageHeight2D - Mask; y++)
                for (int x = Mask * 4; x < (ImageWidth2D - Mask) * 4; x++)
                {
                    Median = new double[(Mask * 2 + 1) * (Mask * 2 + 1)];
                    int index = 0;
                    for (int i = 0 - Mask; i <= Mask; i++)
                        for (int j = 0 - Mask; j <= Mask; j++)
                            Median[index++] = Pixels2D[x + i * 4, y + j];

                    Array.Sort(Median);
                    Pixels2D[x, y] = (byte)Median[Median.Length / 2];
                }

            Pixels = GetPixelArray();
            LoadImage();
        }
        private void FindEdges(object sender, RoutedEventArgs e)
        {
            int[,] Pixels2DGray = new int[ImageWidth2D, ImageHeight2D];
            for (int y = 0; y < ImageHeight2D; y++)
                for (int x = 0; x < ImageWidth2D * 4; x+=4)
                    Pixels2DGray[x / 4, y] = (int)(Pixels2D[x,y] * 0.11 + Pixels2D[x + 1,y] * 0.59 + Pixels2D[x + 2,y] * 0.3);

            int gx, gy, index = 0, h = ImageHeight2D - Mask * 2, w = ImageWidth2D - Mask * 2;
            byte[] PixelArray = new byte[w * h];
            for (int y = Mask; y < ImageHeight2D - Mask; y++)
                for (int x = Mask; x < ImageWidth2D - Mask; x++)
                {
                    gx = Pixels2DGray[x - 1, y + 1] + 2 * Pixels2DGray[x, y + 1] + Pixels2DGray[x + 1, y + 1]
                       - (Pixels2DGray[x - 1, y - 1] + 2 * Pixels2DGray[x, y - 1] + Pixels2DGray[x + 1, y - 1]);

                    gy = Pixels2DGray[x + 1, y - 1] + 2 * Pixels2DGray[x + 1, y] + Pixels2DGray[x + 1, y + 1]
                       - (Pixels2DGray[x - 1, y - 1] + 2 * Pixels2DGray[x - 1, y] + Pixels2DGray[x - 1, y + 1]);

                    int val = Math.Abs(gx) + Math.Abs(gy);
                    if (val > 255) val = 255;
                    PixelArray[index++] = (byte)val;
                }

            image.Source = BitmapSource.Create(w, h, 96, 96, PixelFormats.Gray8, null, PixelArray, w);
        }
        private void Sharp(object sender, RoutedEventArgs e)
        {
            double[] G = new double[9] { -1, -1, -1, -1, 9, -1, -1, -1, -1 };
            double Sum;

            byte[,] Pixels2Dx = new byte[ImageWidth2D * 4, ImageHeight2D];
            for (int y = 0; y < ImageHeight2D; y++)
                for (int x = 0; x < ImageWidth2D * 4; x += 4)
                    Pixels2Dx[x, y] = Pixels2D[x, y];

            for (int y = Mask; y < ImageHeight2D - Mask; y++)
                for (int x = Mask * 4; x < (ImageWidth2D - Mask) * 4; x++)
                {
                    Sum = 0;
                    for (int i = -1; i <= 1; i++)
                        for (int j = -1; j <= 1; j++)
                            Sum += G[(i + 1) * 3 + j + 1] * Pixels2D[x + i * 4, y + j];

                    Sum = Math.Abs(Sum) > 255 ? 255 : Math.Abs(Sum);
                    Pixels2Dx[x, y] = (byte)Sum;
                }

            Pixels2D = Pixels2Dx;
            Pixels = GetPixelArray();
            LoadImage();
        }
        private void Blur(object sender, RoutedEventArgs e)
        {
            double[] G = new double[9] { 1, 2, 1, 2, 4, 2, 1, 2, 1 };
            double Sum;
            for (int y = 1; y < ImageHeight2D - 1; y++)
                for (int x = 4; x < (ImageWidth2D - 1) * 4; x++)
                {
                    Sum = 0;
                    for (int i = -1; i <= 1; i++)
                        for (int j = -1; j <= 1; j++)
                            Sum += G[(i + 1) * 3 + j + 1] * Pixels2D[x + i * 4, y + j];

                    Pixels2D[x, y] = (byte)(Sum / 16);
                }

            Pixels = GetPixelArray();
            LoadImage();
        }
    }
}
