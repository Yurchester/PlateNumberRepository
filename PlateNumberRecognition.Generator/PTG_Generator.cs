using PlateNumberRecognition.Generator.GeneratorFolder;
using PlateNumberRecognition.Vision.Logic.Classes;
using PlateNumberRecognition.Vision.Logic.Engine;
using PlateNumberRecognition.Vision.Logic.Extensions;
using PlateNumberRecognition.Vision.Logic.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PlateNumberRecognition.Generator
{
    public class PTG_Generator
    {
        const string LastSelectedFontsFileName = "LastFonts.txt";
        private readonly EulerGenerator _generator = new EulerGenerator();
        private int _genImageNumber = 0;
        private Font _currentFont;
        public PTG_Generator()
        {
            InitData();
            ProcessData();
            //UpdateFileFontNames();
        }
        private void UpdateFileFontNames()
        {
            var all = Environment.NewLine + File.ReadAllText(LastSelectedFontsFileName);
        }

        private void InitData()
        {
            if (!File.Exists(LastSelectedFontsFileName))
            {
                File.WriteAllLines(LastSelectedFontsFileName,
                    new[]
                    {
                        "Arial",
                        "Tahoma",
                        "Times New Roman",
                        "Calibri",
                        "Calibri Light"
                    });
            }

            var ruRU = CreateLanguage("RU-ru", "Russian", 'а', 'я');

            Languages = new List<LanguageInfo>()
            {
                ruRU
            };
        }

        private LanguageInfo CreateLanguage(string name, string displayName, char minChar, char maxChar)
        {
            return new LanguageInfo(name, displayName, minChar, maxChar);
        }

        public IEnumerable<LanguageInfo> Languages { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public Font CurrentFont
        {
            get
            {
                return _currentFont;
            }
            set
            {
                if (Equals(value, _currentFont))
                {
                    return;
                }

                _currentFont?.Dispose();
                _currentFont = value;
            }
        }

        public static Bitmap CombineBitmap(params Bitmap[] bitmaps)
        {
            List<Bitmap> images = new List<Bitmap>();
            Bitmap finalImage = null;

            try
            {
                int width = 0;
                int height = 0;

                foreach (Bitmap bitmap in bitmaps)
                {
                    width += bitmap.Width;
                    height = bitmap.Height > height ? bitmap.Height : height;

                    images.Add(bitmap);
                }
                finalImage = new Bitmap(width, height);
                using (Graphics g = Graphics.FromImage(finalImage))
                {
                    int offset = 0;
                    foreach (Bitmap image in images)
                    {
                        g.DrawImage(image, new Rectangle(offset, 0, image.Width, image.Height));
                        offset += image.Width;
                    }
                }

                return finalImage;
            }
            catch (Exception ex)
            {
                if (finalImage != null)
                {
                    finalImage.Dispose();
                }

                throw ex;
            }
            finally
            {
                foreach (Bitmap image in images)
                {
                    image.Dispose();
                }
            }
        }
        public async void ProcessData()
        {
            const string DefaultDictionaries = "../../../../PlateNumberRecognition.Dics";
            if (!Directory.Exists(DefaultDictionaries))
            {
                return;
            }
            _genImageNumber = 0;

            //if (IsPrintDebug.IsChecked.GetValueOrDefault())
            //{
            //    RecreateTestDir();
            //}

            DateTime dNow = DateTime.Now;

            var container = new EulerContainer();
            var fontFamilies = GetFileFonts().ToArray();

           _generator.BitmapCreated += GeneratorOnBitmapCreated;
      

            var lang = await GenerateLanguage(
                Languages.FirstOrDefault().Name,
                int.Parse(Languages.FirstOrDefault().MinFont),
                int.Parse(Languages.FirstOrDefault().MaxFont),
                Languages.FirstOrDefault().MinChar,
                Languages.FirstOrDefault().MaxChar,
                fontFamilies);

            var specialChars = new[]
            {
                //'0',
                //'1',
                //'2',
                //'3',
                //'4',
                //'5',
                //'6',
                //'7',
                //'8',
                //'9',
                //'#',
                '&',
                '(',
                ')'
                //'*',
                //'/',
                //':'
            };

            var specialCharsResult = await _generator.GenerateSpecialChars(
                specialChars,
                int.Parse(Languages.FirstOrDefault().MinFont),
                int.Parse(Languages.FirstOrDefault().MaxFont),
                fontFamilies);

            lang.FontFamilyNames = fontFamilies.Select(font => font.Name).ToList();

            container.Languages.Add(lang);
            container.SpecialChars = specialCharsResult;

            var compression = CompressionUtils.Compress(container);
            using (FileStream fileStream = new FileStream(Path.Combine(DefaultDictionaries, $"{Languages.FirstOrDefault().Name}.bin"), FileMode.Create))
            {
                compression.Position = 0;
                compression.CopyTo(fileStream);
            }
        }


        private IEnumerable<FontFamily> GetFileFonts()
        {
            var lastFonts = File.ReadAllLines(LastSelectedFontsFileName);
            return
                lastFonts.Select(font => FontFamily.Families.First(f => f.Name == font))
                    .ToArray();

        }

        private void RecreateTestDir()
        {
            if (Directory.Exists("BmpDebug"))
            {
                Directory.Delete("BmpDebug", true);
            }

            Directory.CreateDirectory("BmpDebug");
            while (!Directory.Exists("BmpDebug"))
            {
                ;
            }
        }

        private async Task<Language> GenerateLanguage(
            string localization,
            int minFont,
            int maxFont,
            char startChr,
            char endChr,
            FontFamily[] fontFamilies)
        {
            List<char> chars = new List<char>();
            for (char c = startChr; c <= endChr; c++)
            {
                chars.Add(c);
            }
            chars.Add('0');
            chars.Add('1');
            chars.Add('2');
            chars.Add('3');
            chars.Add('4');
            chars.Add('5');
            chars.Add('6');
            chars.Add('7');
            chars.Add('8');
            chars.Add('9');

            return await _generator.GenerateLanguage(chars.ToArray(), minFont, maxFont, localization, fontFamilies);
        }

        private void GeneratorOnBitmapCreated(object sender, BitmapEventArgs args)
        {

                        try
                        {
                            var chr = args.Chr;
                            var bmp = args.GeneratedBitmap;
                            var font = args.CurrentFont;
                            var fontName = font.FontFamily.Name;
                            var fontSize = font.Size;
                            var fontStyle = font.Style;
                            var debugDir = "BmpDebug";

                            var chrEx = chr.ToString();
                            if (char.IsLetter(chr))
                            {
                                chrEx = char.IsUpper(chr) ? $"^{chr}" : $"{chr}";
                            }

                            var chrDir = Path.Combine(debugDir, chrEx);
                            if (!Directory.Exists(chrDir))
                            {
                                Directory.CreateDirectory(chrDir);
                                while (!Directory.Exists(chrDir))
                                {
                                    ;
                                }
                            }

                            var debugFileName = $"{fontName} {fontSize} {fontStyle}.png";
                            Bitmap clone = new Bitmap(bmp, new Size(100, 100));
                            clone.Save(Path.Combine(chrDir, debugFileName), ImageFormat.Png);

                            _genImageNumber++;
                            bmp.Dispose();
                            clone.Dispose();
                            CurrentFont = args.CurrentFont;
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex);
                        }

        }

        //private void OnNewFontsClick(object sender, RoutedEventArgs e)
        //{
        //    List<FontFamily> allowedFonts = new List<FontFamily>();
        //    foreach (var fontFamily in System.Drawing.FontFamily.Families)
        //    {
        //        var fontStyle = fontFamily.IsStyleAvailable(System.Drawing.FontStyle.Regular)
        //            ? System.Drawing.FontStyle.Regular
        //            : fontFamily.IsStyleAvailable(System.Drawing.FontStyle.Italic)
        //                ? System.Drawing.FontStyle.Italic
        //                : System.Drawing.FontStyle.Bold;

        //        var tempFont = new Font(fontFamily, 10, fontStyle, GraphicsUnit.Pixel);
        //        var preview = new[]
        //        {
        //            EulerGenerator.PrintChar('ъ', tempFont).ToBitmap(),
        //            EulerGenerator.PrintChar('ф', tempFont).ToBitmap(),
        //            EulerGenerator.PrintChar('е', tempFont).ToBitmap(),
        //            EulerGenerator.PrintChar('А', tempFont).ToBitmap(),
        //            EulerGenerator.PrintChar('Ы', tempFont).ToBitmap(),
        //            EulerGenerator.PrintChar('ф', tempFont).ToBitmap(),
        //            EulerGenerator.PrintChar('у', tempFont).ToBitmap(),
        //            EulerGenerator.PrintChar('ю', tempFont).ToBitmap(),
        //            EulerGenerator.PrintChar('я', tempFont).ToBitmap(),
        //            EulerGenerator.PrintChar('а', tempFont).ToBitmap(),
        //            EulerGenerator.PrintChar('м', tempFont).ToBitmap(),
        //        };

        //        PreviewImage.Source = BitmapUtils.SourceFromBitmap(CombineBitmap(preview));

        //        var dialogResult = MessageBox.Show(
        //            $"\"{fontFamily.Name}\" Используем ?",
        //            "",
        //            MessageBoxButton.YesNoCancel);

        //        if (dialogResult == MessageBoxResult.Cancel)
        //        {
        //            PreviewImage.Source = null;
        //            return;
        //        }

        //        if (dialogResult == MessageBoxResult.Yes)
        //        {
        //            allowedFonts.Add(fontFamily);
        //        }
        //    }

        //    File.WriteAllLines(LastSelectedFontsFileName, allowedFonts.Select(font => font.Name));
        //    UpdateFileFontNames();
        //}
    }
}
