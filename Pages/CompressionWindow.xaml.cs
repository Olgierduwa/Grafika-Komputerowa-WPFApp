using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Grafika_Komputerowa.Pages
{
    /// <summary>
    /// Logika interakcji dla klasy CompressionWindow.xaml
    /// </summary>
    public partial class CompressionWindow : Window
    {
        private int CValue = 100;
        public CompressionWindow()
        {
            InitializeComponent();
        }

        private void button1(object sender, RoutedEventArgs e)
        {
            PPMPage.Compression = CValue;
            this.Close();
        }

        private void back(object sender, RoutedEventArgs e)
        {
            PPMPage.Compression = 0;
            this.Close();
        }

        private void ChangeValue(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            CValue = Convert.ToInt32(((Slider)sender).Value) * 10;
        }
    }
}
