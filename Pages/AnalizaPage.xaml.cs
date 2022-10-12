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
    /// Logika interakcji dla klasy AnalizaPage.xaml
    /// </summary>
    public partial class AnalizaPage : Page
    {
        private BitmapImage bitmapImage;
        private BitmapSource bitmapConvert;
        private byte[] Pixels;
        private byte[,] Pixels2D;
        private byte[] BasePixels;
        private int ImageWidth;
        private int ImageHeight;
        private int Y_MIN, U_MIN, V_MIN;
        private int Y_MAX, U_MAX, V_MAX;
        private byte[,] Struktura;

        public AnalizaPage()
        {
            InitializeComponent();
            Y_MIN = 0; U_MIN = 0; V_MIN = 0;
            Y_MAX = 40; U_MAX = 40; V_MAX = 40;
    }
        private void LoadFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "XD|*.*|JPEG|*.jpg|PNG|*.PNG";
            if (openFileDialog.ShowDialog() == true)
            {
                using (var file = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read))
                {
                    bitmapImage = new BitmapImage(new Uri(openFileDialog.FileName));
                    ImageWidth = bitmapImage.PixelWidth;
                    ImageHeight = bitmapImage.PixelHeight;

                    Pixels = new byte[ImageWidth * 4 * ImageHeight];
                    BasePixels = new byte[ImageWidth * 4 * ImageHeight];
                    
                    bitmapImage.CopyPixels(Pixels, ImageWidth * 4, 0);
                    Pixels.CopyTo(BasePixels, 0);
                    SetPixels2D();

                    image.Source = bitmapImage;

                    B0.IsEnabled = true;
                    B1.IsEnabled = true;
                    B2.IsEnabled = true;

                    B3.IsEnabled = true;
                    B4.IsEnabled = true;
                    B5.IsEnabled = true;
                    B6.IsEnabled = true;

                    Struktura = new byte[,]
                    {
                        {0,1,0},
                        {1,1,1},
                        {0,1,0}
                    };
                    //T1.IsEnabled = true;
                }
            }
        }
        private void SetPixels2D()
        {
            int index = 0;
            Pixels2D = new byte[ImageWidth * 4, ImageHeight];
            for (int y = 0; y < ImageHeight; y++)
                for (int x = 0; x < ImageWidth * 4; x++)
                    Pixels2D[x, y] = Pixels[index++];
        }
        private void SetPixels()
        {
            int index = 0;
            Pixels = new byte[ImageWidth * 4 * ImageHeight];
            for (int y = 0; y < ImageHeight; y++)
                for (int x = 0; x < ImageWidth * 4; x++)
                    Pixels[index++] = Pixels2D[x, y];
        }
        private void RestoreOriginal(object sender, RoutedEventArgs e)
        {
            BasePixels.CopyTo(Pixels, 0);
            SetPixels2D();
            image.Source = BitmapSource.Create(ImageWidth, ImageHeight, 96, 96, PixelFormats.Bgra32, null, Pixels, ImageWidth * 4);

            YMIN.Value = UMIN.Value = VMIN.Value = 0;
            YMAX.Value = UMAX.Value = VMAX.Value = 40;
        }

        private void Dylatacja()
        {
            SetPixels2D();
            byte[] PixelArray = new byte[ImageWidth * ImageHeight * 4];
            for (int y = 0; y < ImageHeight; y++)
                for (int x = 0; x < ImageWidth * 4; x += 4)
                {
                    int xd = 0;
                    if (x != 0 && y != 0 && x != ImageWidth * 4 - 4 && y != ImageHeight - 1)
                    {
                        for (int i = -1; i < 2; i++)
                            for (int j = -1; j < 2; j++)
                                if (Struktura[1 + i, 1 + j] == 1 && Pixels2D[x + i * 4, y + j] > 0) xd++;

                        if (xd > 0)
                        {
                            PixelArray[y * ImageWidth + x + 0] = Pixels2D[x + 0, y];
                            PixelArray[y * ImageWidth + x + 1] = Pixels2D[x + 1, y];
                            PixelArray[y * ImageWidth + x + 2] = Pixels2D[x + 2, y];
                            PixelArray[y * ImageWidth + x + 3] = 255;
                        }

                    }
                }

            PixelArray.CopyTo(Pixels, 0);

            image.Source = BitmapSource.Create(ImageWidth, ImageHeight, 96, 96, PixelFormats.Bgra32, null, Pixels, ImageWidth * 4);
        }
        private void Erozja()
        {
            SetPixels2D();
            int index = 0;
            byte[] PixelArray = new byte[ImageWidth * ImageHeight * 4];
            for (int y = 0; y < ImageHeight; y++)
                for (int x = 0; x < ImageWidth * 4; x += 4)
                {
                    int xd = 0;
                    if (x != 0 && y != 0 && x != ImageWidth * 4 - 4 && y != ImageHeight - 1)
                    {
                        for (int i = -1; i < 2; i++)
                            for (int j = -1; j < 2; j++)
                                if (Struktura[1 + i, 1 + j] == 1 && Pixels2D[x + i * 4, y + j] > 0) xd++;

                        if (xd == 5)
                        {
                            PixelArray[index + 0] = Pixels2D[x + 0, y];
                            PixelArray[index + 1] = Pixels2D[x + 1, y];
                            PixelArray[index + 2] = Pixels2D[x + 2, y];
                            PixelArray[index + 3] = 255;
                        }
                    }
                    index++;
                }

            PixelArray.CopyTo(Pixels, 0);
            SetPixels2D();

            image.Source = BitmapSource.Create(ImageWidth, ImageHeight, 96, 96, PixelFormats.Bgra32, null, Pixels, ImageWidth * 4);
        }

        private void DylatacjaClick(object sender, RoutedEventArgs e)
        {
            Dylatacja();
        }
        private void ErozjaClick(object sender, RoutedEventArgs e)
        {
            Erozja();
        }
        private void OtwarcieClick(object sender, RoutedEventArgs e)
        {
            Erozja();
            Dylatacja();
        }
        private void DomkniecieClick(object sender, RoutedEventArgs e)
        {
            Dylatacja();
            Erozja();
        }

        private void Preset1(object sender, RoutedEventArgs e)
        {
            YMIN.Value = 6;
            UMIN.Value = VMIN.Value = 0;
            YMAX.Value = 14;
            UMAX.Value = VMAX.Value = 20;
        }
        private void Preset2(object sender, RoutedEventArgs e)
        {
            YMIN.Value = 0;
            UMIN.Value = VMIN.Value = 0;
            YMAX.Value = 24;
            UMAX.Value = VMAX.Value = 20;
        }

        private void SetSliderValue(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider slider = (Slider)sender;
            string name = slider.Name;
            int value = Convert.ToInt32(slider.Value);
            switch(name)
            {
                case "YMIN":Y_MIN = value; break;
                case "YMAX":Y_MAX = value; break;
                case "UMIN":U_MIN = value; break;
                case "UMAX":U_MAX = value; break;
                case "VMIN":V_MIN = value; break;
                case "VMAX":V_MAX = value; break;
            }
            XD(); 
        }
        private void XD()
        {
            if (Pixels != null)
            {
                int pixelcount = 0;
                TY.Text = "Y: [" + Y_MIN + "-" + Y_MAX + "]";
                TU.Text = "U: [" + U_MIN + "-" + U_MAX + "]";
                TV.Text = "V: [" + V_MIN + "-" + V_MAX + "]";

                BasePixels.CopyTo(Pixels, 0);
                int[] Pixels_YUV = new int[Pixels.Length];
                double a = 0.001;

                for (int i = 0; i < Pixels.Length; i += 4)
                {
                    Pixels_YUV[i + 0] = Convert.ToInt32((Pixels[i + 2] * 0.299 + Pixels[i + 1] * 0.587 + Pixels[i] * 0.114) / (6.375 + a));
                    Pixels_YUV[i + 1] = Convert.ToInt32((Pixels[i + 2] * -0.147 + Pixels[i + 1] * -0.289 + Pixels[i] * 0.436 + 111.18) / (5.559 + a));
                    Pixels_YUV[i + 2] = Convert.ToInt32((Pixels[i + 2] * 0.615 + Pixels[i + 1] * -0.515 + Pixels[i] * -0.100 + 156.825) / (7.84125 + a));

                    if (Pixels_YUV[i + 0] < Y_MIN || Pixels_YUV[i + 0] > Y_MAX ||
                        Pixels_YUV[i + 1] < U_MIN || Pixels_YUV[i + 1] > U_MAX ||
                        Pixels_YUV[i + 2] < V_MIN || Pixels_YUV[i + 2] > V_MAX)
                    {
                        Pixels[i] = Pixels[i + 1] = Pixels[i + 2] = 0;
                        pixelcount += 4;
                    }
                }
                int allpx = Convert.ToInt32(Pixels.Length) / 4;
                int procent = Convert.ToInt32((Convert.ToDouble(Pixels.Length) - pixelcount) / Convert.ToDouble(Pixels.Length) * 100);
                PIXELS.Text = "Pixeli obiektu: ["+ (allpx - pixelcount / 4) + "px / "+ allpx + "px]";
                PROCENT.Text = "Procent obiektu: [" + procent + "%]";

                bitmapConvert = BitmapSource.Create(ImageWidth, ImageHeight, 96, 96, PixelFormats.Bgra32, null, Pixels, ImageWidth * 4);
                image.Source = bitmapConvert;
            }
        }

        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Format PNG|*.png";
            saveFileDialog.Title = "Zapisz jako obraz PNG";
            if (saveFileDialog.ShowDialog() == true)
            {
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                BitmapFrame outputFrame = BitmapFrame.Create(bitmapConvert);
                encoder.Frames.Add(outputFrame);

                using (var MyFile = new FileStream(saveFileDialog.FileName, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    encoder.Save(MyFile);
                    MyFile.Close();
                }
            }
        }
    }
}
