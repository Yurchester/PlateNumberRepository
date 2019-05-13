using MySql.Data.MySqlClient;
using PlateNumberRecognition.OCR.NeuralNetwork;
using System;
using System.Drawing;
using System.Linq;

namespace PlateNumberRecognition.OCR
{
    public class OCR
    {
        public static MySqlConnection Connection;
        public OCR(MySqlConnection _conn)
        {
            Connection = _conn;
        }

        [Obsolete]
        public static void Run()
        {
            //DataSet.SetNewData();
            //DataSet.SetNewDataV2();
            RNN.RunModelForDigits();
            RNN.RunModelForLetters();
        }
        public static string Process(Bitmap image)
        {
            //Console.WriteLine("...Запуск модели распознания цифр");
            var digitsArray = RNN.DigitsRecognition(image);
            var digitsValue = digitsArray.ToList().Max();
            //Console.WriteLine("...Запуск модели для распознания букв");
            var lettersArray = RNN.LettersRecognition(image);
            var lettersValue = lettersArray.ToList().Max();
            var resString = String.Empty;
            if (
                (digitsValue >= 0.99 && lettersValue >= 0.99) ||
                (Math.Round(Convert.ToDecimal(digitsValue), 2) == Math.Round(Convert.ToDecimal(lettersValue), 2))
               )
            {
                resString += RNN.RunForDigits(digitsArray, digitsValue) + " | " + RNN.RunForLetters(lettersArray, lettersValue);
            }
            else
            {
                if (digitsValue > lettersValue && digitsValue > 0.9)
                    resString = RNN.RunForDigits(digitsArray, digitsValue);
                else if (digitsValue < lettersValue && lettersValue > 0.9)
                    resString = RNN.RunForLetters(lettersArray, lettersValue);
            }
            //if (digitsValue > 0.9)
            //resString = RNN.RunForDigits(digitsArray, digitsValue);
            return resString;
        }
    }
}
