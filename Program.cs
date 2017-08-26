using System;
using System.Threading;

namespace whisper
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.CancelKeyPress += (v1, v2) =>
            {
                Console.WriteLine("Stopping...");
            };
            Console.WriteLine("Starting...");
            while(true)
            {
                var line = Console.Read();
                Console.WriteLine($"Input: {line}");
            }
        }
    }
}
