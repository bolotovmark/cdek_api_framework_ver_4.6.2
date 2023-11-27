using System;

namespace _462
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var client = new ClientCDEK("IdIejLzCOIocgZjkdC2yBb4Scs6tVImq", "Qtwdni8MbMMwOu6oKZGtMlzK55QNmx4Z");
            Console.WriteLine(client.CalculatePrice("Ижевск", 12, 0.07));

        }
    }
    
    
}