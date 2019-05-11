using System.Drawing;

namespace PlateNumberRecognition.Vision.Logic.Interfaces
{
    public interface IApproximator
    {
        IMonomap Approximate(Bitmap image);
    }
}
