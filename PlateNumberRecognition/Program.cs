using System;

namespace PlateNumberRecognition
{
    class Program
    {
        static void Main(string[] args)
        {
            new Engine.Engine().Run();
            Console.ReadKey();
        }
    }
}
