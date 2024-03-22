using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_Filtering
{
    internal class MedianCut
    {

        public class MedianCutColorQuantization
        {
            public static Bitmap ApplyMedianCutColorQuantization(Bitmap image, int paletteSize)
            {
               
                List<Color> palette = BuildPalette(image, paletteSize);

                
                Bitmap quantizedImage = new Bitmap(image.Width, image.Height);
                for (int x = 0; x < image.Width; x++)
                {
                    for (int y = 0; y < image.Height; y++)
                    {
                        Color originalColor = image.GetPixel(x, y);
                        Color closestColor = FindClosestColor(originalColor, palette);
                        quantizedImage.SetPixel(x, y, closestColor);
                    }
                }

                return quantizedImage;
            }

            private static List<Color> BuildPalette(Bitmap image, int paletteSize)
            {

                List<Color> colors = new List<Color>();
                for (int x = 0; x < image.Width; x++)
                {
                    for (int y = 0; y < image.Height; y++)
                    {
                        colors.Add(image.GetPixel(x, y));
                    }
                }


                ColorCube initialCube = new ColorCube(colors);


                List<ColorCube> cubes = new List<ColorCube> { initialCube };
                while (cubes.Count < paletteSize)
                {
                    ColorCube cubeToSplit = cubes.First();
                    List<ColorCube> splitCubes = cubeToSplit.Split();
                    cubes.RemoveAt(0);
                    for(int i = 0;i < splitCubes.Count; i++)
                    {
                        cubes.Add(splitCubes[i]);
                    }

                }

                return cubes.Select(cube => cube.AverageColor).ToList();
            }
            

            private static Color FindClosestColor(Color target, List<Color> palette)
            {
                double minDistanceSquared = double.MaxValue;
                Color closestColor = Color.Black;

                foreach (Color color in palette)
                {
                    double distanceSquared = Math.Pow(color.R - target.R, 2) +
                                             Math.Pow(color.G - target.G, 2) +
                                             Math.Pow(color.B - target.B, 2);
                    if (distanceSquared < minDistanceSquared)
                    {
                        minDistanceSquared = distanceSquared;
                        closestColor = color;
                    }
                }

                return closestColor;
            }
        }

        public class ColorCube
        {
            public List<Color> Colors { get; private set; }
            public Color AverageColor => CalculateAverageColor();
            public int Size => Colors.Count;

            public ColorCube(List<Color> colors)
            {
                Colors = colors;
            }

            public List<ColorCube> Split()
            {

                int maxDimension = Math.Max(
                    Colors.Max(color => color.R) - Colors.Min(color => color.R),
                    Math.Max(
                        Colors.Max(color => color.G) - Colors.Min(color => color.G),
                        Colors.Max(color => color.B) - Colors.Min(color => color.B)
                    )
                );


                int splitChannel = 0;
                if (maxDimension == Colors.Max(color => color.R) - Colors.Min(color => color.R))
                {
                    splitChannel = 0;
                }
                else if (maxDimension == Colors.Max(color => color.G) - Colors.Min(color => color.G))
                {
                    splitChannel = 1;
                }
                else if (maxDimension == Colors.Max(color => color.B) - Colors.Min(color => color.B))
                {
                    splitChannel = 2;
                }


                List<Color> sortedColors = Colors.OrderBy(color => GetColorChannel(color, splitChannel)).ToList();


                int midIndex = sortedColors.Count / 2;
                List<Color> firstHalf = sortedColors.GetRange(0, midIndex);
                List<Color> secondHalf = sortedColors.GetRange(midIndex, sortedColors.Count - midIndex);


                List<ColorCube> newCubes = new List<ColorCube>
                {
                     new ColorCube(firstHalf),
                     new ColorCube(secondHalf)
                };

                return newCubes;
            }

           
            private int GetColorChannel(Color color, int channel)
            {
                switch (channel)
                {
                    case 0: return color.R;
                    case 1: return color.G;
                    case 2: return color.B;
                    default: throw new ArgumentException("Invalid color channel.");
                }
            }

            private Color CalculateAverageColor()
            {
                int sumR = 0, sumG = 0, sumB = 0;
                foreach (Color color in Colors)
                {
                    sumR += color.R;
                    sumG += color.G;
                    sumB += color.B;
                }

                int avgR = sumR / Colors.Count;
                int avgG = sumG / Colors.Count;
                int avgB = sumB / Colors.Count;

                return Color.FromArgb(avgR, avgG, avgB);
            }
        }
    }
}


/*public List<ColorCube> Split()
           {
               // Determine the maximum range (difference between maximum and minimum) along each color channel
               int maxDimension = Math.Max(
                   Colors.Max(color => color.R) - Colors.Min(color => color.R),
                   Math.Max(
                       Colors.Max(color => color.G) - Colors.Min(color => color.G),
                       Colors.Max(color => color.B) - Colors.Min(color => color.B)
                   )
               );

               // Determine which color channel has the maximum range (longest dimension)
               int splitChannel = 0; // Default to red channel
               if (maxDimension == Colors.Max(color => color.R) - Colors.Min(color => color.R))
               {
                   splitChannel = 0; // Red channel
               }
               else if (maxDimension == Colors.Max(color => color.G) - Colors.Min(color => color.G))
               {
                   splitChannel = 1; // Green channel
               }
               else if (maxDimension == Colors.Max(color => color.B) - Colors.Min(color => color.B))
               {
                   splitChannel = 2; // Blue channel
               }

               // Sort colors based on the selected channel
               List<Color> sortedColors = Colors.OrderBy(color => GetColorChannel(color, splitChannel)).ToList();

               // Calculate the index to split the sorted colors into two equal parts
               int midIndex = sortedColors.Count / 2;

               // Create two new color cubes representing the two halves of the split
               List<ColorCube> newCubes = new List<ColorCube>
               {
                   new ColorCube(sortedColors.GetRange(0, midIndex)), // First half
                   new ColorCube(sortedColors.GetRange(midIndex, sortedColors.Count - midIndex)) // Second half
               };

               // Remove the current cube and add the new cubes' colors to the end of the list
               Colors.Clear();
               foreach (ColorCube cube in newCubes)
               {
                   Colors.AddRange(cube.Colors);
               }

               return newCubes;
           }*/


/* public List<ColorCube> Split()
 {

     List<Color> sortedColors = Colors.OrderBy(color => color.R).ToList();


     int midIndex = sortedColors.Count / 2;


     if (sortedColors.Count % 2 != 0)
     {

         midIndex++;
     }


     List<Color> firstHalf = sortedColors.GetRange(0, midIndex);

     List<Color> secondHalf = sortedColors.GetRange(midIndex, sortedColors.Count - midIndex);


     List<ColorCube> newCubes = new List<ColorCube>
     {
         new ColorCube(firstHalf), 
         new ColorCube(secondHalf)
     };


     return newCubes;
 }*/