namespace PlateNumberRecognition.OCR.DAL.Models
{
    public class NumbersModel
    {
        public int ID { get; set; }
        public string Style { get; set; }
        public string Font { get; set; }
        public uint Size { get; set; }
        public string InputVector { get; set; }
        public string OutputVector { get; set; }
    }
}
