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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Grafika_Komputerowa.Pages
{
    /// <summary>
    /// Logika interakcji dla klasy BézierPage.xaml
    /// </summary>
    public partial class BézierPage : Page
    {
        private List<Point> Points;
        private bool ShowLines = true;
        private bool ShowPoints = true;

        public BézierPage()
        {
            InitializeComponent();
            Points = new List<Point>();
        }

        private void ClickOnCanvas(object sender, MouseButtonEventArgs e)
        {
            Points.Add(e.GetPosition(canvas));
            canvas.Children.Clear();
            ShowPointsAndLines();
            if (Points.Count > 1) Loop();
        }
        
        private void Loop()
        {
            for (double t = 0; t < 1; t += 0.0005)
            {
                Rectangle R = new Rectangle { Width = 1, Height = 1, Fill = Brushes.Black };
                Point P = Bezier(Points.ToList(), t);
                Canvas.SetLeft(R, P.X);
                Canvas.SetTop(R, P.Y);
                canvas.Children.Add(R);
            }
        }

        private Point Bezier(List<Point> BasePoints, double t)
        {
            while (BasePoints.Count != 1)
            {
                List<Point> TempPoints = new List<Point>();
                for (int i = 0; i < BasePoints.Count - 1; i++)
                    TempPoints.Add(new Point {
                        X = (1 - t) * BasePoints[i].X + t * BasePoints[i + 1].X,
                        Y = (1 - t) * BasePoints[i].Y + t * BasePoints[i + 1].Y
                    });
                BasePoints = TempPoints.ToList();
            }
            return BasePoints[0];
        }

        private void CleanCanvas(object sender, RoutedEventArgs e)
        {
            Points = new List<Point>();
            canvas.Children.Clear();
        }

        private void ShowPointsClick(object sender, RoutedEventArgs e)
        {
            ShowPoints = !ShowPoints;
            Button b = (Button)sender;
            if(ShowPoints) b.Content = "Ukryj Punkty";
            else b.Content = "Pokaż Punkty";
            canvas.Children.Clear();
            ShowPointsAndLines();
            if (Points.Count > 1) Loop();
        }

        private void ShowLinesClick(object sender, RoutedEventArgs e)
        {
            ShowLines = !ShowLines;
            Button b = (Button)sender;
            if (ShowLines) b.Content = "Ukryj Linie";
            else b.Content = "Pokaż Linie";
            canvas.Children.Clear();
            ShowPointsAndLines();
            if (Points.Count > 1) Loop();
        }

        private void ShowPointsAndLines()
        {
            Point P2 = Points[0];
            foreach (var P1 in Points)
            {
                if (ShowPoints)
                {
                    Rectangle R = new Rectangle { Width = 10, Height = 10, Fill = Brushes.Blue };
                    Canvas.SetLeft(R, P1.X - 5);
                    Canvas.SetTop(R, P1.Y - 5);
                    canvas.Children.Add(R);
                }
                if (ShowLines)
                {
                    Line L = new Line { X1 = P1.X, Y1 = P1.Y, X2 = P2.X, Y2 = P2.Y, Stroke = Brushes.Blue, StrokeThickness = 1 };
                    canvas.Children.Add(L);
                }
                P2 = P1;
            }
        }

        private void AddPoint(object sender, RoutedEventArgs e)
        {
            Points.Add(new Point(Convert.ToInt32(X.Text), Convert.ToInt32(Y.Text)));
            canvas.Children.Clear();
            ShowPointsAndLines();
            if (Points.Count > 1) Loop();
        }

        private bool IsNumeric(string text)
        {
            Regex reg = new Regex("[^0-9]");
            return reg.IsMatch(text);
        }

        private void PrevTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox TB = (TextBox)sender;
            if (!IsNumeric(TB.Text + e.Text)) e.Handled = false;
            else { e.Handled = true; TB.Text = "0"; }
        }
    }
}
