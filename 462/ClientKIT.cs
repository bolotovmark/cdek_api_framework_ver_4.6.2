using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace _462
{
    public class ClientKIT
    {
        private string _apiKey;
        
        public ClientKIT(string apiKey)
        {
            _apiKey = apiKey;
        }

        private string PostRequest(string urlRequest, string jsonRequestData)
        {
            var sentData = Encoding.UTF8.GetBytes(jsonRequestData);
            
            HttpWebRequest req = WebRequest.Create(urlRequest) as HttpWebRequest;
            
            req.Method = "POST";
            req.Headers.Add("Authorization", "Bearer " + _apiKey);
            req.ContentType = "application/json";
            req.Accept = "application/json";
            
            req.ContentLength = sentData.Length;
            Stream sendStream = req.GetRequestStream();
            sendStream.Write(sentData, 0, sentData.Length);

            var res = req.GetResponse() as HttpWebResponse;
            var resStream = res.GetResponseStream();
            var sr = new StreamReader(resStream, Encoding.UTF8);

            return sr.ReadToEnd();
        }

        public string GetCodeCity(string cityDerival)
        {
            string urlRequest = "https://capi.tk-kit.com/1.1/tdd/search/by-name";
            string jsonRequestData = "{\"title\": \"" + cityDerival + "\"}";

            string responceCode = PostRequest(urlRequest, jsonRequestData);
            
            string pattern = "\"code\"\\s*:\\s*\"" + "([^\"]+)" + "\"";
            Match match = Regex.Match(responceCode, pattern);
            
            return match.Groups[1].Value;
        }

        public string CalculatePrice(string cityArrival, double mass, double volume)
        {
            string urlRequest = "https://capi.tk-kit.com/1.1/order/calculate";
            string cityDerivalCode = "590001200000"; //code Чайковский
            if (cityArrival.Contains(","))
            {
                cityArrival = cityArrival.Split(',')[0];
                Console.WriteLine(cityArrival);
            }
            string cityArrivalCode = GetCodeCity(cityArrival);
            string massStr = mass.ToString(CultureInfo.InvariantCulture);
            string volumeStr = volume.ToString(CultureInfo.InvariantCulture);
            
            string jsonData = "{" +
                              "\"city_pickup_code\": \"" + cityDerivalCode + "\"," +
                              "\"city_delivery_code\": \"" + cityArrivalCode + "\"," +
                              "\"declared_price\": \"1000\"," +
                              "\"places\": [" +
                              "{" +
                              "\"count_place\": \"1\"," +
                              "\"volume\": \"" + volumeStr + "\"," +
                              "\"weight\": \"" + massStr + "\"" +
                              "}" +
                              "]" +
                              "}";

            string responce = PostRequest(urlRequest, jsonData);
            string pattern = @"""standart""\s*:\s*\{[^}]*""cost""\s*:\s*(\d+)";
            Match match = Regex.Match(responce, pattern);
            
            return match.Groups[1].Value;
        }
    }
}