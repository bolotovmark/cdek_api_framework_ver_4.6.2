using System;

namespace _462
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            //CDEK
            var clientCdek = new ClientCDEK("IdIejLzCOIocgZjkdC2yBb4Scs6tVImq", "Qtwdni8MbMMwOu6oKZGtMlzK55QNmx4Z");
            Console.WriteLine(clientCdek.CalculatePrice("Ижевск", 12.3, 0.07));
            
            //Delovyi Linii
            var clientDelovyiLinii = new ClientDelovyiLinii("0F74D344-4281-4A25-A545-8A5A48566D44");
            Console.WriteLine(clientDelovyiLinii.CalculatePrice("Ижевск", 12.3, 0.07));
            
            //KIT
            var clientKIT = new ClientKIT("dm8dr4oMCGRQa00FfLj_l3CxmnJ3Tw_6");
            Console.WriteLine(clientKIT.CalculatePrice("Ижевск", 12.4, 0.07));
            
        }
    }
    
    
}