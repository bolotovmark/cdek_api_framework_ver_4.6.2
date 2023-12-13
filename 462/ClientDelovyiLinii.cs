using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace _462
{
    public class ClientDelovyiLinii
    {
        private string _apiKey;
        
        public ClientDelovyiLinii(string apiKey)
        {
            _apiKey = apiKey;
        }
        
        private string PostRequest(string urlRequest, string jsonRequestData)
        {
            var sentData = Encoding.UTF8.GetBytes(jsonRequestData);
            
            HttpWebRequest req = WebRequest.Create(urlRequest) as HttpWebRequest;
            
            req.Method = "POST";
            req.ContentType = "application/json";
            req.Accept = "application/json";
            req.Timeout = 10000;
            
            req.ContentLength = sentData.Length;
            Stream sendStream = req.GetRequestStream();
            sendStream.Write(sentData, 0, sentData.Length);
            
            var res = req.GetResponse() as HttpWebResponse;
            var resStream = res.GetResponseStream();
            var sr = new StreamReader(resStream, Encoding.UTF8);
            var response = sr.ReadToEnd();
            res.Dispose();
            sr.Dispose();
            sendStream.Dispose();
            return response;
        }

        public string GetFirstDate(string cityDerival, string mass, string volume)
        {
            string urlRequest = "https://api.dellin.ru/v2/request/address/dates.json";
            
            string jsonRequestData = "{" +
                                 "\"appkey\":\"" + _apiKey + "\"," +
                                 "\"delivery\":{" +
                                 "\"deliveryType\":{" +
                                 "\"type\":\"auto\"" +
                                 "}," +
                                 "\"derival\":{" +
                                 "\"address\":{" +
                                 "\"search\":\"" + cityDerival + "\"" +
                                 "}" +
                                 "}" +
                                 "}," +
                                 "\"cargo\":{" +
                                 "\"weight\":0," +
                                 "\"height\":0," +
                                 "\"width\":0," +
                                 "\"length\":0," +
                                 "\"totalVolume\":" + volume + "," +
                                 "\"totalWeight\":" + mass +
                                 "}" +
                                 "}";
            
            string responce = PostRequest(urlRequest, jsonRequestData);
            string pattern = "\"dates\":\\[\"(.*?)\"";
            Match match = Regex.Match(responce, pattern);

            return match.Groups[1].Value;
        }

        public string GetCodeCity(string city)
        {
            string urlRequest = "https://api.dellin.ru/v2/public/kladr.json";
            string jsonRequestData = "{" +
                                     "\"appkey\":\"" + _apiKey + "\"," +
                                     "\"q\":\"" + city + "\"," +
                                     "\"limit\":" + 1 +
                                     "}";
            string responce = PostRequest(urlRequest, jsonRequestData);
            string pattern = "\"code\"\\s*:\\s*\"(\\d+)\"";
            Match match = Regex.Match(responce, pattern);

            return match.Groups[1].Value;
        }
        
        public Dictionary<string, string> CalculatePriceCityToCity(string cityArrival, double mass, double volume)
        {
            string urlRequest = "https://api.dellin.ru/v2/calculator.json";
            string massStr = mass.ToString(CultureInfo.InvariantCulture);
            string volumeStr = volume.ToString(CultureInfo.InvariantCulture);
            string firstDate = GetFirstDate("г. Чайковский, ул. Промышленная, 8/25", massStr, volumeStr);
            string codeCity = GetCodeCity(cityArrival);
            
            string jsonRequestData =
                "{" +
                "\"appkey\":\"" + _apiKey + "\"," +
                "\"delivery\":{" +
                "\"deliveryType\":{" +
                "\"type\":\"auto\"" +
                "}," +
                "\"arrival\":{" +
                "\"variant\":\"terminal\"," +
                "\"city\":\"" + codeCity + "\"," +
                "\"time\":{" +
                "\"worktimeStart\":\"08:00\"," +
                "\"worktimeEnd\":\"17:00\"" +
                "}" +
                "}," +
                "\"derival\":{" +
                "\"produceDate\":\"" + firstDate + "\"," +
                "\"variant\":\"address\"," +
                "\"address\":{" +
                "\"search\":\"г. Чайковский, ул. Промышленная, 8/25\"" +
                "}," +
                "\"time\":{" +
                "\"worktimeStart\":\"08:00\"," +
                "\"worktimeEnd\":\"17:00\"" +
                "}" +
                "}" +
                "}," +
                "\"cargo\":{" +
                "\"length\":0," +
                "\"width\":0," +
                "\"height\":0," +
                "\"totalVolume\":" + volumeStr + "," +
                "\"totalWeight\":" + massStr + "," +
                "\"hazardClass\":0" +
                "}" +
                "}";

            string responce = PostRequest(urlRequest, jsonRequestData);
            
            string pattern = "]},\"price\":(.*?),\"";
            Match matchPrice = Regex.Match(responce, pattern);
            var price = matchPrice.Groups[1].Value;
            
            pattern = "\"arrivalToOspReceiver\":\"(\\d{4}-\\d{2}-\\d{2})\"";
            Match matchDate = Regex.Match(responce, pattern);
            var date = matchDate.Groups[1].Value;
            
            // Преобразование строк в объекты DateTime
            DateTime date1 = DateTime.Parse(firstDate);
            DateTime date2 = DateTime.Parse(date);

            // Вычисление разницы в днях
            TimeSpan difference = date2.Subtract(date1);
            
            var output = new Dictionary<string, string>()
            {
                { "price", price},
                { "period", difference.Days.ToString()},
            };
            return output;
        }

        public Dictionary<string, string> CalculatePriceAddressToAddress(string cityArrival, double mass, double volume)
        {
            string urlRequest = "https://api.dellin.ru/v2/calculator.json";
            string massStr = mass.ToString(CultureInfo.InvariantCulture);
            string volumeStr = volume.ToString(CultureInfo.InvariantCulture);
            string firstDate = GetFirstDate("г. Чайковский, ул. Промышленная, 8/25", massStr, volumeStr);
            
            string jsonRequestData =
                "{" +
                "\"appkey\":\"" + _apiKey + "\"," +
                "\"delivery\":{" +
                "\"deliveryType\":{" +
                "\"type\":\"auto\"" +
                "}," +
                "\"arrival\":{" +
                "\"variant\":\"address\"," +
                "\"address\":{" +
                "\"search\":\"" + cityArrival + "\"" +
                "}," +
                "\"time\":{" +
                "\"worktimeStart\":\"08:00\"," +
                "\"worktimeEnd\":\"17:00\"" +
                "}" +
                "}," +
                "\"derival\":{" +
                "\"produceDate\":\"" + firstDate + "\"," +
                "\"variant\":\"address\"," +
                "\"address\":{" +
                "\"search\":\"г. Чайковский, ул. Промышленная, 8/25\"" +
                "}," +
                "\"time\":{" +
                "\"worktimeStart\":\"08:00\"," +
                "\"worktimeEnd\":\"17:00\"" +
                "}" +
                "}" +
                "}," +
                "\"cargo\":{" +
                "\"length\":0," +
                "\"width\":0," +
                "\"height\":0," +
                "\"totalVolume\":" + volumeStr + "," +
                "\"totalWeight\":" + massStr + "," +
                "\"hazardClass\":0" +
                "}" +
                "}";

            string responce = PostRequest(urlRequest, jsonRequestData);
            
            string pattern = "]},\"price\":(.*?),\"";
            Match matchPrice = Regex.Match(responce, pattern);
            var price = matchPrice.Groups[1].Value;
            
            pattern = "\"arrivalToOspReceiver\":\"(\\d{4}-\\d{2}-\\d{2})\"";
            Match matchDate = Regex.Match(responce, pattern);
            var date = matchDate.Groups[1].Value;
            
            // Преобразование строк в объекты DateTime
            DateTime date1 = DateTime.Parse(firstDate);
            DateTime date2 = DateTime.Parse(date);

            // Вычисление разницы в днях
            TimeSpan difference = date2.Subtract(date1);
            
            var output = new Dictionary<string, string>()
            {
                { "price", price},
                { "period", difference.Days.ToString()},
            };
            return output;
        }
    }
}