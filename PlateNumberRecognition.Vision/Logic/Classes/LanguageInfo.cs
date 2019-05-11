namespace PlateNumberRecognition.Vision.Logic.Classes
{
    public class LanguageInfo
    {
        public LanguageInfo(string name, string displayName, char minChar, char maxChar)
        {
            Name = name;
            DisplayName = displayName;
            MinChar = minChar;
            MaxChar = maxChar;
        }

        public string DisplayName { get; private set; }

        public string Name { get; private set; }

        public char MinChar { get; private set; }

        public char MaxChar { get; private set; }

        public string MinFont { get; set; } = 11.ToString();

        public string MaxFont { get; set; } = 78.ToString();
    }
}
