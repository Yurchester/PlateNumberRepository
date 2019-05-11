using System.Drawing;

namespace PlateNumberRecognition.DAL.Models
{
    public class Numbers
    {
        public int ID { get; set; }
        public Bitmap Image { get; set; }
        public string Result { get; set; }
        public short Recognize { get; set; }
        public double Percent { get; set; }
        public string TimeToRecognize { get; set; }
    }
}
