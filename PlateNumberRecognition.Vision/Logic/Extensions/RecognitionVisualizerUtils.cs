using PlateNumberRecognition.Processing;
using PlateNumberRecognition.Vision.Logic.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;

namespace PlateNumberRecognition.Vision.Logic.Extensions
{
    public static class RecognitionVisualizerUtils
    {
        private const float Thickness = 1;

        private static readonly Pen KnownPen = new Pen(Color.LimeGreen, Thickness);

        private static readonly Pen AssumptionPen = new Pen(Color.Orange, Thickness);

        private static readonly Pen UnknownPen = new Pen(Color.Red, Thickness);

        private static readonly IDictionary<QState, Pen> DefaultMapping = new Dictionary<QState, Pen>
        {
            { QState.Ok, KnownPen },
            { QState.Assumptions, AssumptionPen },
            { QState.Unknown, UnknownPen }
        };

        /// <summary>
        /// Визуализировать результат распознания на <see cref="Bitmap"/>.
        /// </summary>
        /// <param name="bitmap">Ссылка на <see cref="Bitmap"/>.</param>
        /// <param name="report">Ссылка на результат распознавания.</param>
        public static Dictionary<SymbolDataModel, double> Visualize(Bitmap bitmap, QReport report)
        {
            List<SymbolDataModel> listOfSymbolData = new List<SymbolDataModel>();
            var thickness = Thickness;
            foreach (var symbol in report.Symbols)
            {
                try
                {
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        RectangleF cloneRect = new RectangleF(0, 0, bitmap.Width, bitmap.Height);
                        Bitmap cloneBitmap = bitmap.Clone(cloneRect, PixelFormat.Format24bppRgb);
                        g.DrawRectangle(DefaultMapping[symbol.State], symbol.StartPoint.X - thickness, symbol.StartPoint.Y - thickness, symbol.Width + thickness, symbol.Height + thickness);
                        bitmap.Save($"D:\\111.bmp");

                        listOfSymbolData.Add(new SymbolDataModel()
                        {
                            Image = CutSection(cloneBitmap, new Rectangle((int)(symbol.StartPoint.X), (int)(symbol.StartPoint.Y), (int)(symbol.Width), (int)(symbol.Height))),
                            Position = new Tuple<int, int>(symbol.StartPoint.X, symbol.StartPoint.Y),
                            Size = new Tuple<int, int>(symbol.Height, symbol.Width)
                        });
                    }
                }
                catch (Exception ex)
                {
                    continue;
                }
            }

            double maxHeightPercent = 100.0 / listOfSymbolData.Max(t => t.Size.Item1);
            double maxWidthPercent = 100.0 / listOfSymbolData.Max(t => t.Size.Item2);
            Dictionary<SymbolDataModel, double> dict = new Dictionary<SymbolDataModel, double>();
            foreach (var item in listOfSymbolData)
            {
                var heightPercent = item.Size.Item1 * maxHeightPercent;
                dict.Add(item, heightPercent);
            }
            Dictionary<SymbolDataModel, double> dicttemp = new Dictionary<SymbolDataModel, double>();
            dicttemp = dict;
            Dictionary<SymbolDataModel, double> dict2 = new Dictionary<SymbolDataModel, double>();
            foreach (var item in dict)
            {
                foreach (var item2 in dicttemp)
                {
                    if (item.Value > 4 && item2.Value > 4) //4 && 4
                    {
                        if (!dict2.ContainsKey(item2.Key))
                        {
                            var temp = (item.Value - item2.Value);
                            if (temp <= 7 && temp >= -7)
                            {
                                dict2.Add(item2.Key, item.Value);
                            }
                        }
                    }
                    //if ((uint)(item.Value - item2.Value) < 10 && !dict2.ContainsKey(item.Key) && item.Value > 5 && item2.Value > 5)
                    //{

                    //}
                }
            }

            return dict2;
        }

        static int i = 0;
        public static Bitmap CutSection(Bitmap image, Rectangle selection)
        {
            Bitmap bmp = image as Bitmap;
            if (bmp == null)
                throw new ArgumentException("No valid bitmap");

            Bitmap cropBmp = bmp.Clone(selection, bmp.PixelFormat);
            //  var temp = Recognition.ToRec(cropBmp);
            //cropBmp.Save($"D:\\symbols\\cropBmp_{i++}.bmp");

            if (CheckPixel(cropBmp))
            {
                var result = ResizeImage(cropBmp, 25, 25);
                //result.Save($"D:\\symbols\\res_{i++}.bmp");

                return result;
            }
            else
                return null;
        }
        public static void VisualizeV2(Bitmap bitmap, QReport report, string folder)
        {

            var thickness = Thickness;
            foreach (var symbol in report.Symbols)
            {
                try
                {
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        RectangleF cloneRect = new RectangleF(0, 0, bitmap.Width, bitmap.Height);
                        Bitmap cloneBitmap = bitmap.Clone(cloneRect, PixelFormat.Format24bppRgb);
                        CutSectionV2(folder, cloneBitmap, new Rectangle((int)(symbol.StartPoint.X), (int)(symbol.StartPoint.Y), (int)(symbol.Width), (int)(symbol.Height)));
                    }
                }
                catch (Exception ex)
                {
                    continue;
                }
            }
        }
        public static void CutSectionV2(string folder, Bitmap image, Rectangle selection)
        {
            Bitmap bmp = image as Bitmap;
            if (bmp == null)
                throw new ArgumentException("No valid bitmap");

            Bitmap cropBmp = bmp.Clone(selection, bmp.PixelFormat);
            var temp = PreProcessing.Run(cropBmp);
            image.Dispose();
            var result = ResizeImage(temp, 25, 25);
            result.Save($"D:\\symbols\\{folder}_{i++}.bmp");
        }
        public static bool CheckPixel(Bitmap bitmap)
        {
            bool flag = false;
            for (int i = 0; i < bitmap.Height; i++)
            {
                for (int j = 0; j < bitmap.Width; j++)
                {
                    if (
                        bitmap.GetPixel(j, i).B > 220 &&
                        bitmap.GetPixel(j, i).G > 220 &&
                        bitmap.GetPixel(j, i).R > 220)
                    {
                        bitmap.SetPixel(j, i, Color.White);
                        flag = true;
                        break;
                    }
                }
            }
            return flag;
        }
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
    }
}

