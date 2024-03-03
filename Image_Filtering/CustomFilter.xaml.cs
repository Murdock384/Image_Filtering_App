using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Image_Filtering
{
    public partial class CustomFilter : Window
    {
        private Point? selectedPoint = null;

        public CustomFilter()
        {
            InitializeComponent();

           
            FunctionGraph.Points = new PointCollection() { new Point(0, 0), new Point(255, 255) };
            DrawEllipse(new Point(0, 0));
            DrawEllipse (new Point(255,255));
        }


        private void DrawEllipse(Point position)
        {
            Ellipse ellipse = new Ellipse()
            {
                Width = 10,
                Height = 10,
                Fill = Brushes.Red,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };
            Canvas.SetLeft(ellipse, position.X - ellipse.Width / 2);
            Canvas.SetTop(ellipse, position.Y - ellipse.Height / 2);
            Canvas.Children.Add(ellipse);
        }

        private void UpdateEllipsePosition(int index, Point position)
        {
            Ellipse ellipse = Canvas.Children[index] as Ellipse;
            if (ellipse != null)
            {
                Canvas.SetLeft(ellipse, position.X - ellipse.Width / 2);
                Canvas.SetTop(ellipse, position.Y - ellipse.Height / 2);
            }
        }
        
        private void UpdatePolyline()
        {
            
            var orderedPoints = FunctionGraph.Points.OrderBy(p => p.X);
            FunctionGraph.Points = new PointCollection(orderedPoints);

           
            var existingEllipses = Canvas.Children.OfType<Ellipse>().ToList();
            foreach (var ellipse in existingEllipses)
            {
                if (!FunctionGraph.Points.Contains(new Point(Canvas.GetLeft(ellipse) + ellipse.Width / 2, Canvas.GetTop(ellipse) + ellipse.Height / 2)))
                {
                    Canvas.Children.Remove(ellipse);
                }
            }

            
            foreach (Point point in FunctionGraph.Points)
            {
                // Check if an ellipse already exists for this point
                if (!existingEllipses.Any(ellipse =>
                    Math.Abs(Canvas.GetLeft(ellipse) + ellipse.Width / 2 - point.X) < 1 &&
                    Math.Abs(Canvas.GetTop(ellipse) + ellipse.Height / 2 - point.Y) < 1))
                {
                    DrawEllipse(point);
                }
            }
        }


        private void Canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            

            Point mousePosition = e.GetPosition(Canvas);
            FunctionGraph.Points.Add(mousePosition);
            DrawEllipse(mousePosition);
            UpdatePolyline();
        }



      

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point clickPosition = e.GetPosition(Canvas);

            // Check if the click position is close to any point on the polyline
            foreach (Point point in FunctionGraph.Points)
            {
                if (IsCloseToPoint(clickPosition, point))
                {
                    if (selectedPoint == point)
                    {
                        // Deselect the point and update its appearance
                        selectedPoint = null;
                        UpdateEllipseAppearance(point, Brushes.Red);
                        XTextBox.Text = "";
                        YTextBox.Text = "";
                    }
                    else
                    {
                        // Deselect the previously selected point, if any
                        if (selectedPoint != null)
                        {
                            UpdateEllipseAppearance(selectedPoint.Value, Brushes.Red);
                        }

                        // Select the new point and update its appearance
                        selectedPoint = point;
                        UpdateEllipseAppearance(point, Brushes.Green);
                        XTextBox.Text = selectedPoint?.X.ToString();
                        YTextBox.Text = selectedPoint?.Y.ToString();
                    }
                    break;
                }
            }
        }



        private void MovePoint_Click(object sender, RoutedEventArgs e)
        {
            if (selectedPoint != null)
            {
                if (double.TryParse(XTextBox.Text, out double newX) && double.TryParse(YTextBox.Text, out double newY))
                {
                    
                    int index = FunctionGraph.Points.IndexOf(selectedPoint.Value);

                    
                    if (selectedPoint.Value.X == 0 || selectedPoint.Value.X == 255)
                    {
                        newY = Clamp(newY, 0, 255); 
                        FunctionGraph.Points[index] = new Point(selectedPoint.Value.X, newY);
                    }
                    else
                    {
                        
                        FunctionGraph.Points[index] = new Point(newX, newY);
                    }

                    
                    UpdateEllipsePosition(index, FunctionGraph.Points[index]);

                    
                    UpdatePolyline();
                }
                else
                {
                    MessageBox.Show("Invalid input for coordinates.");
                }
            }
            else
            {
                MessageBox.Show("No point selected.");
            }
        }

        
        private double Clamp(double value, double min, double max)
        {
            return Math.Max(min, Math.Min(value, max));
        }


        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Point clickPosition = e.GetPosition(Canvas);

            // Check if the click position is close to any point on the polyline
            for (int i = 1; i < FunctionGraph.Points.Count - 1; i++) // Exclude first and last points
            {
                Point point = FunctionGraph.Points[i];
                if (IsCloseToPoint(clickPosition, point))
                {
                    // Remove the intermediary point
                    FunctionGraph.Points.RemoveAt(i);

                    // Remove the corresponding ellipse
                    Canvas.Children.RemoveAt(i);

                    // Update the polyline
                    UpdatePolyline();

                    // Deselect any selected point
                    selectedPoint = null;

                    // Update UI
                    XTextBox.Text = "";
                    YTextBox.Text = "";

                    break;
                }
            }
        }



        private bool IsCloseToPoint(Point clickPosition, Point point)
        {
            
            double threshold = 10;

            
            double distance = Math.Sqrt(Math.Pow(clickPosition.X - point.X, 2) + Math.Pow(clickPosition.Y - point.Y, 2));

            
            return distance <= threshold;
        }

        private void UpdateEllipseAppearance(Point point, Brush fillBrush)
        {
            
            Ellipse ellipse = Canvas.Children.OfType<Ellipse>().FirstOrDefault(e =>
                Canvas.GetLeft(e) + e.Width / 2 == point.X &&
                Canvas.GetTop(e) + e.Height / 2 == point.Y);

            if (ellipse != null)
            {
                
                ellipse.Fill = fillBrush;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
           
            List<Point> functionPoints = new List<Point>();
            foreach (Point point in FunctionGraph.Points)
            {
                functionPoints.Add(point);
            }

            App.FilterPoints = functionPoints;
        }



    }
}
