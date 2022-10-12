using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace Grafika_Komputerowa.Pages
{
    /// <summary>
    /// Logika interakcji dla klasy PPMPage.xaml
    /// </summary>
    public partial class PPMPage : Page
    {
        private BitmapSource BitImage;
        private FileStream MyFile;
        private string ImageFormat;
        private int ImageHeight;
        private int ImageWidth;
        private int MaxColorValue;
        public static int Compression = 1;

        public PPMPage()
        {
            InitializeComponent();
        }

        private void LoadButtonClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Format PPM (*.ppm)|*.ppm|Format JPEG (*.jpg)|*.jpg";
            if (openFileDialog.ShowDialog() == true)
            {
                string path = openFileDialog.FileName;
                using (MyFile = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    int filtrIndex = openFileDialog.FilterIndex;
                    if (filtrIndex == 2)
                    {
                        BitImage = new BitmapImage(new Uri(path));
                        image.Source = BitImage;
                    }
                    else if (SetImageFormat() && SetImageSize() && SetMaxColorValue())
                    {
                        switch(ImageFormat)
                        {
                            case "P3": if(ReadP3Format()) image.Source = BitImage; break;
                            case "P6": if(ReadP6Format()) image.Source = BitImage; break;
                        }
                    }
                }
                MyFile.Close();
                MyFile = null;
            }
        }

        private bool SetImageFormat()
        {
            byte[] Bytes = new byte[2];
            MyFile.Read(Bytes);
            if (Bytes.Length == 2 && Bytes[0] == Convert.ToByte('P'))
            {
                if (Bytes[1] == Convert.ToByte('3')) ImageFormat = "P3";
                else if (Bytes[1] == Convert.ToByte('6')) ImageFormat = "P6";
                else { MessageBox.Show("Nieprawidłowy format pliku (ani P3, ani P6)", "Błąd odczytu"); return false; }
                return true;
            }
            return false;
        }

        private bool SetImageSize()
        {
            int ReadBytes, State = 0, WidthIndex = 0, HeightIndex = 0;
            byte[] SingleByte = new byte[1];
            byte[] WidthBytes = new byte[6];
            byte[] HeightBytes = new byte[6];
            do
            {
                ReadBytes = MyFile.Read(SingleByte);
                SkipComment(SingleByte);
                switch (State)
                {
                    case 0: if (IsNumber(SingleByte[0])) { WidthBytes[WidthIndex++] = SingleByte[0]; State++; } break;
                    case 1: if (IsNumber(SingleByte[0])) WidthBytes[WidthIndex++] = SingleByte[0]; else State++; break;
                    case 2: if (IsNumber(SingleByte[0])) { HeightBytes[HeightIndex++] = SingleByte[0]; State++; } break;
                    case 3: if (IsNumber(SingleByte[0])) HeightBytes[HeightIndex++] = SingleByte[0]; else ReadBytes = 0; break;
                }
            }
            while (ReadBytes > 0);
            if(WidthBytes[0] != 0 && HeightBytes[0] != 0)
            {
                ImageWidth = BytesToInt(WidthBytes);
                ImageHeight = BytesToInt(HeightBytes);
            }
            else
            {
                MessageBox.Show("Plik nie zawiera zdefiniowanego rozmiaru grafiki", "Błąd rozmiaru");
                return false;
            }
            return true;
        }

        private bool SetMaxColorValue()
        {
            MaxColorValue = 0;
            int ReadBytes, State = 0, MaxColorValueIndex = 0;
            byte[] SingleByte = new byte[1];
            byte[] MaxColorValueBytes = new byte[6];
            do
            {
                ReadBytes = MyFile.Read(SingleByte);
                SkipComment(SingleByte);
                switch (State)
                {
                    case 0: if (IsNumber(SingleByte[0])) { MaxColorValueBytes[MaxColorValueIndex++] = SingleByte[0]; State++; } break;
                    case 1: if (IsNumber(SingleByte[0])) MaxColorValueBytes[MaxColorValueIndex++] = SingleByte[0]; else ReadBytes = 0; break;
                }
            }
            while (ReadBytes > 0);
            if (MaxColorValueBytes[0] != 0) MaxColorValue = BytesToInt(MaxColorValueBytes);
            else
            {
                MessageBox.Show("Plik nie zawiera zdefiniowanej maksymalnej wartości koloru", "Błąd rozmiaru");
                return false;
            }
            return true;
        }

        private bool IsNumber(byte Byte)
        {
            if (Byte >= Convert.ToByte('0') && Byte <= Convert.ToByte('9')) return true;
            return false;
        }

        private void SkipComment(byte[] Bytes)
        {
            if(Bytes[0] == Convert.ToByte('#'))
            {
                int ReadCount;
                do ReadCount = MyFile.Read(Bytes); while (ReadCount > 0 && Bytes[0] != 10);
            }
        }

        private int BytesToInt(byte[] Bytes)
        {
            int Value = 0, Base = 1;
            for(int Index = Bytes.Length - 1; Index >= 0; Index--)
                if(Bytes[Index] != 0)
                {
                    Value += (Bytes[Index] - 48) * Base;
                    Base *= 10;
                }
            return Value;
        }

        private bool ReadP3Format()
        {
            int BufferSize = 1024 * 1024, ColorIndex = 0, ByteValue = 0, ReadBytesCount, BytesIndex;
            bool GetColor = false;
            byte[] Bytes;
            byte[] ConvertedBytes = new byte[ImageWidth * ImageHeight * 3];
            do
            {
                Bytes = new byte[BufferSize];
                ReadBytesCount = MyFile.Read(Bytes);
                BytesIndex = 0;
                while (BytesIndex < ReadBytesCount)
                {
                    if (IsNumber(Bytes[BytesIndex])) { ByteValue = (ByteValue * 10) + Bytes[BytesIndex] - 48; GetColor = true; }
                    else if (GetColor)
                    {
                        if(MaxColorValue > 255) ConvertedBytes[ColorIndex] = (byte)(255 * ByteValue / MaxColorValue);
                        else ConvertedBytes[ColorIndex] = (byte)ByteValue;
                        ColorIndex++;
                        ByteValue = 0;
                        GetColor = false;
                    }
                    BytesIndex++;
                }
            }
            while (ReadBytesCount > 0);
            if (ColorIndex != ImageWidth * ImageHeight * 3)
            {
                MessageBox.Show("Ilość bajtów reprezentujących grafikę nie zgadza się ze zdefiniowanym rozmiarem", "Błąd rozmiaru");
                return false;
            }
            BitImage = BitmapSource.Create(ImageWidth, ImageHeight, 96, 96, PixelFormats.Rgb24, null, ConvertedBytes, ImageWidth * 3);
            return true;
        }

        private bool ReadP6Format()
        {
            byte[] Bytes = new byte[ImageWidth * ImageHeight * 3];
            if (MyFile.Read(Bytes) != ImageWidth * ImageHeight * 3)
            {
                MessageBox.Show("Ilość bajtów reprezentujących grafikę nie zgadza się ze zdefiniowanym rozmiarem", "Błąd rozmiaru");
                return false;
            }
            BitImage = BitmapSource.Create(ImageWidth, ImageHeight, 96, 96, PixelFormats.Rgb24, null, Bytes, ImageWidth * 3);
            return true;
        }

        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            CompressionWindow cw = new CompressionWindow();
            cw.ShowDialog();
            if (Compression != 0)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Format JPEG|*.jpg";
                saveFileDialog.Title = "Zapisz jako obraz JPEG";
                if (saveFileDialog.ShowDialog() == true)
                {
                    JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                    BitmapFrame outputFrame = BitmapFrame.Create(BitImage);
                    encoder.Frames.Add(outputFrame);
                    encoder.QualityLevel = Compression;

                    using (MyFile = new FileStream(saveFileDialog.FileName, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        encoder.Save(MyFile);
                        MyFile.Close();
                    }
                }
            }
        }
    }
}
