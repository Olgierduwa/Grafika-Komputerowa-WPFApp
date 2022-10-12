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
    /// Logika interakcji dla klasy Paint.xaml
    /// </summary>
    public partial class PaintPage : Page
    {
        private string shape;
        private Point Start;
        private Point End;
        private Shape DrawnFigure;
        private Shape SelectedFigure;
        private bool Pressed;

        public PaintPage()
        {
            InitializeComponent();
            shape = "Line";
            SelectedFigure = null;
            LineButton.Background = Brushes.LightGoldenrodYellow;
            Pressed = false;
        }

        private void SetButtonsDefault()
        {
            D1.Tag = D1.Text; D1.Foreground = Brushes.Gray; D1.Visibility = Visibility.Visible;
            D2.Tag = D2.Text; D2.Foreground = Brushes.Gray; D2.Visibility = Visibility.Visible;
            D3.Tag = D3.Text; D3.Foreground = Brushes.Gray; D3.Visibility = Visibility.Visible;
            D4.Tag = D4.Text; D4.Foreground = Brushes.Gray; D4.Visibility = Visibility.Visible;
            SelectButton.Background = Brushes.LightGray;
            LineButton.Background = Brushes.LightGray;
            CircleButton.Background = Brushes.LightGray;
            RectangleButton.Background = Brushes.LightGray;
            DrawButton.Visibility = Visibility.Visible;
        }
        private void Unselected()
        {
            if (SelectedFigure != null)
            {
                SelectedFigure.Stroke = Brushes.Black;
                SelectedFigure = null;
                OverwritePanel.Visibility = Visibility.Hidden;
            }
        }
        private void SetSelectedValues()
        {
            O1.Text = Convert.ToInt32(Canvas.GetLeft(SelectedFigure)).ToString();
            O2.Text = Convert.ToInt32(Canvas.GetTop(SelectedFigure)).ToString();
            O4.Text = "0";

            if (SelectedFigure is Line)
            {
                O3.Text = Convert.ToInt32(Canvas.GetTop(SelectedFigure) + SelectedFigure.ActualHeight).ToString();
                O4.Text = Convert.ToInt32(Canvas.GetLeft(SelectedFigure) + SelectedFigure.ActualWidth).ToString();
                O4.Visibility = Visibility.Visible;
            }
            else if (SelectedFigure is Rectangle)
            {
                O3.Text = Convert.ToInt32(SelectedFigure.Height).ToString();
                O4.Text = Convert.ToInt32(SelectedFigure.Width).ToString();
                O4.Visibility = Visibility.Visible;
            }
            else if (SelectedFigure is Ellipse)
            {
                O1.Text = Convert.ToInt32(Canvas.GetLeft(SelectedFigure) + SelectedFigure.Height / 2).ToString();
                O2.Text = Convert.ToInt32(Canvas.GetTop(SelectedFigure) + SelectedFigure.Height / 2).ToString();
                O3.Text = Convert.ToInt32(SelectedFigure.Height / 2).ToString();
                O4.Visibility = Visibility.Hidden;
            }
        }

        private void SelectedButtonClick(object sender, RoutedEventArgs e)
        {
            shape = "Select";
            SetButtonsDefault();
            D1.Visibility = Visibility.Hidden;
            D2.Visibility = Visibility.Hidden;
            D3.Visibility = Visibility.Hidden;
            D4.Visibility = Visibility.Hidden;
            DrawButton.Visibility = Visibility.Hidden;
            SelectButton.Background = Brushes.LightGoldenrodYellow;
        }

        private void LineButtonClick(object sender, RoutedEventArgs e)
        {
            shape = "Line";
            D1.Text = "X1";
            D2.Text = "Y1";
            D3.Text = "X2";
            D4.Text = "Y2";
            Unselected();
            SetButtonsDefault();
            LineButton.Background = Brushes.LightGoldenrodYellow;
        }

        private void RectangleButtonClick(object sender, RoutedEventArgs e)
        {
            shape = "Rectangle";
            D1.Text = "X1";
            D2.Text = "Y1";
            D3.Text = "H";
            D4.Text = "W";
            Unselected();
            SetButtonsDefault();
            RectangleButton.Background = Brushes.LightGoldenrodYellow;
        }

        private void CircleButtonClick(object sender, RoutedEventArgs e)
        {
            shape = "Circle";
            D1.Text = "X1";
            D2.Text = "Y1";
            D3.Text = "R";
            D4.Text = "0";
            Unselected();
            SetButtonsDefault();
            D4.Visibility = Visibility.Hidden;
            CircleButton.Background = Brushes.LightGoldenrodYellow;
        }

        private void TextGotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textbox = (TextBox)sender;
            textbox.Text = "";
            textbox.Foreground = Brushes.Black;
        }

        private void TextLostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textbox = (TextBox)sender;
            if (textbox.Text == "")
            {
                textbox.Text = textbox.Tag.ToString();
                textbox.Foreground = Brushes.Gray;
            }
        }

        private void KeyPressDown(object sender, KeyEventArgs e)
        {

            Pressed = true;
        }

        private void KeyPressUp(object sender, KeyEventArgs e)
        {

            Pressed = false;
        }

        private bool IsNumeric(string text)
        {
            Regex reg = new Regex("[^0-9]");
            return reg.IsMatch(text);
        }



        private void DrawButtonClick(object sender, RoutedEventArgs e)
        {
            if (IsNumeric(D1.Text) || IsNumeric(D2.Text) || IsNumeric(D3.Text) || IsNumeric(D4.Text)) return;

            int X1, Y1, X2, Y2, Height, Width, Radius;
            X1 = Convert.ToInt32(D1.Text);
            Y1 = Convert.ToInt32(D2.Text);
            X2 = Height = Radius = Convert.ToInt32(D3.Text);
            Y2 = Width = Convert.ToInt32(D4.Text);

            Start = new Point(X1, Y1);
            End = new Point(X2, Y2);
            DrawnFigure = new Line();
            switch (shape)
            {
                case "Line": DrawnFigure = new Line { Stroke = Brushes.Black, StrokeThickness = 5, X2 = End.X - Start.X, Y2 = End.Y - Start.Y }; break;
                case "Rectangle": DrawnFigure = new Rectangle { Stroke = Brushes.Black, StrokeThickness = 5, Width = Width, Height = Height }; break;
                case "Circle":
                    DrawnFigure = new Ellipse { Stroke = Brushes.Black, StrokeThickness = 5, Width = Radius * 2, Height = Radius * 2 };
                    Start.X -= Radius; Start.Y -= Radius; break;
            }
            Canvas.SetLeft(DrawnFigure, Start.X);
            Canvas.SetTop(DrawnFigure, Start.Y);
            canvas.Children.Add(DrawnFigure);
        }

        private void OverwriteButtonClick(object sender, RoutedEventArgs e)
        {
            if (IsNumeric(O1.Text) || IsNumeric(O2.Text) || IsNumeric(O3.Text) || IsNumeric(O4.Text)) return;

            int X1, Y1, X2, Y2, Height, Width, Radius;
            X1 = Convert.ToInt32(O1.Text);
            Y1 = Convert.ToInt32(O2.Text);
            X2 = Height = Radius = Convert.ToInt32(O3.Text);
            Y2 = Width = Convert.ToInt32(O4.Text);

            Start = new Point(X1, Y1);
            End = new Point(X2, Y2);

            if (SelectedFigure is Line)
            {
                (SelectedFigure as Line).X2 = End.X - Start.X;
                (SelectedFigure as Line).Y2 = End.Y - Start.Y;
            }
            else if (SelectedFigure is Rectangle)
            {
                SelectedFigure.Width = Width;
                SelectedFigure.Height = Height;
            }
            else if (SelectedFigure is Ellipse)
            {
                SelectedFigure.Width = Radius * 2;
                SelectedFigure.Height = Radius * 2;
                Start.X -= Radius; Start.Y -= Radius;
            }
            Canvas.SetLeft(SelectedFigure, Start.X);
            Canvas.SetTop(SelectedFigure, Start.Y);
        }


        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Start = e.GetPosition(canvas);
            if (shape == "Select")
            {
                var childern = canvas.Children;
                Unselected();
                foreach (Shape child in childern)
                    if (child.IsMouseOver)
                    {
                        SelectedFigure = child;
                        SelectedFigure.Stroke = Brushes.Orange;
                        End.X = Start.X - Canvas.GetLeft(SelectedFigure);
                        End.Y = Start.Y - Canvas.GetTop(SelectedFigure);
                        OverwritePanel.Visibility = Visibility.Visible;
                        SetSelectedValues();
                    }
                    else child.Stroke = Brushes.Black;
            }
            else
            {
                Unselected();
                switch (shape)
                {
                    case "Line": DrawnFigure = new Line { Stroke = Brushes.Black, StrokeThickness = 5 }; break;
                    case "Rectangle": DrawnFigure = new Rectangle { Stroke = Brushes.Black, StrokeThickness = 5 }; break;
                    case "Circle": DrawnFigure = new Ellipse { Stroke = Brushes.Black, StrokeThickness = 5 }; break;
                }
                Canvas.SetLeft(DrawnFigure, Start.X);
                Canvas.SetTop(DrawnFigure, Start.Y);
                canvas.Children.Add(DrawnFigure);
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released) return;

            if (SelectedFigure != null)
            {
                if (Pressed)
                {
                    Start = new Point(Canvas.GetLeft(SelectedFigure), Canvas.GetTop(SelectedFigure));
                    if (SelectedFigure is Ellipse)
                    {
                        var Radius = SelectedFigure.Width / 2;
                        Start.X += Radius;
                        Start.Y += Radius;
                    }
                    ReDraw(e);
                    SetSelectedValues();
                }
                else Move(e);
            }
            else if (DrawnFigure != null) Draw(e);
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            DrawnFigure = null;
        }



        private void Draw(MouseEventArgs e)
        {
            End = e.GetPosition(canvas);
            Point CurrentStart = Start;

            if (DrawnFigure is Line)
            {
                (DrawnFigure as Line).X2 = End.X - Start.X;
                (DrawnFigure as Line).Y2 = End.Y - Start.Y;
            }
            else if (DrawnFigure is Rectangle)
            {
                CurrentStart.X = Math.Min(Start.X, End.X);
                CurrentStart.Y = Math.Min(Start.Y, End.Y);
                End.X = Math.Max(Start.X, End.X);
                End.Y = Math.Max(Start.Y, End.Y);
                DrawnFigure.Width = End.X - CurrentStart.X;
                DrawnFigure.Height = End.Y - CurrentStart.Y;
            }
            else if (DrawnFigure is Ellipse)
            {
                var Radius = Math.Sqrt(Math.Pow(Math.Max(End.X, Start.X) - Math.Min(End.X, Start.X), 2) + Math.Pow(Math.Max(End.Y, Start.Y) - Math.Min(End.Y, Start.Y), 2));
                DrawnFigure.Width = Radius * 2;
                DrawnFigure.Height = Radius * 2;
                CurrentStart.X -= Radius;
                CurrentStart.Y -= Radius;
            }
            Canvas.SetLeft(DrawnFigure, CurrentStart.X);
            Canvas.SetTop(DrawnFigure, CurrentStart.Y);
        }

        private void Move(MouseEventArgs e)
        {
            Point CurrentPos = e.GetPosition(canvas);
            SetSelectedValues();
            Canvas.SetLeft(SelectedFigure, CurrentPos.X - End.X);
            Canvas.SetTop(SelectedFigure, CurrentPos.Y - End.Y);
        }

        private void ReDraw(MouseEventArgs e)
        {
            End = e.GetPosition(canvas);
            Point CurrentStart = Start;

            if (SelectedFigure is Line)
            {
                (SelectedFigure as Line).X2 = End.X - Start.X;
                (SelectedFigure as Line).Y2 = End.Y - Start.Y;
            }
            else if (SelectedFigure is Rectangle)
            {
                CurrentStart.X = Math.Min(Start.X, End.X);
                CurrentStart.Y = Math.Min(Start.Y, End.Y);
                End.X = Math.Max(Start.X, End.X);
                End.Y = Math.Max(Start.Y, End.Y);
                SelectedFigure.Width = End.X - CurrentStart.X;
                SelectedFigure.Height = End.Y - CurrentStart.Y;
            }
            else if (SelectedFigure is Ellipse)
            {
                var Radius = Math.Sqrt(Math.Pow(Math.Max(End.X, Start.X) - Math.Min(End.X, Start.X), 2) + Math.Pow(Math.Max(End.Y, Start.Y) - Math.Min(End.Y, Start.Y), 2));
                SelectedFigure.Width = Radius * 2;
                SelectedFigure.Height = Radius * 2;
                CurrentStart.X -= Radius;
                CurrentStart.Y -= Radius;
            }
            Canvas.SetLeft(SelectedFigure, CurrentStart.X);
            Canvas.SetTop(SelectedFigure, CurrentStart.Y);
        }
    }
}
