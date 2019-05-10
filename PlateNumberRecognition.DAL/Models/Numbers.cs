namespace PlateNumberRecognition.DAL.Models
{
    public class Numbers
    {
        public int ID { get; set; }
        public string Image { get; set; }
        public string Result { get; set; }
        public short Recognize { get; set; }
        public double Percent { get; set; }
        public string TimeToRecognize { get; set; }
    }
}
