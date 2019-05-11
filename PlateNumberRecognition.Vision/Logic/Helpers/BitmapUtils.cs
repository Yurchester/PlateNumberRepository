using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace PlateNumberRecognition.Vision.Logic.Helpers
{
    public static class BitmapUtils
    {
        //public static Bitmap BitmapFromSource(BitmapSource bitmapsource)
        //{
        //    Bitmap bitmap;
        //    using (var outStream = new MemoryStream())
        //    {
        //        BitmapEncoder enc = new BmpBitmapEncoder();
        //        enc.Frames.Add(BitmapFrame.Create(bitmapsource));
        //        enc.Save(outStream);
        //        bitmap = new Bitmap(outStream);
        //    }
        //    return bitmap;
        //}
        //[System.Runtime.InteropServices.DllImport("gdi32.dll")]
        //public static extern bool DeleteObject(IntPtr hObject);
        //public static BitmapSource SourceFromBitmap(Bitmap bitmap)
        //{
        //    using (bitmap)
        //    {
        //        IntPtr hBitmap = bitmap.GetHbitmap();

        //        try
        //        {
        //            return System.Drawing.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        //        }
        //        finally
        //        {
        //            DeleteObject(hBitmap);
        //        }
        //    }
        //}
    }
}

