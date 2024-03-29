﻿using System;
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
            
            Canvas.Loaded += Canvas_Loaded;
            FunctionGraph.Points = new PointCollection() { new Point(0, 0), new Point(255, 255) };
            UpdatePolyline();

        }
        private void Canvas_Loaded(object sender, RoutedEventArgs e)
        {
            DrawGridLines();
        }
        private void DrawGridLines()
        {
           
            var gridLines = Canvas.Children.OfType<Line>().ToList();
            foreach (var line in gridLines)
            {
                Canvas.Children.Remove(line);
            }

            
            for (int i = 0; i <= 10; i++)
            {
                double yPos = Canvas.ActualHeight / 10 * i;
                Line horizontalLine = new Line
                {
                    X1 = 0,
                    Y1 = yPos,
                    X2 = Canvas.ActualWidth,
                    Y2 = yPos,
                    Stroke = Brushes.White,
                    StrokeThickness = 0.5, 
                    StrokeDashArray = new DoubleCollection(new double[] { 2, 2 }), 
                    Opacity = 0.5 
                };
                Canvas.Children.Add(horizontalLine);
            }

           
            for (int i = 0; i <= 10; i++)
            {
                double xPos = Canvas.ActualWidth / 10 * i;
                Line verticalLine = new Line
                {
                    X1 = xPos,
                    Y1 = 0,
                    X2 = xPos,
                    Y2 = Canvas.ActualHeight,
                    Stroke = Brushes.White,
                    StrokeThickness = 0.5, 
                    StrokeDashArray = new DoubleCollection(new double[] { 2, 2 }),
                    Opacity = 0.5 
                };
                Canvas.Children.Add(verticalLine);
            }
        }

        private void DrawEllipse(Point position)
        {
            Ellipse ellipse = new Ellipse()
            {
                Width = 10,
                Height = 10,
                Fill = Brushes.Black,
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
            
            DrawGridLines();

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


            foreach (Point point in FunctionGraph.Points)
            {
                if (IsCloseToPoint(clickPosition, point))
                {
                    if (selectedPoint == point)
                    {

                        selectedPoint = null;
                        UpdateEllipseAppearance(point, Brushes.Red);
                        XTextBox.Text = "";
                        YTextBox.Text = "";
                    }
                    else
                    {

                        if (selectedPoint != null)
                        {
                            UpdateEllipseAppearance(selectedPoint.Value, Brushes.Red);
                        }


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


            for (int i = 1; i < FunctionGraph.Points.Count - 1; i++)
            {
                Point point = FunctionGraph.Points[i];
                if (IsCloseToPoint(clickPosition, point))
                {

                    FunctionGraph.Points.RemoveAt(i);


                    Canvas.Children.RemoveAt(i);


                    UpdatePolyline();


                    selectedPoint = null;


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

            App.CustomFilterInstance newFilter = new App.CustomFilterInstance
            {
                Name = "Custom Filter " + (App.customFilters.Count + 1), // Generate a unique name
                FilterPoints = functionPoints
            };

            App.customFilters.Add(newFilter);
            ((MainWindow)Application.Current.MainWindow).PopulateCustomFiltersMenu();
            MessageBox.Show("Saved Filter!");


        }

        //Modifying existing Functional Filters
        private void UpdatePolylineForInversionFilter()
        {
            FunctionGraph.Points.Clear();
            
           
            FunctionGraph.Points.Add(new Point(255, 0));
            FunctionGraph.Points.Add(new Point(0, 255));
            UpdatePolyline();
        }

        private void MenuItem_Inversion_Click(object sender, RoutedEventArgs e)
        {
            UpdatePolylineForInversionFilter();
        }

        private void UpdatePolylineForBrightnessCorrectionFilter()
        {
            FunctionGraph.Points.Clear();
            FunctionGraph.Points.Add(new Point(0,50));
            FunctionGraph.Points.Add(new Point(188, 255));
            FunctionGraph.Points.Add(new Point(255, 255));
            UpdatePolyline();
        }

        private void MenuItem_BrightnessCorrection_Click(object sender, RoutedEventArgs e)
        {
            UpdatePolylineForBrightnessCorrectionFilter();
        }

        private void UpdatePolylineForContrastEnhancementFilter()
        {
            FunctionGraph.Points.Clear();


            FunctionGraph.Points.Add(new Point(0, 0));
            FunctionGraph.Points.Add(new Point(50,0));
            FunctionGraph.Points.Add(new Point(205, 255));
            FunctionGraph.Points.Add(new Point(255, 255));
            UpdatePolyline();
        }

        private void MenuItem_ContrastEnhancementFilter_Click(object sender, RoutedEventArgs e)
        {
            UpdatePolylineForContrastEnhancementFilter();
        }

    }
}
