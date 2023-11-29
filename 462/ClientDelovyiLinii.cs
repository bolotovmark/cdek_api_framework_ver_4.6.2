using System;
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
            
            req.ContentLength = sentData.Length;
            Stream sendStream = req.GetRequestStream();
            sendStream.Write(sentData, 0, sentData.Length);

            var res = req.GetResponse() as HttpWebResponse;
            var resStream = res.GetResponseStream();
            var sr = new StreamReader(resStream, Encoding.UTF8);

            return sr.ReadToEnd();
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

        public string CalculatePrice(string cityArrival, double mass, double volume)
        {
            string urlRequest = "https://api.dellin.ru/v2/calculator.json";
            string massStr = mass.ToString(CultureInfo.InvariantCulture);
            string volumeStr = volume.ToString(CultureInfo.InvariantCulture);
            string firstDate = GetFirstDate("г. Чайковский, ул. Промышленная, 8/25", massStr, volumeStr);
            if (!cityArrival.Contains(","))
            {
                cityArrival += ", Ленина 1";
            }
            
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
            Match match = Regex.Match(responce, pattern);

            return match.Groups[1].Value;
        }
    }
}