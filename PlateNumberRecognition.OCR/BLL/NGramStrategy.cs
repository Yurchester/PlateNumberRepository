using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlateNumberRecognition.OCR.BLL
{
    public static class NGramStrategy
    {
        private static List<string> _newtext;
        public static List<string> Run(string text, byte nGramSize)
        {
            _newtext = new List<string>();
            StringBuilder nGram = new StringBuilder();
            Queue<int> wordLengths = new Queue<int>();
            int wordCount = 0;
            int lastWordLen = 0;
            if (text != "" && char.IsLetterOrDigit(text[0]))
            {
                nGram.Append(text[0]);
                lastWordLen++;
            }
            for (int i = 1; i < text.Length - 1; i++)
            {
                char before = text[i - 1];
                char after = text[i + 1];
                if (char.IsLetterOrDigit(text[i])
                    || (text[i] != ' '
                        && (char.IsSeparator(text[i])
                            || char.IsPunctuation(text[i]))
                        && (char.IsLetterOrDigit(before)
                            && char.IsLetterOrDigit(after))))
                {
                    nGram.Append(text[i]);
                    lastWordLen++;
                }
                else
                {
                    if (lastWordLen > 0)
                    {
                        wordLengths.Enqueue(lastWordLen);
                        lastWordLen = 0;
                        wordCount++;


                        if (wordCount >= nGramSize)
                        {
                            _newtext.Add(nGram.ToString().Trim().ToLower());

                            if (wordCount > 1)
                                nGram.Remove(0, wordLengths.Dequeue() + 1);
                            else
                                nGram.Clear();
                            wordCount -= 1;
                        }
                        nGram.Append(" ");
                    }
                }
            }

            var uniqueStrings = _newtext.Distinct();

            return uniqueStrings.ToList();
        }
    }
}
