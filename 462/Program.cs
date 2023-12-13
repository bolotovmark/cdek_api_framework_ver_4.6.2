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
            var clientDelovyiLinii = new ClientDelovyiLinii("5994B3CF-AA7A-4D2E-9057-51DB55802FA0");
            output = clientDelovyiLinii.CalculatePriceCityToCity("Пермь", 12.3, 0.07);
            Console.WriteLine("DL(city2city): price: " + output["price"] + " // period: " + output["period"]);

            output = clientDelovyiLinii.CalculatePriceAddressToAddress("Пермь, Профессора Дедюкина 18", 12.3, 0.07);
            Console.WriteLine("DL(address2address): price: " + output["price"] + " // period: " + output["period"]);
        }
    }
    
    
}