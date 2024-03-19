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


        private static Bitmap ApplyAverageDitheringToColor(Bitmap originalImage, int splits)
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

                        if (intensity >= i * (range) && intensity < (i + 1) * (range + 1))
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
                    int rangeIndex = Math.Min(intensity / (range + 1), splits - 2); // Ensure rangeIndex is within bounds

                    // Assign the new intensity based on the threshold
                    int newIntensity = intensity <= thresholds[rangeIndex] ? rangeIndex * (range) : (rangeIndex + 1) * (range);

                    // Set the new pixel color
                    Color newPixel = Color.FromArgb(newIntensity, newIntensity, newIntensity);
                    ditheredImage.SetPixel(x, y, newPixel);
                }
            }

            return ditheredImage;
        }
       





    }
}
