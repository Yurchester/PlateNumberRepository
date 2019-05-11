using PlateNumberRecognition.OCR.BLL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PlateNumberRecognition.OCR.NeuralNetwork
{
    public static class DataManager
    {
        public static Dictionary<int, string> dictionary = new Dictionary<int, string>();
        static DataManager()
        {
            dictionary.Add(0, "а"); dictionary.Add(1, "б"); dictionary.Add(2, "в"); dictionary.Add(3, "г"); dictionary.Add(4, "д");
            dictionary.Add(5, "е"); dictionary.Add(6, "ж"); dictionary.Add(7, "з"); dictionary.Add(8, "и"); dictionary.Add(9, "к");
            dictionary.Add(10, "л"); dictionary.Add(11, "м"); dictionary.Add(12, "н"); dictionary.Add(13, "о"); dictionary.Add(14, "п");
            dictionary.Add(15, "р"); dictionary.Add(16, "с"); dictionary.Add(17, "т"); dictionary.Add(18, "у"); dictionary.Add(19, "ф");
            dictionary.Add(20, "х"); dictionary.Add(21, "ц"); dictionary.Add(22, "ч"); dictionary.Add(23, "ш"); dictionary.Add(24, "щ");
            dictionary.Add(25, "ъ"); dictionary.Add(26, "ь"); dictionary.Add(27, "э"); dictionary.Add(28, "ю"); dictionary.Add(29, "я");
        }
        public static double[][] LoadDigits(out double[][] outputs)
        {
            List<double[]> list = new List<double[]>();
            List<double[]> output = new List<double[]>();

            List<double> row = new List<double>();
            Console.WriteLine("...Загрузка векторных данных");
            Console.WriteLine("...Построение модели для распознания цифр");
            foreach (var vector in new Queries().GetDigitsFromDB())
            {
                foreach (var ch in vector.InputVector)
                {
                    if (ch != ' ' && ch != '\n')
                    {
                        row.Add(Double.Parse(ch.ToString()));
                    }
                }
                list.Add(row.ToArray());
                row = new List<double>();

                output.Add(FormatOutputVector(Double.Parse(vector.OutputVector)));
            }
            outputs = output.ToArray();

            return list.ToArray();
        }

        public static double[][] LoadLetters(out double[][] outputs)
        {
            List<double[]> list = new List<double[]>();
            List<double[]> output = new List<double[]>();
            List<double> row = new List<double>();
            Console.WriteLine("\n...Загрузка векторных данных");
            Console.WriteLine("...Построение модели для распознания букв");
            foreach (var vector in new Queries().GetLettersFromDB())
            {
                foreach (var ch in vector.InputVector)
                {
                    if (ch != ' ' && ch != '\n')
                    {
                        row.Add(Double.Parse(ch.ToString()));
                    }
                }
                list.Add(row.ToArray());
                row = new List<double>();

                output.Add(FormatOutputLettersVector(vector.OutputVector));
            }
            outputs = output.ToArray();

            return list.ToArray();
        }

        #region Utility Methods

        /// <summary>
        /// Converts a numeric output label (0, 1, 2, 3, etc) to its cooresponding array of doubles, where all values are 0 except for the index matching the label (ie., if the label is 2, the output is [0, 0, 1, 0, 0, ...]).
        /// </summary>
        /// <param name="label">double</param>
        /// <returns>double[]</returns>
        public static double[] FormatOutputVector(double label)
        {
            double[] output = new double[10];

            for (int i = 0; i < output.Length; i++)
            {
                if (i == label)
                {
                    output[i] = 1;
                }
                else
                {
                    output[i] = 0;
                }
            }

            return output;
        }
        public static double[] FormatOutputLettersVector(string label)
        {
            //string[] dictionary = new string[30] { "а", "б", "в", "г", "д", "е", "ж", "з", "и", "к", "л", "м", "н", "о", "п", "р", "с", "т", "у", "ф", "х", "ц", "ч", "ш", "щ", "ъ", "ь", "э", "ю", "я" };

            double[] output = new double[30];

            for (int i = 0; i < dictionary.Count; i++)
            {
                foreach (var item in dictionary)
                {
                    if (item.Value == label.ToLower())
                    {
                        output[item.Key] = 1;
                    }
                    else
                    {
                        output[i] = 0;
                    }
                }
            }

            return output;
        }


        /// <summary>
        /// Finds the largest output value in an array and returns its index. This allows for sequential classification from the outputs of a neural network (ie., if output at index 2 is the largest, the classification is class "3" (zero-based)).
        /// </summary>
        /// <param name="output">double[]</param>
        /// <returns>double</returns>
        public static double FormatOutputResult(double[] output)
        {
            return output.ToList().IndexOf(output.Max());
        }

        public static double[][] LoadTest(string testData, out double[][] outputs)
        {
            List<double[]> list = new List<double[]>();
            List<double[]> output = new List<double[]>();
            List<double> row = new List<double>();

            foreach (var vector in testData)
            {
                if (vector != ' ' && vector != '\n')
                {
                    row.Add(Double.Parse(vector.ToString()));
                }
            }
            list.Add(row.ToArray());
            row = new List<double>();
            outputs = output.ToArray();
            // output.Add(new double[10]);
            // Return inputs;
            return list.ToArray();
        }
        #endregion
    }
}


