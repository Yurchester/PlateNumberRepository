using System.Drawing;

namespace PlateNumberRecognition.Processing
{
    public interface IDataPreProcessing
    {
        Bitmap ChangingPixel(Bitmap bitmap);
        Bitmap SetGammaCorrection(Bitmap bitmap, float value);
        Bitmap SetBrightnessCorrection(Bitmap bitmap, int value);
        Bitmap SetContrastCorrection(Bitmap bitmap, int value);
        Bitmap Setgrayscale_filter(Bitmap bitmap);
        Bitmap SetContrastStretch(Bitmap bitmap);
        Bitmap SetErosion(Bitmap bitmap);
        Bitmap SetDilatation(Bitmap bitmap);
        Bitmap SetGaussianSharpen(Bitmap bitmap);
        Bitmap SetBradleyThresholding(Bitmap bitmap);
        //Bitmap SetOtsuThresholding(Bitmap bitmap);
        Bitmap ScaleByPercent(Bitmap bitmap, int Percent);
        Bitmap Bitmap24bppRgb(Bitmap bitmap);
        Bitmap Bitmap8bppIndexed(Bitmap bitmap);
        Bitmap CutSection(Bitmap image, Rectangle selection);
        //Bitmap SetBinaryThresholding(Bitmap bitmap);
        Bitmap SetHistogramEqualizationfilter(Bitmap bitmap);
    }
}
