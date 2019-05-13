using PlateNumberRecognition.Processing;
using PlateNumberRecognition.Vision.Logic.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public static Dictionary<SymbolDataModel, Tuple<double, double>> Visualize(Bitmap bitmap, QReport report)
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
                        if (symbol.State == QState.Assumptions || symbol.State == QState.Ok)
                        {
                            listOfSymbolData.Add(new SymbolDataModel()
                            {
                                Image = CutSection(cloneBitmap, new Rectangle((int)(symbol.StartPoint.X), (int)(symbol.StartPoint.Y), (int)(symbol.Width), (int)(symbol.Height))),
                                Position = new Tuple<int, int>(symbol.StartPoint.X, symbol.StartPoint.Y),
                                Size = new Tuple<int, int>(symbol.Height, symbol.Width)
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    continue;
                }
            }

            double maxHeightPercent = 100.0 / listOfSymbolData.Max(t => t.Size.Item1);
            double maxWidthPercent = 100.0 / listOfSymbolData.Max(t => t.Size.Item2);
            double maxGeoYPercent = 100.0 / listOfSymbolData.Max(t => t.Position.Item2);
            Dictionary<SymbolDataModel, Tuple<double, double>> dict = new Dictionary<SymbolDataModel, Tuple<double, double>>();

            foreach (var item in listOfSymbolData)
            {
                var heightPercent = item.Size.Item1 * maxHeightPercent;
                var GeoYPercent = item.Position.Item2 * maxGeoYPercent;
                dict.Add(item, new Tuple<double, double>(heightPercent, GeoYPercent));
            }

            Dictionary<SymbolDataModel, Tuple<double, double>> tempDictionary = new Dictionary<SymbolDataModel, Tuple<double, double>>();
            tempDictionary = dict;
            Dictionary<SymbolDataModel, Tuple<double, double>> dataDictionary = new Dictionary<SymbolDataModel, Tuple<double, double>>();
            foreach (var item in dict)
            {
                foreach (var item2 in tempDictionary)
                {
                    if (item.Value.Item1 > 4 && item2.Value.Item1 > 4)
                    {
                        if (!dataDictionary.ContainsKey(item2.Key))
                        {
                            var temp = (item.Value.Item1 - item2.Value.Item1);
                            var geoY = (item.Value.Item2 - item2.Value.Item2);

                            if ((temp <= 7 && temp >= -7) || geoY <= 7 && geoY >= -7)
                            {
                                if (geoY <= 7 && geoY >= -7)
                                {
                                    dataDictionary.Add(item2.Key, item.Value);
                                }
                            }
                        }
                    }
                }
            }

            return dataDictionary;
        }

        static int i = 0;
        public static Bitmap CutSection(Bitmap image, Rectangle selection)
        {
            Bitmap bmp = image as Bitmap;
            if (bmp == null)
                throw new ArgumentException("No valid bitmap");

            Bitmap cropBmp = bmp.Clone(selection, bmp.PixelFormat);
            if (CheckPixel(cropBmp))
                return ResizeImage(cropBmp, 25, 25);
            else
                return null;
        }
        public static void VisualizeV2(Bitmap bitmap, QReport report, string folder)
        {
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
                    Debug.WriteLine(ex);
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

