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
    /// Logika interakcji dla klasy ShapePage.xaml
    /// </summary>
    public partial class ShapePage : Page
    {
        private List<Point> Points;
        private List<List<Line>> Shapes;
        private List<Line> SelectShape;

        public ShapePage()
        {
            InitializeComponent();
            Points = new List<Point>();
            Shapes = new List<List<Line>>();
            Shapes.Add(new List<Line>());
            SelectShape = null;
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
        private void CleanCanvas(object sender, RoutedEventArgs e)
        {
            canvas.Children.Clear();
            Points.Clear();
        }
        
        private void RightClickOnCanvas(object sender, MouseButtonEventArgs e)
        {
            if (SelectShape != null) FindShape();
            LastLine();
        }
        private void CloseShape(object sender, RoutedEventArgs e)
        {
            LastLine();
        }
        private void LastLine()
        {
            if (Points.Count > 2)
            {
                Line line = new Line
                {
                    X1 = Points[^1].X,
                    Y1 = Points[^1].Y,
                    X2 = Points[0].X,
                    Y2 = Points[0].Y,
                    Stroke = Brushes.Black,
                    StrokeThickness = 5
                };
                Shapes[^1].Add(line);
                canvas.Children.Add(line);
                Points.Clear();
                Shapes.Add(new List<Line>());
            }
        }

        private void LeftClickOnCanvas(object sender, MouseButtonEventArgs e)
        {
            if(Shapes[^1].Count == 0) FindShape();
            if(SelectShape == null) AddLine(e.GetPosition(canvas));
        }
        private void AddPoint(object sender, RoutedEventArgs e)
        {
            AddLine(new Point(Convert.ToInt32(X.Text), Convert.ToInt32(Y.Text)));
        }
        private void AddLine(Point P)
        {
            Points.Add(P);
            if(Points.Count>1)
            {
                Line line = new Line
                {
                    X1 = Points[^2].X,
                    Y1 = Points[^2].Y,
                    X2 = Points[^1].X,
                    Y2 = Points[^1].Y,
                    Stroke = Brushes.Black,
                    StrokeThickness = 5
                };
                Shapes[^1].Add(line);
                canvas.Children.Add(line);
            }

        }

        private void FindShape()
        {
            foreach (Line Line in canvas.Children)
                if (Line.IsMouseOver)
                    foreach (var shape in Shapes)
                        if (shape.Contains(Line))
                        {
                            SelectShape = shape;
                            foreach (var line in SelectShape)
                                line.Stroke = Brushes.Orange;
                            Line L = SelectShape[0];
                            X.Text = Convert.ToInt32(L.X1).ToString();
                            Y.Text = Convert.ToInt32(L.Y1).ToString();
                            return;
                        }

            SelectShape = null;
            foreach (Line child in canvas.Children) child.Stroke = Brushes.Black;
        }
        private void MoveShapeButton(object sender, RoutedEventArgs e)
        {
            Point Vector = new Point(Convert.ToInt32(X.Text), Convert.ToInt32(Y.Text));
            Move(Vector);
        }
        private void RotateShapeButton(object sender, RoutedEventArgs e)
        {
            double Alpha = Convert.ToDouble(A.Text) * Math.PI / 180;
            Point Vector = new Point(Convert.ToInt32(X.Text), Convert.ToInt32(Y.Text));
            Rotate(Vector, Alpha);
        }
        private void ScaleShapeButton(object sender, RoutedEventArgs e)
        {
            Point Vector = new Point(Convert.ToInt32(X.Text), Convert.ToInt32(Y.Text));
            Scale(Vector, Convert.ToInt32(A.Text));
        }
        private void EditShape(object sender, MouseEventArgs e)
        {
            if (SelectShape != null)
            {
                Point Vector = new Point();
                foreach (var line in SelectShape)
                {
                    Vector.X += line.X1;
                    Vector.Y += line.Y1;
                }
                Vector.X /= SelectShape.Count;
                Vector.Y /= SelectShape.Count;

                if (e.LeftButton == MouseButtonState.Pressed)  // move
                {
                    Vector.X = (e.GetPosition(canvas).X - Vector.X) / 20;
                    Vector.Y = (e.GetPosition(canvas).Y - Vector.Y) / 20;
                    Move(Vector);
                }
                else if (e.RightButton == MouseButtonState.Pressed)  // rotate
                {
                    double Alpha = (e.GetPosition(canvas).X - Vector.X) / 10000 * Math.PI;
                    Rotate(Vector, Alpha);
                }
                else if (e.MiddleButton == MouseButtonState.Pressed)  // scale
                {
                    double scale =  (10000 + e.GetPosition(canvas).X) / (10000 + Vector.X);
                    Scale(Vector, scale);
                }
            }
        }

        private void Move(Point Vector)
        {
            if (SelectShape != null)
            {
                foreach (var line in SelectShape)
                {
                    line.X1 += Vector.X;
                    line.X2 += Vector.X;
                    line.Y1 += Vector.Y;
                    line.Y2 += Vector.Y;
                }
            }

        }
        private void Rotate(Point Vector, double Alpha)
        {
            if (SelectShape != null)
            {
                foreach (var line in SelectShape)
                {
                    double X1 = line.X1, X2 = line.X2, Y1 = line.Y1, Y2 = line.Y2;
                    line.X1 = Vector.X + (X1 - Vector.X) * Math.Cos(Alpha) - (Y1 - Vector.Y) * Math.Sin(Alpha);
                    line.Y1 = Vector.Y + (X1 - Vector.X) * Math.Sin(Alpha) + (Y1 - Vector.Y) * Math.Cos(Alpha);

                    line.X2 = Vector.X + (X2 - Vector.X) * Math.Cos(Alpha) - (Y2 - Vector.Y) * Math.Sin(Alpha);
                    line.Y2 = Vector.Y + (X2 - Vector.X) * Math.Sin(Alpha) + (Y2 - Vector.Y) * Math.Cos(Alpha);
                }
            }
        }
        private void Scale(Point Vector, double Scale)
        {
            if (SelectShape != null)
            {
                foreach (var line in SelectShape)
                {
                    double X1 = line.X1, X2 = line.X2, Y1 = line.Y1, Y2 = line.Y2;
                    line.X1 = Vector.X + (X1 - Vector.X) * Scale;
                    line.X2 = Vector.X + (X2 - Vector.X) * Scale;

                    line.Y1 = Vector.Y + (Y1 - Vector.Y) * Scale;
                    line.Y2 = Vector.Y + (Y2 - Vector.Y) * Scale;
                }
            }
        }

    }
}
