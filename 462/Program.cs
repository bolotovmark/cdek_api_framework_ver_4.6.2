using System;

namespace _462
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            //CDEK
            var clientCdek = new ClientCDEK("IdIejLzCOIocgZjkdC2yBb4Scs6tVImq", "Qtwdni8MbMMwOu6oKZGtMlzK55QNmx4Z");
            
            var output = clientCdek.CalculatePriceCityToCity("Екатеринбург", 12.3, 0.07);
            Console.WriteLine("CDEK(city2city): price: " + output["price"] + " // period: " + output["period"]);
            
            output = clientCdek.CalculatePriceAddressToAddress("Екатеринбург, площадь Бахчиванджи, 1", 12.3, 0.07);
            Console.WriteLine("CDEK(address2address): price: " + output["price"] + " // period: " + output["period"]);

            
            //KIT
            var clientKIT = new ClientKIT("prLwyGH3gxa6O11A9C26lR7SRL0nDPhh");
            output = clientKIT.CalculatePrice("Ижевск", 12.4, 0.07);
            Console.WriteLine("KIT(city2city): price: " + output["price"] + " // period: " + output["period"]);
            
            //Delovyi Linii
            var clientDelovyiLinii = new ClientDelovyiLinii("0F74D344-4281-4A25-A545-8A5A48566D44");
            // Console.WriteLine(clientDelovyiLinii.CalculatePrice("Екатеринбург", 12.3, 0.07));
            // Console.WriteLine(clientDelovyiLinii.CalculatePrice("Екатеринбург, площадь Бахчиванджи, 1", 12.3, 0.07));
            //
            
        }
    }
    
    
}