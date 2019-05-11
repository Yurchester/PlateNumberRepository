using PlateNumberRecognition.Vision.Logic.Engine;
using System;
using System.Diagnostics;

namespace PlateNumberRecognition.Vision.Logic.Models
{
    [DebuggerDisplay("{Height}-{EulerCode}")]
    public class SymbolCode
    {
        /// <summary>
        /// Создание экземпляра класса <see cref="SymbolCode"/>.
        /// </summary>
        public SymbolCode(int height, EulerMonomap2D eulerCode)
        {
            if (height <= 0)
            {
                throw new ArgumentException(nameof(height));
            }

            EulerCode = eulerCode;
            Height = height;
        }

        /// <summary>
        /// Размер шрифта.
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// Значение эйлеровой характеристики.
        /// </summary>
        public EulerMonomap2D EulerCode { get; }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return EulerCode.GetHashCode();
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj?.GetHashCode() == GetHashCode();
        }
    }
}
