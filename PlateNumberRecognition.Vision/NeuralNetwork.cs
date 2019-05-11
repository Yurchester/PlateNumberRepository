using PlateNumberRecognition.OCR.BLL;
using PlateNumberRecognition.Vision.Logic.Classes;
using PlateNumberRecognition.Vision.Logic.Engine;
using PlateNumberRecognition.Vision.Logic.Extensions;
using PlateNumberRecognition.Vision.Logic.Models;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace PlateNumberRecognition.Vision
{
    public class NeuralNetwork
    {
        public static Dictionary<SymbolDataModel, double> Run(Bitmap image, Dictionary<string, EulerContainer> _eulerContainersCache)
        {
            Approximator approximator = new Approximator();
            approximator.Approximate(image);
            var container = _eulerContainersCache["RU-ru"];
            TextRecognizer recognizer = new TextRecognizer(container);
            var report = recognizer.Recognize(image);
            var visualizedImage = new Bitmap(image);

            return RecognitionVisualizerUtils.Visualize(visualizedImage, report);
        }

        public static void RunV2(Dictionary<string, EulerContainer> _eulerContainersCache)
        {
            var container = _eulerContainersCache["RU-ru"];
            TextRecognizer recognizer = new TextRecognizer(container);

            foreach (var pathfile in Directory.EnumerateFiles(@"D:\PlateNumberRecognition\PlateNumberRecognition\PlateNumberRecognition.Generator\bin\Debug\netcoreapp3.0\BmpDebug", "*", SearchOption.AllDirectories))
            {

                try
                {
                    var symbol = NGramStrategy.Run(pathfile.Replace("\\", " "), 1);
                    if (symbol[7] != "1" ||
                        symbol[7] != "2" ||
                        symbol[7] != "3" ||
                        symbol[7] != "4" ||
                        symbol[7] != "5" ||
                        symbol[7] != "6" ||
                        symbol[7] != "7" ||
                        symbol[7] != "8" ||
                        symbol[7] != "9" ||
                        symbol[7] != "0"
                       )
                    {
                        Approximator approximator = new Approximator();
                        var _sourceBitmap = new Bitmap(pathfile);
                        approximator.Approximate(_sourceBitmap);
                        var report = recognizer.Recognize(_sourceBitmap);
                        // визуализируем найденные итоги
                        var visualizedImage = new Bitmap(_sourceBitmap);
                        //   RecognitionVisualizerUtils.Visualize(visualizedImage, report);
                        RecognitionVisualizerUtils.VisualizeV2(visualizedImage, report, symbol[7]);
                    }
                }
                catch
                {
                    continue;
                }
            }
        }
        public static void RunV3(Dictionary<string, EulerContainer> _eulerContainersCache)
        {
            var container = _eulerContainersCache["RU-ru"];
            TextRecognizer recognizer = new TextRecognizer(container);

            foreach (var pathfile in Directory.EnumerateFiles(@"D:\PlateNumberRecognition\PlateNumberRecognition\PlateNumberRecognition.Generator\bin\Debug\netcoreapp3.0\BmpDebug", "*", SearchOption.AllDirectories))
            {

                try
                {
                    var symbol = NGramStrategy.Run(pathfile.Replace("\\", " "), 1);
                    if (symbol[7] != "1" &&
                        symbol[7] != "2" &&
                        symbol[7] != "3" &&
                        symbol[7] != "4" &&
                        symbol[7] != "5" &&
                        symbol[7] != "6" &&
                        symbol[7] != "7" &&
                        symbol[7] != "8" &&
                        symbol[7] != "9" &&
                        symbol[7] != "0"
                       )
                    {
                        Approximator approximator = new Approximator();
                        var _sourceBitmap = new Bitmap(pathfile);
                        approximator.Approximate(_sourceBitmap);
                        var report = recognizer.Recognize(_sourceBitmap);
                        // визуализируем найденные итоги
                        var visualizedImage = new Bitmap(_sourceBitmap);
                        //   RecognitionVisualizerUtils.Visualize(visualizedImage, report);
                        RecognitionVisualizerUtils.VisualizeV2(visualizedImage, report, symbol[7]);
                    }
                }
                catch
                {
                    continue;
                }
            }
        }
    }
}
