using PlateNumberRecognition.Vision.Logic.Helpers;
using PlateNumberRecognition.Vision.Logic.Interfaces;
using System;
using System.Drawing;

namespace PlateNumberRecognition.Vision.Logic.Classes
{
    public class Approximator : IApproximator
    {
        public int Brightness { get; }
        public Approximator() : this(130) { }
        public Approximator(int brightness)
        {
            if (brightness < 0 || brightness > 255)
            {
                throw new ArgumentOutOfRangeException($"{nameof(brightness)} MinValue: 0 MaxValue: 240");
            }

            Brightness = brightness;
        }
        public IMonomap Approximate(Bitmap bitmap)
        {
            bool[,] booleanBitmap = new bool[bitmap.Width, bitmap.Height];
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    Color pixel = bitmap.GetPixel(x, y);
                    booleanBitmap[x, y] = GetBrightness(pixel) < 130;
                }
            }

            return new BitMonomap(booleanBitmap);
        }
        private static int GetBrightness(Color c)
        {
            return (int)Math.Sqrt(
            c.R * c.R * .241 +
            c.G * c.G * .691 +
            c.B * c.B * .068);
        }
    }
}
