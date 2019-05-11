using PlateNumberRecognition.DAL.Models;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace PlateNumberRecognition.OCR.BLL
{
    public static class DataSet
    {
        public static void SetNewData()
        {
            try
            {
                Queries query;
                foreach (var pathToFile in Directory.EnumerateFiles(@"D:\symbols", "*", SearchOption.AllDirectories))
                {
                    string fileName = Path.GetFileName(pathToFile);
                    var list = NGramStrategy.Run(fileName.Replace("_", " "), 1);
                    var symbol = NGramStrategy.Run(pathToFile.Replace("\\", " "), 1);
                    Bitmap bitmap = new Bitmap(pathToFile);
                    var image = ImageToBinary(bitmap);
                    query = new Queries();
                    var insert = new Digits()
                    {
                        InputVector = ImageToBinary(bitmap),
                        OutputVector = list[0]
                    };
                    query.InsertDigits(insert);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        public static void SetNewDataV2()
        {
            try
            {
                Queries query;
                foreach (var pathToFile in Directory.EnumerateFiles(@"D:\symbols", "*", SearchOption.AllDirectories))
                {
                    string fileName = Path.GetFileName(pathToFile);
                    var list = NGramStrategy.Run(fileName.Replace("_", " "), 1);
                    var symbol = NGramStrategy.Run(pathToFile.Replace("\\", " "), 1);
                    Bitmap bitmap = new Bitmap(pathToFile);
                    var image = ImageToBinary(bitmap);
                    query = new Queries();
                    var insert = new Letters()
                    {
                        InputVector = ImageToBinary(bitmap),
                        OutputVector = list[0]
                    };
                    query.InsertLetters(insert);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        public static string ImageToBinary(Bitmap img)
        {
            string texto = String.Empty;
            try
            {
                for (int i = 0; i < img.Height; i++)
                {
                    for (int j = 0; j < img.Width; j++)
                    {
                        texto = (img.GetPixel(j, i).B > 130 &&
                                 img.GetPixel(j, i).G > 130 && img.GetPixel(j, i).R > 130)
                                 ? texto + "0" : texto + "1";
                    }
                    texto = texto + "\n"; // this is to make the enter between lines   (\r\n)
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            return texto;
        }
        public static string GetBase64StringForImage(string imgPath)
        {
            byte[] imageBytes = File.ReadAllBytes(imgPath);
            string base64String = Convert.ToBase64String(imageBytes);
            return base64String;
        }

        public static Bitmap Base64StringToBitmap(string base64String)
        {
            Bitmap bmpReturn = null;
            byte[] byteBuffer = Convert.FromBase64String(base64String);
            MemoryStream memoryStream = new MemoryStream(byteBuffer);
            memoryStream.Position = 0;
            bmpReturn = (Bitmap)Bitmap.FromStream(memoryStream);
            memoryStream.Close();
            memoryStream = null;
            byteBuffer = null;
            return bmpReturn;
        }
    }
}

