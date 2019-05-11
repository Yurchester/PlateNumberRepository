using System;

namespace PlateNumberRecognition
{
    class Program
    {
        [Obsolete]
        static void Main(string[] args)
        {
            new Engine.Engine().Run();
            Console.ReadKey();
        }
    }
}
