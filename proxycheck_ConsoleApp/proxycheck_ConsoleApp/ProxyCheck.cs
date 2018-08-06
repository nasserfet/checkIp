using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace proxycheck_ConsoleApp
{
    public class ProxyCheck
    {
        public void Check(string path_input, string path_output)
        {
            try
            {
                int counter = 0;
                List<string> listProxy_output = new List<string>();
                List<string> listProxy_input = new List<string>();
                
                listProxy_input = ReadCsvFile(path_input);

                listProxy_input = MatchIP(listProxy_input);
                var totalCount = listProxy_input.Count;
                var consoleTitle = Console.Title;
                foreach (string proxy in listProxy_input)
                {
                    ++counter;
                    bool isActive = TryIp(proxy, proxy);
                    OutPutMessage(proxy, isActive, counter, totalCount, consoleTitle);

                    if (isActive)
                    {
                        listProxy_output.Add(proxy);
                    }
                }
                WriteCsvFile(listProxy_output, path_output);
            }
            catch (Exception ex) { throw ex; }
        }
        #region csv
        private List<string> ReadCsvFile(string filePath)
        {
            try
            {
                using (var reader = new StreamReader(filePath))
                {
                    List<string> list = new List<string>();
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(';');

                        list.AddRange(values);
                    }
                    return list;
                }
            }
            catch (Exception ex)
            { throw ex; }
        }
        private void WriteCsvFile(List<string> items, string filePath)
        {
            var stringBuilder = new StringBuilder();
            foreach (var item in items)
            {
                stringBuilder.AppendLine(item + ";");
            }

            try
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(filePath, true))
                {
                    file.WriteLine("Created at : " + DateTime.Now + Environment.NewLine);
                    file.WriteLine(stringBuilder);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        private static void OutPutMessage(string proxy,bool isActive, int counter, int totalCount , string consoleTitle) {
            var progress_percent = (double)((double)counter / (double)totalCount) * 100;
            Console.Title = string.Format("{0} - progress : {1} %",consoleTitle , progress_percent);

            if (isActive)
            {
                Console.BackgroundColor = ConsoleColor.Cyan;
                Console.ForegroundColor = ConsoleColor.Blue;
            }
            else {
                Console.BackgroundColor = ConsoleColor.Cyan;
                Console.ForegroundColor = ConsoleColor.Red;
            }

            Console.WriteLine("executing proxy : {0} ,status : {1} ,{2} progress : {3} of {4} at {5} , progress : {6}% {7}", proxy, isActive, Environment.NewLine, counter, totalCount, DateTime.Now, progress_percent, Environment.NewLine);
            Console.ResetColor();
        }

        #region ip
        private List<string> MatchIP(List<string> items,bool checkPort = true)
        {
            List<string> items_output = new List<string>();
            string regexPatern_ip = checkPort 
                ? "[0-9]{1,3}.[0-9]{1,3}.[0-9]{1,3}.[0-9]{1,3}:[0-9]{1,5}" 
                : "[0-9]{1,3}.[0-9]{1,3}.[0-9]{1,3}.[0-9]{1,3}" ;

            try
            {
                foreach (string item in items)
                {
                    List<string> matched_items = MatchRegex(item, regexPatern_ip);
                    if (matched_items.Count > 0)
                    {
                        items_output.AddRange(matched_items);
                    }
                }
                return items_output;
            }
            catch (Exception ex)
            { throw ex; }
        }

        private bool TryIp(string ip, string proxy)
        {
            try
            {
                Thread.Sleep(60000 / 149);
                string returnedIp = GetIp(proxy);

                returnedIp = MatchIP(new List<string> { returnedIp }, false).FirstOrDefault();
                ip = MatchIP(new List<string> { ip }, false).FirstOrDefault();
                if (returnedIp == ip)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex) { throw ex; }
        }

        private string GetIp(string proxyUri, string url = "http://ip-api.com/line/?fields=query")
        {
            string responseBody = string.Empty;

            try
            {
                HttpClient httpClient;
                if (!string.IsNullOrEmpty(proxyUri))
                {
                    WebProxy proxy = new WebProxy(proxyUri, false)
                    {
                        UseDefaultCredentials = true,
                    };

                    HttpClientHandler httpClientHandler = new HttpClientHandler()
                    {
                        Proxy = proxy,
                        PreAuthenticate = false,
                        UseDefaultCredentials = true,
                    };
                    httpClient = new HttpClient(httpClientHandler);
                }
                else
                {
                    httpClient = new HttpClient();
                }
                httpClient.Timeout = TimeSpan.FromSeconds(1);

                HttpResponseMessage response = httpClient.GetAsync(url).Result;
                //response.EnsureSuccessStatusCode
                if (response.IsSuccessStatusCode)
                {
                    responseBody = response.Content.ReadAsStringAsync().Result;
                }
            }
            catch (Exception ex)
            { 
                //throw ex; 
            }
            return responseBody;

        }
        #endregion
        private List<string> MatchRegex(string text, string regexPatern)
        {
            List<string> items_output = new List<string>();
            Regex regex = new Regex(regexPatern);

            foreach (Match match in regex.Matches(text))
            {
                items_output.Add(match.Value);
            }
        
        return items_output;
        }
    }
}
