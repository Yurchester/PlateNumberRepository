using PlateNumberRecognition.Vision.Logic.Interfaces;
using System;

namespace PlateNumberRecognition.Vision.Logic.Helpers
{
    public abstract class MonomapBase : IMonomap
    {
        /// <inheritdoc/>
        public virtual int Width
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <inheritdoc/>
        public virtual int Height
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <inheritdoc/>
        public virtual bool this[int x, int y]
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}

