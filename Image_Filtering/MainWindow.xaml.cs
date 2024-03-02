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


        private int[,] KernelBlur = {
            { 1, 1, 1 },
            { 1, 1, 1 },
            { 1, 1, 1 }
        };

        private int[,] KernelGaussianBlur = {
            { 0, 1, 0 },
            { 1, 4, 1 },
            { 0, 1, 0 }
        };

        private int[,] KernelSharpen = {
            { -1, -1, -1 },
            { -1, 9, -1 },
            { -1, -1, -1 }
        };

        private int[,] KernelEdgeDetection = {
            { -1, -1, -1 },
            { -1, 8, -1 },
            { -1, -1, -1 }
        };

        /*private int[,] KernelEdgeDetection = {
            { 0, 0, 0 },
            { -1, 1, 0 },
            { 0, 0, 0}
        };*/

        /*private int[,] KernelEmbossEast = {
            { -1, 0, 1 },
            { -1, 1, 1 },
            { -1, 0, 1 }
        };*/

        private int[,] KernelEmbossSouth = {
            { -1, -1, -1 },
            { 0, 1, 0 },
            { 1, 1, 1 }
        };

       /* private int[,] KernelEmbossSouthEast = {
            { -1, -1, 0 },
            { -1, 1, 1 },
            { 0, 1, 1 }
        };*/


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
        private Bitmap ApplyInversionFilter(Bitmap image)
        {
            Bitmap filteredImage = new Bitmap(image.Width, image.Height);

            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    System.Drawing.Color color = image.GetPixel(x, y);
                    System.Drawing.Color invertedColor = System.Drawing.Color.FromArgb(255 - color.R, 255 - color.G, 255 - color.B);
                    filteredImage.SetPixel(x, y, invertedColor);
                }
            }

            return filteredImage;
        }

        private Bitmap ApplyBrightnessCorrectionFilter(Bitmap image, double brightness)
        {
            Bitmap filteredImage = new Bitmap(image.Width, image.Height);

            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    System.Drawing.Color color = image.GetPixel(x, y);
                    int newRed = (int)Math.Min(255, Math.Max(0, color.R + brightness));
                    int newGreen = (int)Math.Min(255, Math.Max(0, color.G + brightness));
                    int newBlue = (int)Math.Min(255, Math.Max(0, color.B + brightness));
                    System.Drawing.Color correctedColor = System.Drawing.Color.FromArgb(newRed, newGreen, newBlue);
                    filteredImage.SetPixel(x, y, correctedColor);
                }
            }

            return filteredImage;
        }

        private Bitmap ApplyContrastEnhancementFilter(Bitmap image, double contrast)
        {
            Bitmap filteredImage = new Bitmap(image.Width, image.Height);

            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    System.Drawing.Color color = image.GetPixel(x, y);
                    int newRed = (int)Math.Min(255, Math.Max(0, 128 + contrast * (color.R - 128)));
                    int newGreen = (int)Math.Min(255, Math.Max(0, 128 + contrast * (color.G - 128)));
                    int newBlue = (int)Math.Min(255, Math.Max(0, 128 + contrast * (color.B - 128)));
                    System.Drawing.Color correctedColor = System.Drawing.Color.FromArgb(newRed, newGreen, newBlue);
                    filteredImage.SetPixel(x, y, correctedColor);
                }
            }

            return filteredImage;
        }


        private Bitmap ApplyGammaCorrectionFilter(Bitmap image, double gamma)
        {
            Bitmap filteredImage = new Bitmap(image.Width, image.Height);

            gamma = 1 / gamma;

            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    System.Drawing.Color color = image.GetPixel(x, y);


                    int newRed = (int)(255 * Math.Pow(color.R / 255.0, gamma));
                    int newGreen = (int)(255 * Math.Pow(color.G / 255.0, gamma));
                    int newBlue = (int)(255 * Math.Pow(color.B / 255.0, gamma));


                    newRed = Math.Max(0, Math.Min(255, newRed));
                    newGreen = Math.Max(0, Math.Min(255, newGreen));
                    newBlue = Math.Max(0, Math.Min(255, newBlue));

                    System.Drawing.Color correctedColor = System.Drawing.Color.FromArgb(newRed, newGreen, newBlue);
                    filteredImage.SetPixel(x, y, correctedColor);
                }
            }

            return filteredImage;
        }
        // Event handler for applying functional filters
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
                    return ApplyInversionFilter(image);
                case "Increase Brightness":
                    return ApplyBrightnessCorrectionFilter(image, 50);
                case "Decrease Brightness":
                    return ApplyBrightnessCorrectionFilter(image, -20);
                case "Contrast Enhancement":
                    return ApplyContrastEnhancementFilter(image, 20);
                case "Gamma Correction":
                    return ApplyGammaCorrectionFilter(image, 0.5);
                default:
                    return image;
            }
        }



        /*private Bitmap ApplyConvolutionalFilter(Bitmap image, int[,] kernel)
        {
            Bitmap filteredImage = new Bitmap(image.Width, image.Height);
            int kernelSize = kernel.GetLength(0);
            int radius = kernelSize / 2;

            Func<int, int, System.Drawing.Color> applyKernel = (x, y) =>
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
            };

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    System.Drawing.Color filteredColor = applyKernel(x, y);
                    filteredImage.SetPixel(x, y, filteredColor);
                }
            }

            return filteredImage;
        }*/




        /*private Bitmap ApplyConvolutionalFilter(Bitmap image, int[,] kernel)
        {
            int width = image.PixelWidth;
            int height = image.PixelHeight;
            int kernelSize = kernel.GetLength(0);
            int radius = kernelSize / 2;


            Bitmap filteredImage = new Bitmap(width, height, image.DpiX, image.DpiY, PixelFormats.Bgra32, null);


            image.Lock();
            filteredImage.Lock();

            int bytesPerPixel = 4;
            int stride = width * bytesPerPixel;


            byte[] imagePixels = new byte[height * stride];
            byte[] filteredPixels = new byte[height * stride];
            image.CopyPixels(imagePixels, stride, 0);
            filteredImage.CopyPixels(filteredPixels, stride, 0);


            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int red = 0, green = 0, blue = 0;
                    int totalWeight = 0;


                    for (int i = 0; i < kernelSize; i++)
                    {
                        for (int j = 0; j < kernelSize; j++)
                        {
                            int xOffset = x - radius + j;
                            int yOffset = y - radius + i;

                            if (xOffset >= 0 && xOffset < width && yOffset >= 0 && yOffset < height)
                            {

                                int pixelIndex = (yOffset * stride) + (xOffset * bytesPerPixel);


                                int pixelBlue = imagePixels[pixelIndex];
                                int pixelGreen = imagePixels[pixelIndex + 1];
                                int pixelRed = imagePixels[pixelIndex + 2];


                                red += pixelRed * kernel[i, j];
                                green += pixelGreen * kernel[i, j];
                                blue += pixelBlue * kernel[i, j];


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


                    int filteredPixelIndex = (y * stride) + (x * bytesPerPixel);


                    filteredPixels[filteredPixelIndex] = (byte)blue;
                    filteredPixels[filteredPixelIndex + 1] = (byte)green;
                    filteredPixels[filteredPixelIndex + 2] = (byte)red;
                    filteredPixels[filteredPixelIndex + 3] = 255; // Alpha channel
                }
            }

            // Copy the filtered image data back to the writable bitmap
            filteredImage.WritePixels(new Int32Rect(0, 0, width, height), filteredPixels, stride, 0);

            // Unlock the bitmaps
            image.Unlock();
            filteredImage.Unlock();

            return filteredImage;
        }
*/


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


        private Bitmap ApplyConvolutionFilter(Bitmap image, int[,] kernel)
        {
            BitmapSource bitmapSource = ConvertBitmapToBitmapSource(image);
            BitmapSource filteredBitmapSource = ApplyConvolutionFilter(bitmapSource, kernel);
            return ConvertBitmapSourceToBitmap(filteredBitmapSource);
        }

        /*private BitmapSource ApplyConvolutionFilter(BitmapSource original, int[,] kernel)
        {
            int width = original.PixelWidth;
            int height = original.PixelHeight;

            WriteableBitmap resultBitmap = new WriteableBitmap(original);
            Int32Rect rect = new Int32Rect(0, 0, width, height);
            byte[] pixels = new byte[width * height * 4]; // 4 channels (RGBA)

            original.CopyPixels(rect, pixels, width * 4, 0);

            resultBitmap.Lock();

            for (int y = 1; y < height - 1; y++)
            {
                for (int x = 1; x < width - 1; x++)
                {
                    int[] colorSum = { 0, 0, 0 };

                    for (int ky = -1; ky <= 1; ky++)
                    {
                        for (int kx = -1; kx <= 1; kx++)
                        {
                            int index = ((y + ky) * width + (x + kx)) * 4;
                            colorSum[0] += pixels[index] * kernel[ky + 1, kx + 1];
                            colorSum[1] += pixels[index + 1] * kernel[ky + 1, kx + 1];
                            colorSum[2] += pixels[index + 2] * kernel[ky + 1, kx + 1];
                        }
                    }

                    int resultIndex = ((y - 1) * (width - 2) + (x - 1)) * 4;
                    resultBitmap.WritePixels(new Int32Rect(x - 1, y - 1, 1, 1), new byte[] { (byte)(colorSum[0] / 9), (byte)(colorSum[1] / 9), (byte)(colorSum[2] / 9), 255 }, 4, 0);
                }
            }

            resultBitmap.Unlock();

            return resultBitmap;
        }*/

        /* private BitmapSource ApplyConvolutionFilter(BitmapSource original, int[,] kernel)
         {
             int width = original.PixelWidth;
             int height = original.PixelHeight;
             int totalweight = 0;

             WriteableBitmap resultBitmap = new WriteableBitmap(original);
             Int32Rect rect = new Int32Rect(0, 0, width, height);
             byte[] pixels = new byte[width * height * 4]; // 4 channels (RGBA)

             original.CopyPixels(rect, pixels, width * 4, 0);

             resultBitmap.Lock();

             for (int y = 0; y < height; y++)
             {
                 for (int x = 0; x < width; x++)
                 {
                     int[] colorSum = { 0, 0, 0 };

                     for (int ky = -1; ky <= 1; ky++)
                     {
                         for (int kx = -1; kx <= 1; kx++)
                         {
                             // Handle pixels at the edge by mirroring
                             int offsetX = Math.Max(0, Math.Min(width - 1, x + kx));
                             int offsetY = Math.Max(0, Math.Min(height - 1, y + ky));

                             int index = (offsetY * width + offsetX) * 4;
                             colorSum[0] += pixels[index] * kernel[ky + 1, kx + 1];
                             colorSum[1] += pixels[index + 1] * kernel[ky + 1, kx + 1];
                             colorSum[2] += pixels[index + 2] * kernel[ky + 1, kx + 1];
                             totalweight += kernel[ky + 1, kx + 1];
                         }
                     }

                     // Clip RGB values to [0, 255] range
                     byte red = (byte)Math.Min(255, Math.Max(0, colorSum[0]));
                     byte green = (byte)Math.Min(255, Math.Max(0, colorSum[1]));
                     byte blue = (byte)Math.Min(255, Math.Max(0, colorSum[2]));

                     int resultIndex = (y * width + x) * 4;
                     resultBitmap.WritePixels(new Int32Rect(x, y, 1, 1), new byte[] { red, green, blue, 255 }, 4, 0);
                 }
             }

             resultBitmap.Unlock();

             return resultBitmap;
         }
 */

        private BitmapSource ApplyConvolutionFilter(BitmapSource original, int[,] kernel)
        {
            int width = original.PixelWidth;
            int height = original.PixelHeight;
            int totalWeight = 0;

            WriteableBitmap resultBitmap = new WriteableBitmap(original);
            Int32Rect rect = new Int32Rect(0, 0, width, height);
            byte[] pixels = new byte[width * height * 4]; // 4 channels (RGBA)

            original.CopyPixels(rect, pixels, width * 4, 0);

            resultBitmap.Lock();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int[] colorSum = { 0, 0, 0 };

                    for (int ky = -1; ky <= 1; ky++)
                    {
                        for (int kx = -1; kx <= 1; kx++)
                        {
                            
                            int offsetX = Math.Max(0, Math.Min(width - 1, x + kx));
                            int offsetY = Math.Max(0, Math.Min(height - 1, y + ky));

                            int index = (offsetY * width + offsetX) * 4;
                            colorSum[0] += pixels[index] * kernel[ky + 1, kx + 1];
                            colorSum[1] += pixels[index + 1] * kernel[ky + 1, kx + 1];
                            colorSum[2] += pixels[index + 2] * kernel[ky + 1, kx + 1];
                            totalWeight += kernel[ky + 1, kx + 1];
                        }
                    }

                    
                    if (totalWeight > 0)
                    {
                        colorSum[0] /= totalWeight;
                        colorSum[1] /= totalWeight;
                        colorSum[2] /= totalWeight;
                    }

                    
                    byte red = (byte)Math.Min(255, Math.Max(0, colorSum[0]));
                    byte green = (byte)Math.Min(255, Math.Max(0, colorSum[1]));
                    byte blue = (byte)Math.Min(255, Math.Max(0, colorSum[2]));

                    int resultIndex = (y * width + x) * 4;
                    resultBitmap.WritePixels(new Int32Rect(x, y, 1, 1), new byte[] { red, green, blue, 255 }, 4, 0);

                    
                    totalWeight = 0;
                }
            }

            resultBitmap.Unlock();

            return resultBitmap;
        }



        private Bitmap ApplyBlurFilter(Bitmap image)
        {
            return ApplyConvolutionFilter(image, KernelBlur);
        }

        private Bitmap ApplyGaussianBlurFilter(Bitmap image)
        {
            return ApplyConvolutionFilter(image, KernelGaussianBlur);
        }

        private Bitmap ApplySharpenFilter(Bitmap image)
        {
            return ApplyConvolutionFilter(image, KernelSharpen);
        }

        private Bitmap ApplyEmbossFilter(Bitmap image)
        {
            return ApplyConvolutionFilter(image, KernelEmbossSouth);
        }
        private Bitmap ApplyEdgeDetectionFilter(Bitmap image)
        {
            return ApplyConvolutionFilter(image, KernelEdgeDetection);
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
                    return ApplyBlurFilter(image);
                case "Gaussian Blur":
                    return ApplyGaussianBlurFilter(image);
                case "Sharpen":
                    return ApplySharpenFilter(image);
                case "Emboss":
                    return ApplyEmbossFilter(image);
                case "Edge Detection":
                    return ApplyEdgeDetectionFilter(image);
                default:
                    return image;
            }
        }


        private void DisplayImage(Bitmap image, bool isOriginal)
        {
            if (isOriginal)
            {
                OriginalImageDisplay.Source = ConvertBitmapToBitmapImage(image);
            }
            else
            {
                FilteredImageDisplay.Source = ConvertBitmapToBitmapImage(image);
            }
        }

        private BitmapImage ConvertBitmapToBitmapImage(Bitmap bitmap)
        {
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            MemoryStream memoryStream = new MemoryStream();
            bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Bmp);
            memoryStream.Seek(0, SeekOrigin.Begin);
            bitmapImage.StreamSource = memoryStream;
            bitmapImage.EndInit();
            return bitmapImage;
        }

        private BitmapSource ConvertBitmapToBitmapSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Bmp);
                memory.Position = 0;

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }

        private Bitmap ConvertBitmapSourceToBitmap(BitmapSource bitmapSource)
        {
            Bitmap bitmap;
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                encoder.Save(outStream);
                bitmap = new Bitmap(outStream);
            }
            return new Bitmap(bitmap);
        }
    }


}