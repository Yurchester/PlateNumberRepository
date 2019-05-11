using PlateNumberRecognition.Vision.Logic.Interfaces;
using System;

namespace PlateNumberRecognition.Vision.Logic.Helpers
{
    public class StretchPad : MonomapBase
    {
        private readonly IMonomap _monomap;

        private readonly int _left;

        private readonly int _top;

        private readonly int _right;

        private readonly int _bottom;

        /// <summary>
        /// Создание экземпляра класса <see cref="StretchPad"/>.
        /// </summary>
        public StretchPad(IMonomap monomap, int left = 1, int top = 1, int right = 1, int bottom = 1)
        {
            if (monomap == null)
            {
                throw new ArgumentNullException(nameof(monomap));
            }

            _monomap = monomap;
            _left = left;
            _top = top;
            _right = right;
            _bottom = bottom;
        }

        /// <inheritdoc/>
        public override int Height => _monomap.Height + _top + _bottom;

        /// <inheritdoc/>
        public override int Width => _monomap.Width + _left + _right;

        /// <inheritdoc/>
        public override bool this[int x, int y]
        {
            get
            {
                var leftX = x - _left;
                var topY = y - _top;

                if (0 <= leftX && leftX < _monomap.Width &&
                    0 <= topY && topY < _monomap.Height)
                {
                    return _monomap[leftX, topY];
                }

                return false;
            }
        }
    }
}


