using MySql.Data.MySqlClient;
using PlateNumberRecognition.BLL.Services;
using PlateNumberRecognition.DAL.Connection;
using PlateNumberRecognition.OCR.BLL;
using PlateNumberRecognition.Processing;
using PlateNumberRecognition.Vision;
using PlateNumberRecognition.Vision.Logic.Engine;
using PlateNumberRecognition.Vision.Logic.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace PlateNumberRecognition.Engine
{
    public class Engine
    {
        private MySqlConnection _conn;
        private static readonly Dictionary<string, EulerContainer> _eulerContainersCache = new Dictionary<string, EulerContainer>();
        public static ObservableCollection<string> Languages { get; } = new ObservableCollection<string>();
        private static bool _flag = false;
        public Engine()
        {
            _conn = null;
        }
        [Obsolete]
        public void Run()
        {
            InitLanguages();
            Console.WriteLine("...Подключение к базе данных");
            _conn = DBMySQLUtils.GetDBConnection(new DBSettings());
            _conn.Open();
            Console.WriteLine("...Прогрев нейронной сети");
            new OCR.OCR(_conn);
            //NeuralNetwork.RunV2(_eulerContainersCache);
            //DataSet.SetNewData();
            //NeuralNetwork.RunV3(_eulerContainersCache);
            //DataSet.SetNewDataV2();
            OCR.OCR.Run();

            while (true)
            {
                RunAsync();
                Console.WriteLine("\n...Timeout 5 секунд");
                Thread.Sleep(5000);
            }
        }
        [Obsolete]
        public void RunAsync()
        {
            if (_conn.State.ToString() == "Open")
            {
                Console.WriteLine("\n...Подключение успешно...\n");

                Console.WriteLine("\n...Получение необработанных изображений"); //Нужно сделать
                var list = new BLL.Services.Queries(_conn).GetNumbersFromDB();

                if (list.Count > 0)
                {
                    foreach (var item in list)
                    {
                        Console.WriteLine("...Обработка изображения");
                        Bitmap processedImage = PreProcessing.Run(item.Image);

                        // Console.WriteLine("...Взятие нужных фреймов");
                        // NeuralNetwork.Run();//нужно сделать

                        Console.WriteLine("...Получение символов");

                        var listOfSymbols = NeuralNetwork.Run(processedImage, _eulerContainersCache).GroupBy(t => t.Value);
                        Console.WriteLine("...Запуск движка");
                        int i = 0;
                        List<Tuple<int, string>> templist = new List<Tuple<int, string>>();
                        foreach (var symbol in listOfSymbols)
                        {
                            int j = 0;
                            int count = symbol.Count();
                            string res = String.Empty;
                            foreach (var symbolItem in symbol.OrderBy(t => t.Key.Position.Item1))
                            {
                                j++;
                                var img = symbolItem.Key;
                                if (img.Image != null)
                                {
                                    //img.Image.Save($"D:\\symbols\\{img.Image}_{i++}.bmp");
                                    res += OCR.OCR.Process(img.Image);
                                    _flag = true;
                                }
                                if (j == count && !String.IsNullOrEmpty(res))
                                {
                                    templist.Add(new Tuple<int, string>(symbolItem.Key.Position.Item1, res));
                                }
                            }
                            //Console.WriteLine("result = " + res);
                        }
                        string StringRes = String.Empty;
                        //string StringCode = String.Empty;
                        //int k = 0; //для взятия первых двух значений, иначе бывают случаи 12.6.0.0
                        //if (templist.Count >= 1)
                        //{
                        //    foreach (var output in templist.OrderBy(t => t.Item1))
                        //    {
                        //        var codeCheck = GetRegex(output.Item2);
                        //        if (codeCheck.Length > 15)
                        //        {
                        //            foreach (var code in codeCheck)
                        //            {
                        //                if (code == 'з' || code == 'э')
                        //                    StringCode += 3.ToString();
                        //                else if (code == 'о')
                        //                    StringCode += 0.ToString();
                        //                else if (code == 'в')
                        //                    StringCode += 8.ToString();
                        //                else if (code == 'т')
                        //                    StringCode += 7.ToString();
                        //                else if (code == 'i')
                        //                    StringCode += 1.ToString();
                        //                else
                        //                    StringCode += code;
                        //            }
                        //        }
                        //    }
                        //    Console.WriteLine("code = " + StringCode);
                        //    //if (new Queries(_conn).UpdateCodeToDB(item.id, StringCode) == 1)
                        //    //{
                        //    //    ///
                        //    //}
                        //}

                        if (templist.Count >= 1)
                        {
                            foreach (var output in templist.OrderBy(t => t.Item1))
                            {
                                if (output.Item2.Length > 6)
                                {
                                    StringRes = output.Item2.ToUpper();
                                    break;
                                }
                            }
                            Console.WriteLine("\ncar number = " + StringRes.TrimEnd('.'));
                        }
                        if (_flag)
                        {
                            new BLL.Services.Queries(_conn).UpdateResultToDB(item.ID, StringRes);
                            _flag = false;
                        }
                    }
                }
                else
                {
                    Console.WriteLine("...Отсутствие данных");
                }

            }
            else
            {
                Console.WriteLine("...Ошибка подключения");
                Console.WriteLine("...Повторное подключение");
                _conn = DBMySQLUtils.GetDBConnection(new DBSettings());
                _conn.Open();
            }

            // await Task.Delay(5000);
        }
        public static string GetRegex(string text)
        {
            return Regex.Replace(text, @"[^\d]+", "");
        }
        private static void InitLanguages()
        {
            const string DictionaryName = "PlateNumberRecognition.Dics";
            const string DictionaryPath = "../../../../PlateNumberRecognition.Dics";
            var current = new DirectoryInfo(DictionaryPath);
            do
            {
                DirectoryInfo dicsDir = null;
                if (current.GetDirectories().Any(dic => (dicsDir = dic).Name == DictionaryName))
                {
                    foreach (var file in dicsDir.GetFiles("*.bin"))
                    {
                        var container = GetEulerContainer(file.FullName);
                        foreach (var lang in container.Languages)
                        {
                            _eulerContainersCache.Add(lang.LocalizationName, container);
                            Languages.Add(lang.LocalizationName);
                        }
                    }
                }

                current = current.Parent;
                if (current == null || !current.Exists)
                {
                    break;
                }
            } while (true);
        }
        private static EulerContainer GetEulerContainer(string path)
        {
            using (var file = File.Open(path, FileMode.Open))
            {
                return CompressionUtils.Decompress<EulerContainer>(file);
            }
        }
    }
}

