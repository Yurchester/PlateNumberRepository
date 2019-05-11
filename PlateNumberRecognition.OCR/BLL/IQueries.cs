using PlateNumberRecognition.DAL.Models;
using PlateNumberRecognition.OCR.DAL.Models;
using System.Collections.Generic;

namespace PlateNumberRecognition.OCR.BLL
{
    public interface IQueries
    {
        //void InsertPicturesToDB(NumbersModel model);
        List<Digits> GetDigitsFromDB();
        List<Letters> GetLettersFromDB();
    }
}
