using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
    /// Logika interakcji dla klasy Morfologia.xaml
    /// </summary>
    public partial class MorfologiaPage : Page
    {
        private BitmapImage bitmapImage;
        private byte[] Pixels;
        private byte[,] Pixels2D;
        private byte[,] Struktura;
        private byte[] BasePixels;
        private int ImageWidth;
        private int ImageHeight;

        public MorfologiaPage()
        {
            InitializeComponent();
        }
        private void LoadFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Plik PNG |*.PNG| XD |*.jpg";
            if (openFileDialog.ShowDialog() == true)
            {
                using (var file = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read))
                {
                    bitmapImage = new BitmapImage(new Uri(openFileDialog.FileName));
                    ImageWidth = bitmapImage.PixelWidth;
                    ImageHeight = bitmapImage.PixelHeight;

                    Pixels = new byte[ImageWidth * ImageHeight];
                    BasePixels = new byte[ImageWidth * ImageHeight];

                    if(bitmapImage.Format != PixelFormats.Gray8)
                    {
                        byte[] Px = new byte[ImageWidth * ImageHeight * 4];
                        bitmapImage.CopyPixels(Px, ImageWidth * 4, 0);
                        for (int i = 0; i < Px.Length; i += 4)
                            Pixels[i/4] = (byte)((double)Px[i]*0.1 + (double)Px[i + 1]*0.8 + (double)Px[i + 2]*0.1);
                    }
                    else bitmapImage.CopyPixels(Pixels, ImageWidth, 0);
                    Pixels.CopyTo(BasePixels, 0);

                    image.Source = bitmapImage;

                    Binaryzacja();

                    int index = 0;
                    Pixels2D = new byte[ImageWidth, ImageHeight];
                    for (int y = 0; y < ImageHeight; y++)
                        for (int x = 0; x < ImageWidth; x++)
                            Pixels2D[x, y] = Pixels[index++];

                    Struktura = new byte[,]
                    {
                        {0,1,0},
                        {1,1,1},
                        {0,1,0}
                    };


                    B0.IsEnabled = true;
                    B1.IsEnabled = true;
                    B2.IsEnabled = true;
                    B3.IsEnabled = true;
                    B4.IsEnabled = true;
                }
            }
        }
        private void RestoreClick(object sender, RoutedEventArgs e)
        {
            BasePixels.CopyTo(Pixels, 0);

            Binaryzacja();

            int index = 0;
            Pixels2D = new byte[ImageWidth, ImageHeight];
            for (int y = 0; y < ImageHeight; y++)
                for (int x = 0; x < ImageWidth; x++)
                    Pixels2D[x, y] = Pixels[index++];


            image.Source = BitmapSource.Create(ImageWidth, ImageHeight, 96, 96, PixelFormats.Gray8, null, Pixels, ImageWidth);
        }

        private void Binaryzacja()
        {
            BasePixels.CopyTo(Pixels, 0);
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
        private void Dylatacja()
        {
            int index = 0;
            byte[] PixelArray = new byte[ImageWidth * ImageHeight];
            Pixels.CopyTo(PixelArray, 0);
            for (int y = 0; y < ImageHeight; y++)
                for (int x = 0; x < ImageWidth; x++)
                {
                    int xd = 0;
                    if (x != 0 && y != 0 && x != ImageWidth - 1 && y != ImageHeight - 1)
                    {
                        for (int i = -1; i < 2; i++)
                            for (int j = -1; j < 2; j++)
                                if (Struktura[1 + i, 1 + j] == 1 && Pixels2D[x + i, y + j] > 0) xd++;

                        if(xd > 0) PixelArray[index] = 255;
                    }
                    index++;
                }

            index = 0;
            PixelArray.CopyTo(Pixels, 0);
            Pixels2D = new byte[ImageWidth, ImageHeight];
            for (int y = 0; y < ImageHeight; y++)
                for (int x = 0; x < ImageWidth; x++)
                    Pixels2D[x, y] = Pixels[index++];

            image.Source = BitmapSource.Create(ImageWidth, ImageHeight, 96, 96, PixelFormats.Gray8, null, Pixels, ImageWidth);
        }
        private void Erozja()
        {
            int index = 0;
            byte[] PixelArray = new byte[ImageWidth * ImageHeight];
            //Pixels.CopyTo(PixelArray, 0);
            for (int y = 0; y < ImageHeight; y++)
                for (int x = 0; x < ImageWidth; x++)
                {
                    int xd = 0;
                    if (x != 0 && y != 0 && x != ImageWidth - 1 && y != ImageHeight - 1)
                    {
                        for (int i = -1; i < 2; i++)
                            for (int j = -1; j < 2; j++)
                                if (Struktura[1 + i, 1 + j] == 1 && Pixels2D[x + i, y + j] == 255) xd++;

                        if (xd == 5) PixelArray[index] = 255;
                    }
                    index++;
                }

            index = 0;
            PixelArray.CopyTo(Pixels, 0);
            Pixels2D = new byte[ImageWidth, ImageHeight];
            for (int y = 0; y < ImageHeight; y++)
                for (int x = 0; x < ImageWidth; x++)
                    Pixels2D[x, y] = Pixels[index++];

            image.Source = BitmapSource.Create(ImageWidth, ImageHeight, 96, 96, PixelFormats.Gray8, null, Pixels, ImageWidth);
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
        private void HitOrMissClick(object sender, RoutedEventArgs e)
        {
            int index = 0;
            byte[] PixelArray = new byte[ImageWidth * ImageHeight];
            for (int y = 0; y < ImageHeight; y++)
                for (int x = 0; x < ImageWidth; x++)
                {
                    int xd = 0;
                    if (x != 0 && y != 0 && x != ImageWidth - 1 && y != ImageHeight - 1)
                    {
                        for (int i = -1; i < 2; i++)
                            for (int j = -1; j < 2; j++)
                                if (Struktura[1 + i, 1 + j] == 1 && Pixels2D[x + i, y + j] > 0) xd++;

                        if (xd > 0 && xd < 3) PixelArray[index] = 255;
                    }
                    index++;
                }

            index = 0;
            PixelArray.CopyTo(Pixels, 0);
            Pixels2D = new byte[ImageWidth, ImageHeight];
            for (int y = 0; y < ImageHeight; y++)
                for (int x = 0; x < ImageWidth; x++)
                    Pixels2D[x, y] = Pixels[index++];

            image.Source = BitmapSource.Create(ImageWidth, ImageHeight, 96, 96, PixelFormats.Gray8, null, Pixels, ImageWidth);
        }

    }
}
