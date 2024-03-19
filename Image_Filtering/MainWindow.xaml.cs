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
using Color = System.Windows.Media.Color;


namespace Image_Filtering
{

    public partial class MainWindow : Window
    {
        private Bitmap originalImage;
        private Bitmap filteredImage;
        public List<App.CustomFilterInstance> customFilters = new List<App.CustomFilterInstance>();

        public MainWindow()
        {
            InitializeComponent();
            this.WindowState = WindowState.Maximized;
           
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            PopulateCustomFiltersMenu();
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
        
        private void CustomFiltersWindowOpen_Click(object sender, RoutedEventArgs e)
        {
          
            CustomFilter customFiltersWindow = new CustomFilter();

            customFiltersWindow.Show();
        }

        private void CustomFiltersItems_Click(object sender, RoutedEventArgs e)
        {

            PopulateCustomFiltersMenu();
        }

        public void PopulateCustomFiltersMenu()
        {
            CustomFiltersItems.Items.Clear(); 

           
            foreach (var filter in App.customFilters)
            {
                MenuItem menuItem = new MenuItem();
                menuItem.Header = filter.Name; 
                menuItem.Click += (sender, e) => CustomFilterMenuItem_Click(sender, e);
                menuItem.Foreground = System.Windows.Media.Brushes.Black;
                CustomFiltersItems.Items.Add(menuItem); 
            }
        }
        private void CustomFilterMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            string filterName = menuItem.Header.ToString();

            
            App.CustomFilterInstance selectedFilter = App.customFilters.FirstOrDefault(filter => filter.Name == filterName);

            Bitmap imageToFilter = (filteredImage == null) ? originalImage : filteredImage;
            ApplyCustomFilterToImage(selectedFilter.FilterPoints,imageToFilter);
        }

        private void ApplyCustomFilterToImage(List<System.Windows.Point> functionPoints, Bitmap image)
        {
            if (image == null)
            {
                MessageBox.Show("Please open an image first.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Bitmap filteredImage = new Bitmap(image.Width, image.Height);

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    
                    System.Drawing.Color originalColor = image.GetPixel(x, y);

                   
                    double outputValueR = InterpolateOutputValueFromPolyline(functionPoints, originalColor.R);
                    double outputValueG = InterpolateOutputValueFromPolyline(functionPoints, originalColor.G);
                    double outputValueB = InterpolateOutputValueFromPolyline(functionPoints, originalColor.B);

                    
                    System.Drawing.Color filteredColor = System.Drawing.Color.FromArgb(originalColor.A,
                        (int)outputValueR, (int)outputValueG, (int)outputValueB);

                    filteredImage.SetPixel(x, y, filteredColor);
                }
            }

            
            DisplayImage(filteredImage, false);
        }




        private double InterpolateOutputValueFromPolyline(List<System.Windows.Point> functionPoints, int x)
        {

            System.Windows.Point leftPoint = functionPoints.LastOrDefault(p => p.X <= x);
            System.Windows.Point rightPoint = functionPoints.FirstOrDefault(p => p.X >= x);

            if (leftPoint == null || rightPoint == null)
            {

                return 0;
            }
            if(leftPoint == rightPoint)
            {
                return leftPoint.Y;
            }


            double y1 = leftPoint.Y;
            double y2 = rightPoint.Y;
            double x1 = leftPoint.X;
            double x2 = rightPoint.X;

            double outputValue = y1 + (y2 - y1) * (x - x1) / (x2 - x1);
            return outputValue;
        }



        private void GreyscaleFilter_Click(object sender, RoutedEventArgs e)
        {


            Bitmap imageToFilter = (filteredImage == null) ? originalImage : filteredImage;
            Bitmap greyscaleFiltered = Filters.ConvertToGrayscale(imageToFilter);
            DisplayImage(greyscaleFiltered, false);
        }
        private void MedianFilter_Click(object sender, RoutedEventArgs e)
        {
            

            Bitmap imageToFilter = (filteredImage == null) ? originalImage : filteredImage;
            Bitmap medianFiltered = Filters.ApplyMedianFilter(imageToFilter);
            DisplayImage(medianFiltered, false);
        }

        
        

     

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

        private void AverageDitheringKEqual2_Click(object sender, RoutedEventArgs e)
        {
            ApplyAverageDithering(2);
        }

        private void AverageDitheringKEqual4_Click(object sender, RoutedEventArgs e)
        {
            ApplyAverageDithering(4);
        }

        private void AverageDitheringKEqual8_Click(object sender, RoutedEventArgs e)
        {
            ApplyAverageDithering(8);
        }

        private void AverageDitheringKEqual16_Click(object sender, RoutedEventArgs e)
        {
            ApplyAverageDithering(16);
        }

        private void ApplyAverageDithering(int splits)
        {
            Bitmap imageToFilter = (filteredImage == null) ? originalImage : filteredImage;
            Bitmap ditheredImage = Dithering.ApplyAverageDithering(imageToFilter, splits);


            DisplayImage(ditheredImage, false);
        }

        private void OpenMedianCutWindow_Click(object sender, RoutedEventArgs e)
        {
            MedianCutColorPalleteSelector selectorWindow = new MedianCutColorPalleteSelector();
            if (selectorWindow.ShowDialog() == true)
            {
                int selectedPaletteCount = selectorWindow.SelectedPaletteCount;
                if (selectedPaletteCount > 0)
                {
                    Bitmap imageToFilter = (filteredImage == null) ? originalImage : filteredImage;
                    Bitmap quantizedImage = MedianCut.MedianCutColorQuantization.ApplyMedianCutColorQuantization(imageToFilter, selectedPaletteCount);
                    DisplayImage(quantizedImage, false);

                }
                else
                {
                    MessageBox.Show("Not Enough Color Palleted entered for processing!");
                }
                
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
                filteredImage = image;
            }
        }

        

        
    }


}