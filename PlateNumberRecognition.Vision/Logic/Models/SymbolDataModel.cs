using System;
using System.Collections.Generic;
using System.Drawing;

namespace PlateNumberRecognition.Vision.Logic.Models
{
    public class SymbolDataModel
    {
        public Bitmap Image { get; set; }
        public Tuple<int, int> Position;
        public Tuple<int, int> Size;
    }
    public class SymbolsGroupModel
    {
        public int id { get; set; }
        public List<Bitmap> Image { get; set; }
    }
}
