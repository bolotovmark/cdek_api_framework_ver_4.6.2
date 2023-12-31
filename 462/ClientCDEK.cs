using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace _462
{
    public class ClientCDEK
    {
        private readonly string _account;
        private readonly string _securePassword;
        private string _accessToken = "";

        public ClientCDEK(string account, string securePassword)
        {
            _account = account;
            _securePassword = securePassword; 
            RenewAccessToken();
        }

        private void RenewAccessToken()
        {
            string URL = "https://api.cdek.ru/v2/oauth/token?parameters";
            
            WebRequest webRequest = WebRequest.Create(URL);
            
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            
            Stream reqStream = webRequest.GetRequestStream();
            //string postData = $"grant_type=client_credentials&client_id={_account}&client_secret={_securePassword}";
            string postData ="grant_type=client_credentials&client_id=" + _account + "&client_secret=" + _securePassword;
            byte[] postArray = Encoding.ASCII.GetBytes(postData);
            reqStream.Write(postArray, 0, postArray.Length);
            reqStream.Close();
            
            StreamReader sr = new StreamReader(webRequest.GetResponse().GetResponseStream());
            string result = sr.ReadToEnd();

            string pattern = "\"access_token\":\"(.*?)\"";
            var match = Regex.Match(result, pattern);
            _accessToken = match.Groups[1].Value;
        }

        public string GetCodeCity(string fullNameCity)
        {
            string URL = "https://api.cdek.ru/v2/location/cities?city=" + fullNameCity;
            
            WebRequest webRequest = WebRequest.Create(URL);
            
            webRequest.Method = "GET";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.Headers.Add("Authorization", "Bearer " + _accessToken);
            
            StreamReader sr = new StreamReader(webRequest.GetResponse().GetResponseStream());
            string result = sr.ReadToEnd();
            
            string pattern = "\"code\":(\\d+)";
            var match = Regex.Match(result, pattern);
            return match.Groups[1].Value;
        }

        public Dictionary<string, string> CalculatePriceCityToCity(string cityArrival, double mass, double volume)
        {
            string urlRequest = "https://api.cdek.ru/v2/calculator/tariff";
            string tarifCode = "136";   // код тарифа
            string fromCodeCity = "563";    //код города отправления - Чайковский 
            if (cityArrival.Contains(","))
            {
                cityArrival = cityArrival.Split(',')[0];
                Console.WriteLine(cityArrival);
            }
            
            string toCodeCity = GetCodeCity(cityArrival);
            string massStr = (mass * 1000).ToString(CultureInfo.InvariantCulture);
            string volumeStr = volume.ToString(CultureInfo.InvariantCulture);
            
            string jsonRequestData = "{" +
                                     "\"tariff_code\":" + "\"" + tarifCode + "\"," +
                                     "\"from_location\":" + "{" +
                                     "\"code\":" + fromCodeCity +
                                     "}," +
                                     "\"to_location\":" + "{" +
                                     "\"code\":" + toCodeCity +
                                     "}," +
                                     "\"packages\":[" + "{" +
                                     "\"height\":0," +
                                     "\"length\":0," +
                                     "\"weight\":" + massStr + "," +
                                     "\"width\":0" +
                                     "}" +
                                     "]" +
                                     "}";
            // string jsonRequestData = $"{{\n    \"tariff_code\": \"{tarifCode}\",\n    " +
            //                          $"\"from_location\": {{\n        \"code\": {fromCodeCity}\n    }},\n    \"to_location\": {{\n        " +
            //                          $"\"code\": {toCodeCity}\n    }},\n    \"packages\": [\n        {{\n            \"height\": 0,\n           " +
            //                          $" \"length\": 0,\n            \"weight\": {massStr},\n            \"width\": 0\n        }}\n    ]\n}}";
            var sentData = Encoding.UTF8.GetBytes(jsonRequestData);
            
            HttpWebRequest req = WebRequest.Create(urlRequest) as HttpWebRequest;
            req.Method = "POST";
            req.Headers.Add("Authorization", "Bearer " + _accessToken);
            req.ContentType = "application/json";
            req.Accept = "application/json";
            
            req.ContentLength = sentData.Length;
            Stream sendStream = req.GetRequestStream();
            sendStream.Write(sentData, 0, sentData.Length);
 

            var res = req.GetResponse() as HttpWebResponse;
            var resStream = res.GetResponseStream();
            var sr = new StreamReader(resStream, Encoding.UTF8);

            var json = sr.ReadToEnd();
            
            string pattern =  "\"delivery_sum\":(\\d+(\\.\\d+)?)";
            var matchPrice = Regex.Match(json, pattern);
            var price = matchPrice.Groups[1].Value;
                
            pattern = "\"period_min\"\\s*:\\s*(\\d+)";
            var matchPeriod = Regex.Match(json, pattern);
            var period = matchPeriod.Groups[1].Value;

            var output = new Dictionary<string, string>()
            {
                { "price", price},
                { "period", period},
            };
            return output;
        }
        
        public Dictionary<string, string> CalculatePriceAddressToAddress(string cityArrival, double mass, double volume)
        {
            string urlRequest = "https://api.cdek.ru/v2/calculator/tariff";
            string tarifCode = "136";   // код тарифа
            string fromCityAddress = "г. Чайковский, ул. Промышленная, 8/25";
            string massStr = (mass * 1000).ToString(CultureInfo.InvariantCulture);
            string volumeStr = volume.ToString(CultureInfo.InvariantCulture);
            
            string jsonRequestData = "{" +
                                     "\"tariff_code\":" + "\"" + tarifCode + "\"," +
                                     "\"from_location\":" + "{" +
                                     "\"address\": \"" + fromCityAddress +
                                     "\"}," +
                                     "\"to_location\":" + "{" +
                                     "\"address\": \"" + cityArrival +
                                     "\"}," +
                                     "\"packages\":[" + "{" +
                                     "\"height\":0," +
                                     "\"length\":0," +
                                     "\"weight\":" + massStr + "," +
                                     "\"width\":0" +
                                     "}" +
                                     "]" +
                                     "}";

            // string jsonRequestData = $"{{\n    \"tariff_code\": \"{tarifCode}\",\n    " +
            //                          $"\"from_location\": {{\n        \"code\": {fromCodeCity}\n    }},\n    \"to_location\": {{\n        " +
            //                          $"\"code\": {toCodeCity}\n    }},\n    \"packages\": [\n        {{\n            \"height\": 0,\n           " +
            //                          $" \"length\": 0,\n            \"weight\": {massStr},\n            \"width\": 0\n        }}\n    ]\n}}";
            var sentData = Encoding.UTF8.GetBytes(jsonRequestData);
            
            HttpWebRequest req = WebRequest.Create(urlRequest) as HttpWebRequest;
            req.Method = "POST";
            req.Headers.Add("Authorization", "Bearer " + _accessToken);
            req.ContentType = "application/json";
            req.Accept = "application/json";
            
            req.ContentLength = sentData.Length;
            Stream sendStream = req.GetRequestStream();
            sendStream.Write(sentData, 0, sentData.Length);
 

            var res = req.GetResponse() as HttpWebResponse;
            var resStream = res.GetResponseStream();
            var sr = new StreamReader(resStream, Encoding.UTF8);

            var json = sr.ReadToEnd();
            
            string pattern =  "\"delivery_sum\":(\\d+(\\.\\d+)?)";
            var matchPrice = Regex.Match(json, pattern);
            var price = matchPrice.Groups[1].Value;
                
            pattern = "\"period_min\"\\s*:\\s*(\\d+)";
            var matchPeriod = Regex.Match(json, pattern);
            var period = matchPeriod.Groups[1].Value;
            
            var output = new Dictionary<string, string>()
            {
                { "price", price},
                { "period", period},
            };
            return output;
        }
    }
}