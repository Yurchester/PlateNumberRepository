using PlateNumberRecognition.Vision.Logic.Models;
using System.Collections.Generic;

namespace PlateNumberRecognition.Vision.Logic.Interfaces
{
    /// <summary>
    /// Сканер изображения, даёт возможность на изображении найти все отрывки изображений.
    /// </summary>
    public interface IScanner
    {
        /// <summary>
        /// Получить фрагменты исходного изображения.
        /// </summary>
        /// <param name="sourceImage"></param>
        /// <returns></returns>
        IList<QSymbol> GetFragments(IMonomap sourceImage);
    }
}
