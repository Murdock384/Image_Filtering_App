using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Drawing.Imaging;
using System.IO;

namespace Image_Filtering
{
    internal class Filters
    {
        public static int[,] KernelBlur = {
            { 1, 1, 1 },
            { 1, 1, 1 },
            { 1, 1, 1 }
        };

        public static int[,] KernelGaussianBlur = {
            { 0, 1, 0 },
            { 1, 4, 1 },
            { 0, 1, 0 }
        };

        public static int[,] KernelSharpen = {
            { -1, -1, -1 },
            { -1, 9, -1 },
            { -1, -1, -1 }
        };

        public static int[,] KernelEdgeDetection = {
            { -1, -1, -1 },
            { -1, 8, -1 },
            { -1, -1, -1 }
        };

        public static int[,] KernelEmbossSouth = {
            { -1, -1, -1 },
            { 0, 1, 0 },
            { 1, 1, 1 }
        };

        //Functional Filters
        public static Bitmap ApplyInversionFilter(Bitmap image)
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

        public static Bitmap ApplyBrightnessCorrectionFilter(Bitmap image, double brightness)
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

        public static Bitmap ApplyContrastEnhancementFilter(Bitmap image, double contrast)
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


        public static Bitmap ApplyGammaCorrectionFilter(Bitmap image, double gamma)
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

        //Convolutional Filters
        public static Bitmap ApplyConvolutionFilter(Bitmap image, int[,] kernel)
        {
            BitmapSource bitmapSource = ConvertBitmapToBitmapSource(image);
            BitmapSource filteredBitmapSource = ApplyConvolutionFilter(bitmapSource, kernel);
            return ConvertBitmapSourceToBitmap(filteredBitmapSource);
        }


        public static BitmapSource ApplyConvolutionFilter(BitmapSource original, int[,] kernel)
        {
            int width = original.PixelWidth;
            int height = original.PixelHeight;
            int totalWeight = 0;

            WriteableBitmap resultBitmap = new WriteableBitmap(original);
            Int32Rect rect = new Int32Rect(0, 0, width, height);
            byte[] pixels = new byte[width * height * 4];

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

        public static Bitmap ApplyBlurFilter(Bitmap image)
        {
            return ApplyConvolutionFilter(image, KernelBlur);
        }

        public static Bitmap ApplyGaussianBlurFilter(Bitmap image)
        {
            return ApplyConvolutionFilter(image, KernelGaussianBlur);
        }

        public static Bitmap ApplySharpenFilter(Bitmap image)
        {
            return ApplyConvolutionFilter(image, KernelSharpen);
        }

        public static Bitmap ApplyEmbossFilter(Bitmap image)
        {
            return ApplyConvolutionFilter(image, KernelEmbossSouth);
        }
        public static Bitmap ApplyEdgeDetectionFilter(Bitmap image)
        {
            return ApplyConvolutionFilter(image, KernelEdgeDetection);
        }

        private static BitmapSource ConvertBitmapToBitmapSource(Bitmap bitmap)
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

        private static Bitmap ConvertBitmapSourceToBitmap(BitmapSource bitmapSource)
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
        public static BitmapImage ConvertBitmapToBitmapImage(Bitmap bitmap)
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

        public static Bitmap ConvertToGrayscale(Bitmap image)
        {
            Bitmap grayscaleImage = new Bitmap(image.Width, image.Height);

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color pixelColor = image.GetPixel(x, y);
                    int grayValue = (int)(0.2126 * pixelColor.R + 0.7152 * pixelColor.G + 0.0722 * pixelColor.B);
                    Color grayColor = Color.FromArgb(grayValue, grayValue, grayValue);
                    grayscaleImage.SetPixel(x, y, grayColor);
                }
            }

            return grayscaleImage;
        }
        public static Bitmap ApplyMedianFilter(Bitmap image)
        {
            Bitmap filteredImage = new Bitmap(image.Width, image.Height);


            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {

                    int[] neighborhood = GetNeighborhoodValues(image, x, y);


                    Array.Sort(neighborhood);


                    int medianValue = neighborhood[neighborhood.Length / 2];


                    filteredImage.SetPixel(x, y, System.Drawing.Color.FromArgb(medianValue, medianValue, medianValue));
                }
            }

            return filteredImage;
        }
        private static int[] GetNeighborhoodValues(Bitmap image, int x, int y)
        {
            int[] values = new int[9];
            int index = 0;


            for (int j = -1; j <= 1; j++)
            {
                for (int i = -1; i <= 1; i++)
                {
                    int pixelX = x + i;
                    int pixelY = y + j;


                    pixelX = Math.Max(0, Math.Min(pixelX, image.Width - 1));
                    pixelY = Math.Max(0, Math.Min(pixelY, image.Height - 1));


                    System.Drawing.Color pixelColor = image.GetPixel(pixelX, pixelY);
                    int grayscaleValue = (int)(pixelColor.R * 0.3 + pixelColor.G * 0.59 + pixelColor.B * 0.11);


                    values[index++] = grayscaleValue;
                }
            }

            return values;
        }




    }
}
