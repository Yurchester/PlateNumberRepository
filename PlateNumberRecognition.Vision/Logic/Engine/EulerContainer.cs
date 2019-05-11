using PlateNumberRecognition.Vision.Logic.Models;
using System.Collections.Generic;

namespace PlateNumberRecognition.Vision.Logic.Engine
{
    public class EulerContainer
    {
        /// <summary>
        /// Создание экземпляра класса <see cref="EulerContainer"/>.
        /// </summary>
        public EulerContainer()
        {
            Languages = new List<Language>();
            SpecialChars = new List<Symbol>();
        }

        /// <summary>
        /// Список языковых наборов.
        /// </summary>
        public List<Language> Languages { get; set; }

        /// <summary>
        /// Набор спецсимволо в.
        /// </summary>
        public List<Symbol> SpecialChars { get; set; }
    }
}
