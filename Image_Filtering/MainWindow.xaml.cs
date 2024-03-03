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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using Microsoft.Win32;
using System.IO;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;


namespace Image_Filtering
{

    public partial class MainWindow : Window
    {
        private Bitmap originalImage;
        private Bitmap filteredImage;
        List<System.Windows.Point> functionPoints;

        public MainWindow()
        {
            InitializeComponent();
            this.WindowState = WindowState.Maximized;
        }
        private void OpenMenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.png;*.jpg;*.jpeg;*.gif;*.bmp)|*.png;*.jpg;*.jpeg;*.gif;*.bmp";

            if (openFileDialog.ShowDialog() == true)
            {
                originalImage = new Bitmap(openFileDialog.FileName);
                filteredImage = null;
                DisplayImage(originalImage, true);
                FilteredImageDisplay.Source = null;
            }
        }


        private void SaveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (filteredImage == null)
            {
                MessageBox.Show("No filtered image to save.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PNG Image (*.png)|*.png";
            if (saveFileDialog.ShowDialog() == true)
            {
                filteredImage.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        private void ResetMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (originalImage == null)
            {
                MessageBox.Show("Please open an image first.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            
            DisplayImage(originalImage, false);
            filteredImage = null;
        }
        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            
            Application.Current.Shutdown();
        }

        //Functional Filters
        
     
        private void ApplyFunctionalFilterMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            string filterType = menuItem.Header.ToString();

            if (originalImage == null)
            {
                MessageBox.Show("Please open an image first.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }


            Bitmap imageToFilter = (filteredImage == null) ? originalImage : filteredImage;


            filteredImage = ApplyFunctionalFilter(imageToFilter, filterType);


            DisplayImage(filteredImage, false);
        }
        
        private Bitmap ApplyFunctionalFilter(Bitmap image, string filterType)
        {
            switch (filterType)
            {
                case "Inversion":
                    return Filters.ApplyInversionFilter(image);
                case "Increase Brightness":
                    return Filters.ApplyBrightnessCorrectionFilter(image, 50);
                case "Decrease Brightness":
                    return Filters.ApplyBrightnessCorrectionFilter(image, -20);
                case "Contrast Enhancement":
                    return Filters.ApplyContrastEnhancementFilter(image, 20);
                case "Gamma Correction":
                    return Filters.ApplyGammaCorrectionFilter(image, 0.5);
                default:
                    return image;
            }
        }
        //Custom Filters
        private void CustomFiltersMenuItem_Click(object sender, RoutedEventArgs e)
        {
          
            CustomFilter customFiltersWindow = new CustomFilter();

            customFiltersWindow.Show();
        }

        private void ApplyCustomFilterMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Retrieve the saved filter points from the global variable
            functionPoints = App.FilterPoints;

            // Apply the filter to the selected image
            ApplyCustomFilterToImage(functionPoints);
        }

        private void ApplyCustomFilterToImage(List<System.Windows.Point> functionPoints)
        {
            if (originalImage == null)
            {
                MessageBox.Show("Please open an image first.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Create a new bitmap for the filtered image
            filteredImage = new Bitmap(originalImage.Width, originalImage.Height);

            for (int y = 0; y < originalImage.Height; y++)
            {
                for (int x = 0; x < originalImage.Width; x++)
                {
                    // Get the color value of the pixel in the original image
                    System.Drawing.Color originalColor = originalImage.GetPixel(x, y);

                    // Interpolate the output value from the polyline for each color component (R, G, B)
                    double outputValueR = InterpolateOutputValueFromPolyline(functionPoints, originalColor.R);
                    double outputValueG = InterpolateOutputValueFromPolyline(functionPoints, originalColor.G);
                    double outputValueB = InterpolateOutputValueFromPolyline(functionPoints, originalColor.B);

                    // Update the color value of the pixel in the filtered image
                    System.Drawing.Color filteredColor = System.Drawing.Color.FromArgb(originalColor.A,
                        (int)outputValueR, (int)outputValueG, (int)outputValueB);

                    filteredImage.SetPixel(x, y, filteredColor);
                }
            }

            // Display the filtered image
            DisplayImage(filteredImage, false);
        }

        private double InterpolateOutputValueFromPolyline(List<System.Windows.Point> functionPoints, int x)
        {
            
            System.Windows.Point leftPoint = functionPoints.FirstOrDefault(p => p.X <= x);
            System.Windows.Point rightPoint = functionPoints.LastOrDefault(p => p.X >= x);

            if (leftPoint == null || rightPoint == null)
            {
                
                return 0; 
            }

            
            double t = (x - leftPoint.X) / (rightPoint.X - leftPoint.X);
            double outputValue = leftPoint.Y + t * (rightPoint.Y - leftPoint.Y);
            return outputValue;
        }





        /* private Bitmap ApplyConvolutionalFilter(Bitmap image, int[,] kernel)
         {
             Bitmap filteredImage = new Bitmap(image.Width, image.Height);
             int kernelSize = kernel.GetLength(0);
             int radius = kernelSize / 2;

             for (int y = 0; y < image.Height; y++)
             {
                 for (int x = 0; x < image.Width; x++)
                 {
                     System.Drawing.Color filteredColor = ApplyKernel(image, kernel, x, y, kernelSize, radius);
                     filteredImage.SetPixel(x, y, filteredColor);
                 }
             }

             return filteredImage;
         }

         private System.Drawing.Color ApplyKernel(Bitmap image, int[,] kernel, int x, int y, int kernelSize, int radius)
         {
             int totalWeight = 0;
             int red = 0, green = 0, blue = 0;

             for (int i = 0; i < kernelSize; i++)
             {
                 for (int j = 0; j < kernelSize; j++)
                 {
                     int xOffset = x - radius + j;
                     int yOffset = y - radius + i;

                     if (xOffset >= 0 && xOffset < image.Width && yOffset >= 0 && yOffset < image.Height)
                     {
                         System.Drawing.Color pixelColor = image.GetPixel(xOffset, yOffset);

                         red += pixelColor.R * kernel[i, j];
                         green += pixelColor.G * kernel[i, j];
                         blue += pixelColor.B * kernel[i, j];

                         totalWeight += kernel[i, j];
                     }
                 }
             }

             if (totalWeight > 0)
             {
                 red /= totalWeight;
                 green /= totalWeight;
                 blue /= totalWeight;
             }

             red = Math.Min(255, Math.Max(0, red));
             green = Math.Min(255, Math.Max(0, green));
             blue = Math.Min(255, Math.Max(0, blue));

             return System.Drawing.Color.FromArgb(red, green, blue);
         }*/








        private void ApplyConvolutionalFilterMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            string filterType = menuItem.Header.ToString();

            if (originalImage == null)
            {
                MessageBox.Show("Please open an image first.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }


            Bitmap imageToFilter = (filteredImage == null) ? originalImage : filteredImage;


            filteredImage = ApplyConvolutionalFilter(imageToFilter, filterType);


            DisplayImage(filteredImage, false);
        }

        private Bitmap ApplyConvolutionalFilter(Bitmap image, string filterType)
        {
            switch (filterType)
            {
                case "Blur":
                    return Filters.ApplyBlurFilter(image);
                case "Gaussian Blur":
                    return Filters.ApplyGaussianBlurFilter(image);
                case "Sharpen":
                    return Filters.ApplySharpenFilter(image);
                case "Emboss":
                    return Filters.ApplyEmbossFilter(image);
                case "Edge Detection":
                    return Filters.ApplyEdgeDetectionFilter(image);
                default:
                    return image;
            }
        }


        private void DisplayImage(Bitmap image, bool isOriginal)
        {
            if (isOriginal)
            {
                OriginalImageDisplay.Source = Filters.ConvertBitmapToBitmapImage(image);
            }
            else
            {
                FilteredImageDisplay.Source = Filters.ConvertBitmapToBitmapImage(image);
            }
        }

        

        
    }


}