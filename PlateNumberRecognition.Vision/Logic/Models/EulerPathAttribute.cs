using System;

namespace PlateNumberRecognition.Vision.Logic.Models
{
    [AttributeUsage(AttributeTargets.Property)]
    public class EulerPathAttribute : Attribute
    {
        /// <summary>
        /// Создание экземпляра класса <see cref="EulerPathAttribute"/>.
        /// </summary>
        /// <param name="path">Значение пути.</param>
        public EulerPathAttribute(string path)
        {
            Path = path;
        }

        /// <summary>
        /// Эйлеровая характеристика в вытянутом виде.
        /// </summary>
        /// <remarks>
        /// Обход пути происходит построчечно, слева на право.
        /// </remarks>
        public string Path { get; private set; }
    }
}