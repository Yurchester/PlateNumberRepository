using System;
using System.Drawing;

namespace PlateNumberRecognition.Processing
{
    public class PreProcessing
    {
        public static Bitmap Run(Bitmap image)
        {
            Console.WriteLine("...Наложение фильтров на изображение");
            var gray = new DataPreProcessing().Setgrayscale_filter(image);
            var bmp24 = new DataPreProcessing().Bitmap24bppRgb(gray);
            //// var equalization = new DataPreProcessing().SetHistogramEqualizationfilter(bmp24);
            // var gamma = new DataPreProcessing().SetGammaCorrection(bmp24, 1.1f);
            // var brigthness = new DataPreProcessing().SetBrightnessCorrection(gamma, 25); //-50
            //var contrast = new DataPreProcessing().SetContrastCorrection(bmp24, 100);
            var contrast2 = new DataPreProcessing().SetContrastStretch(bmp24);
            var scaledbitmap = new DataPreProcessing().ScaleByPercent(contrast2, 100);
            var gaussian = new DataPreProcessing().SetGaussianSharpen(scaledbitmap);

            return new DataPreProcessing().SetBradleyThresholding(gaussian);
        }
    }
}

