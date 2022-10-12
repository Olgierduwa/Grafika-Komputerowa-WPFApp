using System;
using System.Collections.Generic;
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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Grafika_Komputerowa.Pages
{
    /// <summary>
    /// Logika interakcji dla klasy RGBPage.xaml
    /// </summary>
    public partial class RGBPage : Page
    {
        private byte R, G, B, C, M, Y, K;

        MeshGeometry3D side1Plane = new MeshGeometry3D();

        public RGBPage()
        {
            InitializeComponent();
            R = G = B = 0;
            C = M = Y = 0;
            K = 255;
            SK.Value = K;
            FillCube();
        }

        private void FillCube()
        {
            List<byte[]> Bytes = new List<byte[]>();
            Bytes.Add(GetPixelsArray(255, 255, 0, 255, 255, 255, 0, 255, 0));
            Bytes.Add(GetPixelsArray(255, 0, 0, 255, 0, 255, 0, 0, 0));
            Bytes.Add(GetPixelsArray(255, 0, 0, 255, 0, 255, 255, 255, 0));
            Bytes.Add(GetPixelsArray(0, 0, 0, 0, 0, 255, 0, 255, 0));
            Bytes.Add(GetPixelsArray(255, 255, 255, 255, 0, 255, 0, 255, 255));
            Bytes.Add(GetPixelsArray(255, 255, 0, 255, 0, 0, 0, 255, 0));

            ImageBrush[] brush = new ImageBrush[6];
            for (int i = 0; i < 6; i++)
            {
                brush[i] = new ImageBrush();
                brush[i].Viewbox = new Rect(0, 0, 256, 256);
                brush[i].TileMode = TileMode.None;
                brush[i].Stretch = Stretch.Fill;
                brush[i].ViewboxUnits = BrushMappingMode.Absolute;
                brush[i].ViewportUnits = BrushMappingMode.Absolute;
                brush[i].ImageSource = BitmapSource.Create(256, 256, 96, 96, PixelFormats.Rgb24, null, Bytes[i], 768);
            }

            GModel1.Material = new DiffuseMaterial(brush[0]);
            GModel2.Material = new DiffuseMaterial(brush[1]);
            GModel3.Material = new DiffuseMaterial(brush[2]);
            GModel4.Material = new DiffuseMaterial(brush[3]);
            GModel5.Material = new DiffuseMaterial(brush[4]);
            GModel6.Material = new DiffuseMaterial(brush[5]);
        }

        private byte[] GetPixelsArray(int R1, int G1, int B1, int R2, int G2, int B2, int R3, int G3, int B3)
        {
            byte[] Bytes = new byte[196608];
            int _R = R1, __R = R1;
            int _G = G1, __G = G1;
            int _B = B1, __B = B1;
            int XR = R1 > R2 ? -1 : R1 < R2 ? 1 : 0;
            int XG = G1 > G2 ? -1 : G1 < G2 ? 1 : 0;
            int XB = B1 > B2 ? -1 : B1 < B2 ? 1 : 0;
            int YR = R1 > R3 ? -1 : R1 < R3 ? 1 : 0;
            int YG = G1 > G3 ? -1 : G1 < G3 ? 1 : 0;
            int YB = B1 > B3 ? -1 : B1 < B3 ? 1 : 0;

            for (int x = 0; x < 256; x++)
            {
                for (int y = 0; y < 768; y += 3)
                {
                    Bytes[x * 768 + y + 0] = (byte)_R;
                    Bytes[x * 768 + y + 1] = (byte)_G;
                    Bytes[x * 768 + y + 2] = (byte)_B;
                    _R += XR;
                    _G += XG;
                    _B += XB;
                }
                __R += YR;
                __G += YG;
                __B += YB;
                _R = __R;
                _G = __G;
                _B = __B;
            }
            return Bytes;
        }
        private void Vertical_Scroll(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            
            GModel1.Transform = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), VScroll.Value));
            GModel2.Transform = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), VScroll.Value));
            GModel3.Transform = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), VScroll.Value));
            GModel4.Transform = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), VScroll.Value));
            GModel5.Transform = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), VScroll.Value));
            GModel6.Transform = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), VScroll.Value));
        }
        private void Horizontal_Scroll(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            GModel1.Transform = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), HScroll.Value));
            GModel2.Transform = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), HScroll.Value));
            GModel3.Transform = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), HScroll.Value));
            GModel4.Transform = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), HScroll.Value));
            GModel5.Transform = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), HScroll.Value));
            GModel6.Transform = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), HScroll.Value));
        }
        

        private void PrevTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox TB = (TextBox)sender;
            if (!IsNumeric(TB.Text + e.Text) && Convert.ToInt16(TB.Text + e.Text) > 0 && Convert.ToInt16(TB.Text + e.Text) < 256) e.Handled = false;
            else { e.Handled = true; TB.Text = "0"; TB.Focus(); }
        }
        private void RGBTextLostFocus(object sender, RoutedEventArgs e)
        {
            R = (byte)Convert.ToInt16(TR.Text);
            G = (byte)Convert.ToInt16(TG.Text);
            B = (byte)Convert.ToInt16(TB.Text);
            SR.Value = R;
            SG.Value = G;
            SB.Value = B;
            SetCMYKValues();
        }
        private void CMYKTextLostFocus(object sender, RoutedEventArgs e)
        {
            C = (byte)Convert.ToInt16(TC.Text);
            M = (byte)Convert.ToInt16(TM.Text);
            Y = (byte)Convert.ToInt16(TY.Text);
            K = (byte)Convert.ToInt16(TK.Text);
            SC.Value = C;
            SM.Value = M;
            SY.Value = Y;
            SK.Value = K;
            SetRGBValues();
        }
        private void SetRGBValue(object sender, MouseEventArgs e)
        {
            if(Mouse.LeftButton == MouseButtonState.Pressed)
            {
                R = (byte)SR.Value;
                G = (byte)SG.Value;
                B = (byte)SB.Value;
                SetCMYKValues();
            }
        }
        private void SetCMYKValue(object sender, MouseEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {

                C = (byte)SC.Value;
                M = (byte)SM.Value;
                Y = (byte)SY.Value;
                K = (byte)SK.Value;
                SetRGBValues();
            }
        }

        private void SetRGBValues()
        {
            double F = 1, PC = Convert.ToDouble(C) / 255, PM = Convert.ToDouble(M) / 255, PY = Convert.ToDouble(Y) / 255, PK = Convert.ToDouble(K) / 255;

            R = (byte)(255 * (F - Math.Min(F, PC * (F - PK) + PK)));
            G = (byte)(255 * (F - Math.Min(F, PM * (F - PK) + PK)));
            B = (byte)(255 * (F - Math.Min(F, PY * (F - PK) + PK)));

            SR.Value = R;
            SG.Value = G;
            SB.Value = B;

            SetColorRect();
        }
        private void SetCMYKValues()
        {
            double F = 1, PR = Convert.ToDouble(R) / 255, PG = Convert.ToDouble(G) / 255, PB = Convert.ToDouble(B) / 255;

            K = (byte)(255 * Math.Min(Math.Min(F - PR, F - PG), F - PB));

            double PK = Convert.ToDouble(K) / 255;

            C = (F - PK) > 0 ? (byte)(255 * ((F - PR - PK) / (F - PK))) : (byte)C;
            M = (F - PK) > 0 ? (byte)(255 * ((F - PG - PK) / (F - PK))) : (byte)M;
            Y = (F - PK) > 0 ? (byte)(255 * ((F - PB - PK) / (F - PK))) : (byte)Y;

            SC.Value = C;
            SM.Value = M;
            SY.Value = Y;
            SK.Value = K;

            SetColorRect();
        }

        private void SetColorRect()
        {
            TR.Text = R.ToString();
            TG.Text = G.ToString();
            TB.Text = B.ToString();

            TC.Text = C.ToString();
            TM.Text = M.ToString();
            TY.Text = Y.ToString();
            TK.Text = K.ToString();

            ColorRectRGB.Fill = new SolidColorBrush(Color.FromRgb(R,G,B));
        }
        private bool IsNumeric(string text)
        {
            Regex reg = new Regex("[^0-9]");
            return reg.IsMatch(text);
        }
    }
}
