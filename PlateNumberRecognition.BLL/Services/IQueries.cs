using PlateNumberRecognition.DAL.Models;
using System.Collections.Generic;

namespace PlateNumberRecognition.BLL.Services
{
    public interface IQueries
    {
        List<Numbers> GetNumbersFromDB();
        void UpdateResultToDB(int id, string result);
    }
}
