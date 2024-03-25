using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Image_Filtering
{
    internal class Dithering
    {

        public static Bitmap ApplyAverageDithering(Bitmap originalImage, int splits)
        {
            if (originalImage == null)
            {
                MessageBox.Show("Please open an image first.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }

            if (IsGrayscale(originalImage))
            {
                return ApplyAverageDitheringToGrayscale(originalImage, splits);
            }
            else
            {
                return ApplyAverageDitheringToColor(originalImage, splits);
            }
        }

        private static bool IsGrayscale(Bitmap image)
        {
            
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color pixelColor = image.GetPixel(x, y);
                    if (pixelColor.R != pixelColor.G || pixelColor.R != pixelColor.B)
                    {
                        return false; 
                    }
                }
            }
            return true;
        }


        /*private static Bitmap ApplyAverageDitheringToColor(Bitmap originalImage, int splits)
        {
            int range = 256 / splits;

            Bitmap ditheredImage = new Bitmap(originalImage.Width, originalImage.Height);

            for (int y = 0; y < originalImage.Height; y++)
            {
                for (int x = 0; x < originalImage.Width; x++)
                {
                    Color originalColor = originalImage.GetPixel(x, y);


                    int redIntensity = originalColor.R;
                    int redThreshold = 0;
                    for (int i = 0; i < splits; i++)
                    {
                        if (redIntensity <= redThreshold + range)
                        {
                            int redLevel = (255 * i) / (splits - 1);
                            originalColor = Color.FromArgb(originalColor.A, redLevel, originalColor.G, originalColor.B);
                            break;
                        }
                        redThreshold += range;
                    }


                    int greenIntensity = originalColor.G;
                    int greenThreshold = 0;
                    for (int i = 0; i < splits; i++)
                    {
                        if (greenIntensity <= greenThreshold + range)
                        {
                            int greenLevel = (255 * i) / (splits - 1);
                            originalColor = Color.FromArgb(originalColor.A, originalColor.R, greenLevel, originalColor.B);
                            break;
                        }
                        greenThreshold += range;
                    }


                    int blueIntensity = originalColor.B;
                    int blueThreshold = 0;
                    for (int i = 0; i < splits; i++)
                    {
                        if (blueIntensity <= blueThreshold + range)
                        {
                            int blueLevel = (255 * i) / (splits - 1);
                            originalColor = Color.FromArgb(originalColor.A, originalColor.R, originalColor.G, blueLevel);
                            break;
                        }
                        blueThreshold += range;
                    }

                    ditheredImage.SetPixel(x, y, originalColor);
                }
            }

            return ditheredImage;
        }*/


        public static Bitmap ApplyAverageDitheringToColor(Bitmap originalImage, int splits)
        {
            Bitmap ditheredImage = new Bitmap(originalImage.Width, originalImage.Height);
            int range = 255 / (splits - 1);

            // Compute the thresholds for each split for each color channel
            int[][] thresholds = new int[3][]; // Three color channels: Red, Green, Blue
            for (int channel = 0; channel < 3; channel++)
            {
                thresholds[channel] = new int[splits - 1];
                for (int i = 0; i < splits - 1; i++)
                {
                    int sum = 0;
                    int count = 0;
                    for (int y = 0; y < originalImage.Height; y++)
                    {
                        for (int x = 0; x < originalImage.Width; x++)
                        {
                            Color pixel = originalImage.GetPixel(x, y);
                            int intensity = 0;
                            switch (channel)
                            {
                                case 0:
                                    intensity = pixel.R;
                                    break;
                                case 1: 
                                    intensity = pixel.G;
                                    break;
                                case 2: 
                                    intensity = pixel.B;
                                    break;
                            }

                            if (intensity >= i * (range) && intensity < ((i + 1) * (range)))
                            {
                                sum += intensity;
                                count++;
                            }

                            if (intensity == 255 && i == splits - 2)
                            {
                                sum += intensity;
                                count++;
                            }
                        }
                    }
                    thresholds[channel][i] = count == 0 ? 0 : sum / count;
                }
            }

            // Apply dithering for each color channel
            for (int y = 0; y < originalImage.Height; y++)
            {
                for (int x = 0; x < originalImage.Width; x++)
                {
                    Color pixel = originalImage.GetPixel(x, y);

                   
                    int[] newIntensities = new int[3];
                    for (int channel = 0; channel < 3; channel++)
                    {
                        int intensity = 0;
                        switch (channel)
                        {
                            case 0: 
                                intensity = pixel.R;
                                break;
                            case 1: 
                                intensity = pixel.G;
                                break;
                            case 2:
                                intensity = pixel.B;
                                break;
                        }

                        // Find the range that the intensity belongs to for the current channel
                        int rangeIndex = Math.Min(intensity / (range + 1), splits - 2);

                       
                        int newIntensity = intensity <= thresholds[channel][rangeIndex] ? rangeIndex * range : (rangeIndex + 1) * range;

                        newIntensities[channel] = newIntensity;
                    }

                   
                    Color newPixel = Color.FromArgb(newIntensities[0], newIntensities[1], newIntensities[2]);
                    ditheredImage.SetPixel(x, y, newPixel);
                }
            }

            return ditheredImage;
        }
        public static Bitmap ApplyAverageDitheringToGrayscale(Bitmap originalImage, int splits)
        {
        
            Bitmap ditheredImage = new Bitmap(originalImage.Width, originalImage.Height);

        
            int range = 255 / (splits - 1);

            // Compute the thresholds for each split
            int[] thresholds = new int[splits - 1]; 
            for (int i = 0; i < splits - 1; i++)
            {

                int sum = 0;
                int count = 0;
                for (int y = 0; y < originalImage.Height; y++)
                {
                    for (int x = 0; x < originalImage.Width; x++)
                    {
                        Color pixel = originalImage.GetPixel(x, y);
                        int intensity = pixel.R;

                        if (intensity >= i * (range) && intensity < ((i + 1) * (range)))
                        {
                            sum += intensity;
                            count++;
                        }

                        if (intensity == 255 && i == splits - 2)
                        {
                            sum += intensity;
                            count++;
                        }
                    }
                }
                
                thresholds[i] = count == 0 ? 0 : sum / count;
            }

            // Apply dithering
            for (int y = 0; y < originalImage.Height; y++)
            {
                for (int x = 0; x < originalImage.Width; x++)
                {
                    Color pixel = originalImage.GetPixel(x, y);
                    int intensity = pixel.R;

                    // Find the range that the intensity belongs to
                    int rangeIndex = Math.Min(intensity / (range + 1), splits - 2); 

                    
                    int newIntensity = intensity <= thresholds[rangeIndex] ? rangeIndex * (range) : (rangeIndex + 1) * (range);

                    
                    Color newPixel = Color.FromArgb(newIntensity, newIntensity, newIntensity);
                    ditheredImage.SetPixel(x, y, newPixel);
                }
            }

            return ditheredImage;
        }

        public static Bitmap ConvertToYCbCr(Bitmap originalImage)
        {
            Bitmap yCbCrImage = new Bitmap(originalImage.Width, originalImage.Height);

            for (int y = 0; y < originalImage.Height; y++)
            {
                for (int x = 0; x < originalImage.Width; x++)
                {
                    Color pixel = originalImage.GetPixel(x, y);

                    int newY = (int)(0.299 * pixel.R + 0.587 * pixel.G + 0.114 * pixel.B);
                    int newCb = (int)(128 - 0.168736 * pixel.R - 0.331264 * pixel.G + 0.5 * pixel.B);
                    int newCr = (int)(128 + 0.5 * pixel.R - 0.418688 * pixel.G - 0.081312 * pixel.B);

                    newY = Math.Max(0, Math.Min(newY, 255));
                    newCb = Math.Max(0, Math.Min(newCb, 255));
                    newCr = Math.Max(0, Math.Min(newCr, 255));

                    Color newYCbCrPixel = Color.FromArgb(newY, newCb, newCr);
                    yCbCrImage.SetPixel(x, y, newYCbCrPixel);
                }
            }

            return yCbCrImage;
        }
        public static Bitmap ConvertToRGB(Bitmap yCbCrImage)
        {
            Bitmap rgbImage = new Bitmap(yCbCrImage.Width, yCbCrImage.Height);

            for (int y = 0; y < yCbCrImage.Height; y++)
            {
                for (int x = 0; x < yCbCrImage.Width; x++)
                {
                    Color pixel = yCbCrImage.GetPixel(x, y);

                    int Y = pixel.R;
                    int Cb = pixel.G;
                    int Cr = pixel.B;

                    int newR = (int)(Y + 1.402 * (Cr - 128));
                    int newG = (int)(Y - 0.344136 * (Cb - 128) - 0.714136 * (Cr - 128));
                    int newB = (int)(Y + 1.772 * (Cb - 128));

                    newR = Math.Max(0, Math.Min(newR, 255));
                    newG = Math.Max(0, Math.Min(newG, 255));
                    newB = Math.Max(0, Math.Min(newB, 255));

                    Color newRGBPixel = Color.FromArgb(newR, newG, newB);
                    rgbImage.SetPixel(x, y, newRGBPixel);
                }
            }

            return rgbImage;
        }

        public static Bitmap ApplyDitheringToYCbCr(Bitmap originalImage, int splits)
        {
            Bitmap ditheredImage = new Bitmap(originalImage.Width, originalImage.Height);
            int range = 255 / (splits - 1);

           
            Bitmap yCbCrImage = ConvertToYCbCr(originalImage);

           
            int[][] thresholds = new int[3][]; 
            for (int plane = 0; plane < 3; plane++)
            {
                thresholds[plane] = new int[splits - 1];
                for (int i = 0; i < splits - 1; i++)
                {
                    int sum = 0;
                    int count = 0;
                    for (int y = 0; y < yCbCrImage.Height; y++)
                    {
                        for (int x = 0; x < yCbCrImage.Width; x++)
                        {
                            Color pixel = yCbCrImage.GetPixel(x, y);
                            int intensity = 0;
                            switch (plane)
                            {
                                case 0:
                                    intensity = pixel.R;
                                    break;
                                case 1:
                                    intensity = pixel.G;
                                    break;
                                case 2:
                                    intensity = pixel.B;
                                    break;
                            }

                            if (intensity >= i * range && intensity < (i + 1) * (range + 1))
                            {
                                sum += intensity;
                                count++;
                            }
                        }
                    }
                    thresholds[plane][i] = count == 0 ? 0 : sum / count;
                }
            }

            
            for (int y = 0; y < yCbCrImage.Height; y++)
            {
                for (int x = 0; x < yCbCrImage.Width; x++)
                {
                    Color pixel = yCbCrImage.GetPixel(x, y);

                    int[] newIntensities = new int[3];
                    for (int plane = 0; plane < 3; plane++)
                    {
                        int intensity = 0;
                        switch (plane)
                        {
                            case 0:
                                intensity = pixel.R;
                                break;
                            case 1:
                                intensity = pixel.G;
                                break;
                            case 2:
                                intensity = pixel.B;
                                break;
                        }

                       
                        int rangeIndex = Math.Min(intensity / (range + 1), splits - 2);

                        int newIntensity = intensity <= thresholds[plane][rangeIndex] ? rangeIndex * range : (rangeIndex + 1) * range;

                        newIntensities[plane] = newIntensity;
                    }

                    
                    Color newPixel = Color.FromArgb(newIntensities[0], newIntensities[1], newIntensities[2]);
                    ditheredImage.SetPixel(x, y, newPixel);
                }
            }

            // Convert Y'CbCr image back to RGB
            ditheredImage = ConvertToRGB(ditheredImage);

            return ditheredImage;
        }

       




    }
}
