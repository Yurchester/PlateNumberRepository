using AForge.Imaging.Filters;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace PlateNumberRecognition.Processing
{
    public class DataPreProcessing : IDataPreProcessing
    {
        public Bitmap SetBradleyThresholding(Bitmap bitmap)
        {
            Grayscale grayscale_filter = new Grayscale(0.2125, 0.7154, 0.0721);
            Bitmap grayImage = grayscale_filter.Apply(bitmap);
            return new BradleyLocalThresholding().Apply(grayImage);
        }

        public Bitmap SetBrightnessCorrection(Bitmap bitmap, int value)
        {
            return new BrightnessCorrection(value).Apply(bitmap);
        }

        public Bitmap SetContrastCorrection(Bitmap bitmap, int value)
        {
            return new ContrastCorrection(value).Apply(bitmap);
        }

        public Bitmap SetContrastStretch(Bitmap bitmap)
        {
            return new ContrastStretch().Apply(bitmap);
        }

        public Bitmap SetDilatation(Bitmap bitmap)
        {
            return new Dilatation().Apply(bitmap);
        }

        public Bitmap SetErosion(Bitmap bitmap)
        {
            return new Erosion().Apply(bitmap);
        }

        public Bitmap SetGammaCorrection(Bitmap bitmap, float value)
        {
            return new GammaCorrection(value).Apply(bitmap);
        }

        public Bitmap SetGaussianSharpen(Bitmap bitmap)
        {
            return new GaussianSharpen().Apply(bitmap);
        }

        public Bitmap Setgrayscale_filter(Bitmap bitmap)
        {
            return new Grayscale(0.2125, 0.7154, 0.0721).Apply(bitmap);
        }
        public Bitmap SetHistogramEqualizationfilter(Bitmap bitmap)
        {
            return new HistogramEqualization().Apply(bitmap);
        }

        //public Bitmap SetOtsuThresholding(Bitmap bitmap)
        //{
        //    Image<Gray, Byte> img_gray, img_threshold;
        //    img_gray = new Image<Gray, Byte>(bitmap);
        //    img_threshold = new Image<Gray, byte>(img_gray.Size);
        //    CvInvoke.Threshold(img_gray, img_threshold, 0, 255, Emgu.CV.CvEnum.ThresholdType.Otsu);

        //    return img_threshold.ToBitmap();
        //}
        //public Bitmap SetBinaryThresholding(Bitmap bitmap)
        //{
        //    Image<Gray, Byte> img_gray, img_threshold;
        //    img_gray = new Image<Gray, Byte>(bitmap);
        //    img_threshold = new Image<Gray, byte>(img_gray.Size);
        //    CvInvoke.Threshold(img_gray, img_threshold, 0, 255, Emgu.CV.CvEnum.ThresholdType.Binary);

        //    return img_threshold.ToBitmap();
        //}
        public Bitmap ScaleByPercent(Bitmap bitmap, int Percent)
        {
            float nPercent = ((float)Percent / 100);

            int sourceWidth = bitmap.Width;
            int sourceHeight = bitmap.Height;
            var destWidth = (int)(sourceWidth * nPercent);
            var destHeight = (int)(sourceHeight * nPercent);

            var bmPhoto = new Bitmap(destWidth, destHeight,
                                     PixelFormat.Format32bppArgb);
            bmPhoto.SetResolution(bitmap.HorizontalResolution,
                                  bitmap.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(bitmap,
                              new Rectangle(0, 0, destWidth, destHeight),
                              new Rectangle(0, 0, sourceWidth, sourceHeight),
                              GraphicsUnit.Pixel);
            grPhoto.Dispose();

            return bmPhoto;
        }
        public Bitmap Bitmap24bppRgb(Bitmap bitmap)
        {
            return bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), PixelFormat.Format24bppRgb);
            // return bitmap.Clone(new Rectangle(0, 0, bitmap.Width - 1, bitmap.Height - 1), PixelFormat.Format24bppRgb);
        }
        public Bitmap Bitmap8bppIndexed(Bitmap bitmap)
        {
            return bitmap.Clone(new Rectangle(0, 0, bitmap.Width - 1, bitmap.Height - 1), PixelFormat.Format8bppIndexed);
        }
        public Bitmap ChangingPixel(Bitmap bitmap)
        {
            for (int i = 0; i < bitmap.Height; i++)
            {
                for (int j = 0; j < bitmap.Width; j++)
                {
                    if (
                        bitmap.GetPixel(j, i).B > 220 &&
                        bitmap.GetPixel(j, i).G > 220 &&
                        bitmap.GetPixel(j, i).R > 220 &&
                        bitmap.GetPixel(j, i).A > 220)
                    {
                        bitmap.SetPixel(j, i, Color.White);
                    }
                    else if (
               bitmap.GetPixel(j, i).B < 220 &&
               bitmap.GetPixel(j, i).G < 220 &&
               bitmap.GetPixel(j, i).R < 220 &&
                bitmap.GetPixel(j, i).A < 220)
                    {
                        bitmap.SetPixel(j, i, Color.Black);
                    }
                }
            }
            return bitmap;
        }
        public Bitmap CutSection(Bitmap image, Rectangle selection)
        {
            Bitmap bmp = image as Bitmap;
            if (bmp == null)
                throw new ArgumentException("No valid bitmap");

            Bitmap cropBmp = bmp.Clone(selection, bmp.PixelFormat);
            image.Dispose();

            return cropBmp;
        }

        //public Bitmap FloodFill(Bitmap bitmap, int x, int y, Color color)
        //{
        //    BitmapData data = bitmap.LockBits(
        //    new Rectangle(0, 0, bitmap.Width, bitmap.Height),
        //    ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
        //    int[] bits = new int[data.Stride / 4 * data.Height];
        //    Marshal.Copy(data.Scan0, bits, 0, bits.Length);

        //    LinkedList<Point> check = new LinkedList<Point>();
        //    int floodTo = color.ToArgb();
        //    int floodFrom = bits[x + y * data.Stride / 4];
        //    bits[x + y * data.Stride / 4] = floodTo;

        //    if (floodFrom != floodTo)
        //    {
        //        check.AddLast(new Point(x, y));
        //        while (check.Count > 0)
        //        {
        //            Point cur = check.First.Value;
        //            check.RemoveFirst();

        //            foreach (Point off in new Point[] {
        //                 new Point(0, -1), new Point(0, 1),
        //                 new Point(-1, 0), new Point(1, 0)})
        //            {
        //                Point next = new Point(cur.X + off.X, cur.Y + off.Y);
        //                if (next.X >= 0 && next.Y >= 0 &&
        //                  next.X < data.Width &&
        //                  next.Y < data.Height)
        //                {
        //                    if (bits[next.X + next.Y * data.Stride / 4] == floodFrom)
        //                    {
        //                        check.AddLast(next);
        //                        bits[next.X + next.Y * data.Stride / 4] = floodTo;
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    Marshal.Copy(bits, 0, data.Scan0, bits.Length);
        //    bitmap.UnlockBits(data);
        //    return bitmap;
        //}
    }
}

