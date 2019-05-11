using PlateNumberRecognition.Vision.Logic.Engine;
using PlateNumberRecognition.Vision.Logic.Interfaces;
using System.Drawing;

namespace PlateNumberRecognition.Vision.Logic.Models
{
    public class QSymbol
    {
        /// <summary>
        /// Создание экземпляра класса <see cref="QSymbol"/>.
        /// </summary>
        public QSymbol(IMonomap monomap, Point startPoint, EulerMonomap2D euler)
        {
            Euler = euler;
            Monomap = monomap;
            StartPoint = startPoint;
        }


        /// <summary>
        /// Значение эйлеровой характеристики.
        /// </summary>
        public EulerMonomap2D Euler { get; }


        /// <summary>
        /// Ссылка на изображение.
        /// </summary>
        public IMonomap Monomap { get; }

        /// <summary>
        /// Левая верхняя точка изображения.
        /// </summary>
        /// <remarks>Ширину и высоту можно узнать из <see cref="IMonomap"/>.</remarks>
        public Point StartPoint { get; private set; }

        /// <summary>
        /// Ширина изображения.
        /// </summary>
        public int Width => Monomap.Width;

        /// <summary>
        /// Высота изображения.
        /// </summary>
        public int Height => Monomap.Height;
    }
}
