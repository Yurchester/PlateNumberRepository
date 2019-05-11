using AForge.Imaging.Filters;
using PlateNumberRecognition.Vision.Logic.Engine;
using PlateNumberRecognition.Vision.Logic.Extensions;
using PlateNumberRecognition.Vision.Logic.Helpers;
using PlateNumberRecognition.Vision.Logic.Interfaces;
using PlateNumberRecognition.Vision.Logic.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading.Tasks;

namespace PlateNumberRecognition.Generator.GeneratorFolder
{
    public class EulerGenerator
    {
        private const int ImageBound = 10;
        private readonly object _syncObject = new object();
        public event EventHandler<BitmapEventArgs> BitmapCreated;
        public static IMonomap PrintChar(char chr, Font font)
        {
            using (Bitmap bitmap = new Bitmap((int)font.Size + ImageBound * 2, (int)font.Size + ImageBound * 2))
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(Color.White);
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
                graphics.DrawString(chr.ToString(), font, Brushes.Black, ImageBound, ImageBound);
                graphics.Flush();

                try
                {
                    Grayscale filter = new Grayscale(0.2125, 0.7154, 0.0721);
                    filter.Apply(bitmap);
                    bool[,] booleanBitmap = new bool[bitmap.Width, bitmap.Height];
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        for (int y = 0; y < bitmap.Height; y++)
                        {
                            Color pixel = bitmap.GetPixel(x, y);
                            booleanBitmap[x, y] = GetBrightness(pixel) < 130;
                        }
                    }

                    return new BitMonomap(booleanBitmap);
                }
                catch
                {
                    throw;
                }
            }
        }
        private static int GetBrightness(Color c)
        {
            return (int)Math.Sqrt(
            c.R * c.R * .241 +
            c.G * c.G * .691 +
            c.B * c.B * .068);
        }

        public async Task<Language> GenerateLanguage(
            char[] sourceChars,
            int minSize,
            int maxSize,
            string localizationName,
            FontFamily[] fontFamilies)
        {
            if (string.IsNullOrEmpty(localizationName))
            {
                throw new ArgumentNullException(nameof(localizationName));
            }

            Language language = new Language { LocalizationName = localizationName };

            await Task.Factory.StartNew(
                () =>
                {
                    language.Chars.AddRange(sourceChars.Select(@char => new Symbol { Chr = char.ToLower(@char) }));
                    language.Chars.AddRange(sourceChars.Select(@char => new Symbol { Chr = char.ToUpper(@char) }));

                    InternalGenerateEulerCollections(
                        sourceChars,
                        minSize,
                        maxSize,
                        language.Chars,
                        fontFamilies);
                });

            return language;
        }

        public async Task<List<Symbol>> GenerateSpecialChars(
            char[] sourceChars,
            int minSize,
            int maxSize,
            FontFamily[] fontFamilies)
        {
            return await Task.Factory.StartNew(
                () =>
                {
                    var result = new List<Symbol>();
                    result.AddRange(sourceChars.Select(@char => new Symbol { Chr = char.ToLower(@char) }));

                    InternalGenerateEulerCollections(sourceChars, minSize, maxSize, result, fontFamilies);

                    return result;
                });
        }
        Font newFont;
        private void InternalGenerateEulerCollections(
            char[] sourceChars,
            int minSize,
            int maxSize,
            List<Symbol> charColleciton,
            FontFamily[] fontFamilies)
        {
            var lowcaseChars = sourceChars.Select(char.ToLower).ToArray();
            var uppercaseChars = sourceChars.Select(char.ToUpper).ToArray();
            List<FontStyle> styles = new List<FontStyle>();

            if (Styles.Bold)
                styles.Add(FontStyle.Bold);
            if (Styles.Regular)
                styles.Add(FontStyle.Regular);
            if (Styles.Italic)
                styles.Add(FontStyle.Italic);

            if (styles.Count == 0)
                styles.Add(FontStyle.Regular);

            try
            {
                foreach (var fontFamily in fontFamilies)
                {
                    foreach (var fontStyle in styles)
                    {
                        try
                        {
                            newFont = new Font(fontFamily, minSize, fontStyle);
                        }
                        catch (Exception ex)
                        {
                            continue;
                        }
                        if (newFont != null)
                        {
                            InternalGenerateEulerValue(lowcaseChars, newFont, minSize, maxSize, charColleciton);
                            InternalGenerateEulerValue(uppercaseChars, newFont, minSize, maxSize, charColleciton);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            //Parallel.ForEach(
            //    fontFamilies,
            //    fontFamily =>
            //    {
            //        foreach (var fontStyle in styles)
            //        {
            //            var newFont = new Font(fontFamily, minSize, fontStyle);
            //            InternalGenerateEulerValue(lowcaseChars, newFont, minSize, maxSize, charColleciton);
            //            InternalGenerateEulerValue(uppercaseChars, newFont, minSize, maxSize, charColleciton);
            //        }
            //    });
        }

        private object _saveLock = new object();

        private void InternalGenerateEulerValue(
            char[] sourceChars,
            Font font,
            int minSize,
            int maxSize,
            List<Symbol> symbols)
        {
            foreach (char chr in sourceChars)
            {
                for (int size = minSize; size < maxSize + 1; size++)
                {
                    // TODO Что бы красиво выводить побуквенно идут по размерам каждой буквы, иначе тут лучше не вертикально а горизонтально по слою проходить
                    using (var newFont = new Font(font.FontFamily, size, font.Style, GraphicsUnit.Pixel))
                    {
                        IMonomap monomap = PrintChar(chr, newFont);
                        int height = GetFontHeight(monomap);

                        if (height < minSize)
                        {
                            continue;
                        }

                        var euler = EulerCharacteristicComputer.Compute2D(monomap);

                        var chr1 = chr;
                        Symbol symbol = symbols.First(s => s.Chr == chr1);
                        SymbolCode symbolCode = new SymbolCode(height, euler);

                        lock (_syncObject)
                        {
                            symbol.Codes.Add(symbolCode);
                        }

                        BitmapCreated?.Invoke(this, new BitmapEventArgs(monomap.ToBitmap(), newFont, chr));
                    }
                }
            }
        }

        private int GetFontHeight(IMonomap monomap)
        {
            int topY = -1, bottomY = -1;

            for (int y = 0; y < monomap.Height; y++)
            {
                for (int x = 0; x < monomap.Width; x++)
                {
                    if (monomap[x, y])
                    {
                        topY = y;
                    }
                }
            }

            for (int y = monomap.Height - 1; y >= 0; y--)
            {
                for (int x = 0; x < monomap.Width; x++)
                {
                    if (monomap[x, y])
                    {
                        bottomY = y;
                    }
                }
            }

            return topY - bottomY + 1;
        }
    }
}