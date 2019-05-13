using Accord.Math;
using Accord.Neuro;
using Accord.Neuro.Learning;
using Accord.Neuro.Networks;
using PlateNumberRecognition.OCR.BLL;
using System;
using System.Drawing;
using System.Linq;

namespace PlateNumberRecognition.OCR.NeuralNetwork
{
    public class RNN
    {
        private static DeepBeliefNetwork _networkForDigits;
        private static DeepBeliefNetwork _networkForLetters;
        [Obsolete]
        public static void RunModelForDigits()
        {
            double[][] inputsDigits;
            double[][] outputsDigits;

            inputsDigits = DataManager.LoadDigits(out outputsDigits);

            Console.WriteLine("...Процесс обучения глубокой нейронной сети для цифр\n");

            _networkForDigits = new DeepBeliefNetwork(inputsDigits.First().Length, 10);
            new GaussianWeights(_networkForDigits, 0.1).Randomize();
            _networkForDigits.UpdateVisibleWeights();

            //DeepBeliefNetworkLearning teacher = new DeepBeliefNetworkLearning(_networkForDigits)
            //{
            //    Algorithm = (h, v, i) => new ContrastiveDivergenceLearning(h, v)
            //    {
            //        LearningRate = 0.1,
            //        Momentum = 0.5,
            //        Decay = 0.001,
            //    }
            //};

            //int batchCount = Math.Max(1, inputsDigits.Length / 100);
            //int[] groups = Accord.Statistics.Tools.RandomGroups(inputsDigits.Length, batchCount);
            //double[][][] batches = inputsDigits.Subgroups(groups);
            double error = 0.0;

            var teacher2 = new BackPropagationLearning(_networkForDigits)
            {
                LearningRate = 0.1,
                Momentum = 0.5
            };

            for (int i = 0; i <= 400; i++)
            {
                error = teacher2.RunEpoch(inputsDigits, outputsDigits) / inputsDigits.Length;
                if (i % 10 == 0)
                {
                    Console.WriteLine(i + " Epoch, Error = " + error);
                }
            }
        }

        public static double[] DigitsRecognition(Bitmap image)
        {
            double[][] testDigitsInputs;
            double[][] testDigitsOutputs;
            testDigitsInputs = DataManager.LoadTest(DataSet.ImageToBinary(image), out testDigitsOutputs);
            double[] outputDigitsValues = _networkForDigits.Compute(testDigitsInputs.FirstOrDefault());
            int i = 0;
            Console.WriteLine("\n\n\n\n\n");
            foreach (var item in testDigitsInputs.ToList())
            {
                foreach (var item2 in item)
                {
                    i++;
                    Console.Write(item2);
                    if (i == 25)
                    {
                        Console.WriteLine();
                        i = 0;
                    }
                }
            }

            return outputDigitsValues;
        }
        public static string RunForDigits(double[] outputDigitsValues, double digitsValue)
        {
            return DataManager.FormatOutputResult(outputDigitsValues).ToString();
        }

        public static void RunModelForLetters()
        {
            double[][] inputsLetters;
            double[][] outputsLetters;

            inputsLetters = DataManager.LoadLetters(out outputsLetters);

            Console.WriteLine("...Процесс обучения глубокой нейронной сети для букв\n");

            _networkForLetters = new DeepBeliefNetwork(inputsLetters.First().Length, 30, 30);
            new GaussianWeights(_networkForLetters, 0.1).Randomize();
            _networkForLetters.UpdateVisibleWeights();
            DeepBeliefNetworkLearning teacher = new DeepBeliefNetworkLearning(_networkForLetters)
            {
                Algorithm = (h, v, i) => new ContrastiveDivergenceLearning(h, v)
                {
                    LearningRate = 0.1,
                    Momentum = 0.5,
                    Decay = 0.001,
                }
            };
            int batchCount = Math.Max(1, inputsLetters.Length / 100);
            int[] groups = Accord.Statistics.Tools.RandomGroups(inputsLetters.Length, batchCount);
            double[][][] batches = inputsLetters.Subgroups(groups);
            double[][][] layerData;
            double error = 0.0;
            //Console.WriteLine("...Запуск первого алгоритма обучения\n");
            //for (int layerIndex = 0; layerIndex < _networkForLetters.Machines.Count - 1; layerIndex++)
            //{
            //    teacher.LayerIndex = layerIndex;
            //    layerData = teacher.GetLayerInput(batches);
            //    for (int i = 0; i < 100; i++)
            //    {
            //        error = teacher.RunEpoch(layerData) / inputsLetters.Length;
            //        if (i % 10 == 0)
            //        {
            //              Console.WriteLine(i + ", Error = " + error);
            //        }
            //    }
            //}
            //Console.WriteLine("...Запуск второго алгоритма обучения\n");
            var teacher2 = new BackPropagationLearning(_networkForLetters)
            {
                LearningRate = 0.1,
                Momentum = 0.5
            };

            for (int i = 0; i <= 400; i++)
            {
                error = teacher2.RunEpoch(inputsLetters, outputsLetters) / inputsLetters.Length;
                if (i % 10 == 0)
                {
                    Console.WriteLine(i + " Epoch, Error = " + error);
                }
            }
        }
        public static double[] LettersRecognition(Bitmap image)
        {
            double[][] testLettersInputs;
            double[][] testLettersOutputs;
            testLettersInputs = DataManager.LoadTest(DataSet.ImageToBinary(image), out testLettersOutputs);
            double[] outputLettersValues = _networkForLetters.Compute(testLettersInputs.FirstOrDefault());

            return outputLettersValues;
        }
        public static string RunForLetters(double[] outputValues, double lettersValue)
        {
            var result = DataManager.FormatOutputResult(outputValues);
            var resString = String.Empty;
            foreach (var item in DataManager.dictionary)
            {
                if (item.Key == result)
                {
                    resString = item.Value;
                }
            }

            return resString;
        }
    }
}

